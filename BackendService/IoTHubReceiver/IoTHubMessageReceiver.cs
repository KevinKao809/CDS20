using CDSShareLib;
using CDSShareLib.Helper;
using IoTHubReceiver.Enums;
using IoTHubReceiver.Helper;
using IoTHubReceiver.Model;
using IoTHubReceiver.Utilities;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yare.Lib.Rules;
using static CDSShareLib.Helper.AzureSQLHelper.EventRuleCatalogModel;

namespace IoTHubReceiver
{
    public class IoTHubMessageReceiver
    {
        private class CompatibleEventHub
        {
            public string EndpointName { get; set; }
            public string IoTHubConnectionString { get; set; }
            public string ConsumerGroup { get; set; }
            public string StorageConnectionString { get; set; }

            public CompatibleEventHub(string endpointName,
                string iothubConnectionString,
                string consumerGroup,
                string storageConnectionString)
            {
                this.EndpointName = endpointName;
                this.IoTHubConnectionString = iothubConnectionString;
                this.ConsumerGroup = consumerGroup;
                this.StorageConnectionString = storageConnectionString;
            }
        }
        private CompatibleEventHub _CompatibleEventHub;

        public static bool _isRunning = false;
        public static string _runStatus = "";
        private static ConsoleLog _consoleLog;

        private EventProcessorHost _eventProcessorHost;
        private EventProcessorFactoryModel _epfm;
        private bool _ignoreCDS20DB = IoTHubReceiver._ignoreCDS20DB;
        private static char REPLACE_SPACE_TO_DASH = '-';

        public IoTHubMessageReceiver(CdsInfo cdsInfo)
        {
            _consoleLog = IoTHubReceiver._consoleLog;
            _epfm = new EventProcessorFactoryModel();
            _epfm.CdsInfo = cdsInfo;
        }

        public async Task<bool> Start()
        {
            if (_isRunning)
                return true;

            _runStatus = "Initial";
            EventProcessorFactoryModel epfm = _epfm;
            CdsInfo cdsInfo = epfm.CdsInfo;

            await loadConfigurationFromDB(epfm);

            string leaseName = getLeaseName(cdsInfo.CompanyId, cdsInfo.IoTHubId, cdsInfo.IoTHubAlias);
            string hostName = getHostName(cdsInfo.CompanyId, cdsInfo.IoTHubId, cdsInfo.PartitionNum, cdsInfo.IoTHubAlias);
            _consoleLog.Info("hostName= {0}, leaseName= {1}", hostName, leaseName);

            _eventProcessorHost = new EventProcessorHost(
                hostName, // Task ID
                _CompatibleEventHub.EndpointName, // Endpoint: messages/events
                _CompatibleEventHub.ConsumerGroup,// Consumer Group
                _CompatibleEventHub.IoTHubConnectionString, // IoT Hub Connection String
                _CompatibleEventHub.StorageConnectionString,
                leaseName,
                leaseName);

            _consoleLog.Info("Registering IoTHubAliasEventMessageReceiver on {0} - partition {1}", cdsInfo.IoTHubAlias, cdsInfo.PartitionNum);

            var options = new EventProcessorOptions
            {
                InitialOffsetProvider = (partitionId) => DateTime.UtcNow
            };
            options.ExceptionReceived += (sender, e) =>
            {
                if (sender == null)
                    return; // ignore others processor with un-tracked partitions.

                _consoleLog.Error("EventProcessorOptions Exception:{0}", e.Exception);
                _consoleLog.BlobLogError("EventProcessorOptions Exception:{0}", e.Exception);
            };

            try
            {
                await _eventProcessorHost.RegisterEventProcessorFactoryAsync(new IoTHubMessageProcessorFactory(epfm), options);
                _isRunning = true;
                _runStatus = "Good";
            }
            catch (Exception ex)
            {
                _isRunning = false;
                _consoleLog.Error("RegisterEventProcessorFactoryAsync Fail. IoTHubId: {0}, PartitionNum: {1}, Exception: {2}", cdsInfo.IoTHubId, cdsInfo.PartitionNum, ex.Message);
                _consoleLog.BlobLogError("RegisterEventProcessorFactoryAsync Fail. IoTHubId: {0}, PartitionNum: {1}, Exception: {2}", cdsInfo.IoTHubId, cdsInfo.PartitionNum, ex.Message);
                _runStatus = "Error";
                throw ex;
            }

            return true;
        }

        public async Task<bool> Stop()
        {
            if (!_isRunning)
                return true;

            await _eventProcessorHost.UnregisterEventProcessorAsync();
            _isRunning = false;

            CdsInfo cdsInfo = _epfm.CdsInfo;
            _consoleLog.Info("Unregistering IoTHubAliasEventMessageReceiver on {0} - partition {1}", cdsInfo.IoTHubAlias, cdsInfo.PartitionNum);
            _consoleLog.BlobLogInfo("Unregistering IoTHubAliasEventMessageReceiver on {0} - partition {1}", cdsInfo.IoTHubAlias, cdsInfo.PartitionNum);
            return true;
        }

        private async Task loadConfigurationFromDB(EventProcessorFactoryModel epfm)
        {
            CdsInfo cdsInfo = epfm.CdsInfo;
            IoTHub iotHub;
            CompanyInSubscriptionPlan cisp;
            string telemetryStorageContainer;
            string eventStorageContainer;
            if (_ignoreCDS20DB)
            {
                /* For Test */
                TestSampleHelper tsHelper = new TestSampleHelper();
                iotHub = tsHelper.generateIoTHubForTest(cdsInfo.IoTHubId);

                cisp = new CompanyInSubscriptionPlan();
                cisp.ExpiredDate = DateTime.UtcNow.AddMonths(1);// Test
                cisp.StoreColdMessage = true;
                cisp.StoreHotMessage = true;
                /* Walker's Cosmos DB */
                cisp.CosmosDBConnectionString = "https://sfdev.documents.azure.com:443/;AccountKey=PHsydcvXyVdELDtWTgLvlbrP5ohuaJbMKQNNCxZKR1UPwS45qVkYiTuXR6wTm9PhnqIDe5IwUoQ0fqmk28CJww==;";// "https://sfdocumentdb.documents.azure.com:443/;AccountKey=uscYe8taxXEtIIzQjCM47T3y3F53wMn2QOPUOnZu55oBClFnzOzfd5UDSlMixgCR6aqBNbHebJmIgoSmdk2MxQ==;";
                cisp.CosmosDBName = "db69";
                cisp.CosmosDBCollectionID = "69";

                telemetryStorageContainer = "telemetry";
                eventStorageContainer = "event";
            }
            else
            {
                /* For CDS 2.0 DB */
                AzureSQLHelper.IoTHubModel ioTHubModel = new AzureSQLHelper.IoTHubModel();
                iotHub = ioTHubModel.GetById(Int32.Parse(cdsInfo.IoTHubId));

                AzureSQLHelper.CompanyModel companyModel = new AzureSQLHelper.CompanyModel();
                cisp = companyModel.GetActiveSubscriptionPlanByCompanyId(Int32.Parse(cdsInfo.CompanyId));

                telemetryStorageContainer = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("TelemetryStorageContainer");
                telemetryStorageContainer = telemetryStorageContainer == null ? "telemetry" : telemetryStorageContainer;
                eventStorageContainer = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EventStorageContainer");
                eventStorageContainer = eventStorageContainer == null ? "event" : eventStorageContainer;
            }

            if (iotHub == null)
            {
                _consoleLog.Info("IoTHub Not Found. IoTHubId:{0}", cdsInfo.IoTHubId);
                _consoleLog.BlobLogError("IoTHub Not Found. IoTHubId:{0}", cdsInfo.IoTHubId);
                throw new Exception("IoTHub Not Found");
            }
            if (cisp == null)
            {
                _consoleLog.Info("NO Actived CompanyInSubscriptionPlan. CompanyId:{0}", cdsInfo.CompanyId);
                _consoleLog.BlobLogError("NO Actived CompanyInSubscriptionPlan. CompanyId:{0}", cdsInfo.CompanyId);
                throw new Exception("NO Actived CompanyInSubscriptionPlan");
            }

            /* IoTHubAlias */
            cdsInfo.IoTHubAlias = iotHub.IoTHubName;

            /* CompanyInSubscriptionPlan */
            cdsInfo.CompanyInSubscriptionPlan = cisp;

            /* Load the message schema from IoT Hub devices */
            epfm.SimpleIoTDeviceMessageCatalogList = findAllMessageCatalogIdInIoTDevices(iotHub.IoTDevice);

            /* Load the message JSON transformer from IoT Hub devices */
            epfm.MessageTransformerInDeviceId = findAllMessageTransformers(iotHub.IoTDevice);

            /* Load Alarm Rules */
            epfm.EventRulesInMessageId = findAllEventRules(iotHub);

            /* Load Monitor Frequence In MinSec By MessageId */
            epfm.MonitorFrequenceInMinSecByMessageId = findMonitorFrequenceInMinSecByMessageId(iotHub.IoTDevice);

            /* Load IoT Hub configuration */
            _CompatibleEventHub = findCompatibleEventHub(iotHub);

            /* Cosmos DB Helper */
            epfm.docDBHelper = await createDocDBHelper(cdsInfo.CompanyInSubscriptionPlan);

            /* Blob Helper */
            epfm.TelemetryBlobStorageHelper = new BlobStorageHelper(_CompatibleEventHub.StorageConnectionString, telemetryStorageContainer);
            epfm.EventBlobStorageHelper = new BlobStorageHelper(_CompatibleEventHub.StorageConnectionString, eventStorageContainer);

            /* Service Bus Helper */
            CdsBackendSetting cdsBackendSetting = cdsInfo.cdsBackendSetting;
            epfm.EventQueueClient = QueueClient.CreateFromConnectionString(cdsBackendSetting.ServiceBusConnectionString, cdsBackendSetting.ServiceBusEventActionQueue);
            epfm.InfraQueueClient = QueueClient.CreateFromConnectionString(cdsBackendSetting.ServiceBusConnectionString, cdsBackendSetting.ServiceBusProvisionQueue);
        }

        private async Task<DocDBHelper> createDocDBHelper(CompanyInSubscriptionPlan cisp)
        {
            try
            {
                DocDBHelper docDBHelper = new DocDBHelper(cisp.CosmosDBConnectionString);

                await docDBHelper.checkDatabaseIfNotExists(cisp.CosmosDBName);

                await docDBHelper.checkCollectionIfNotExists(cisp.CosmosDBName, cisp.CosmosDBCollectionID);

                //CosmosDbHelper cdsCosmosDbHelper = new CosmosDbHelper(cisp.CosmosDBConnectionString, cisp.CosmosDBName, cisp.CosmosDBCollectionID);

                //await cdsCosmosDbHelper.checkDatabaseIfNotExists();

                //await cdsCosmosDbHelper.checkCollectionIfNotExists();

                return docDBHelper;
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                _consoleLog.Error("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                _consoleLog.BlobLogError("fetchDocumentDB: {0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                throw;
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                _consoleLog.Error("Error: {0}, Message: {1}", e.Message, baseException.Message);
                _consoleLog.BlobLogError("fetchDocumentDB: Error: {0}, Message: {1}", e.Message, baseException.Message);
                throw;
            }
        }

        private string getLeaseName(string companyId, string iotHubId, string iotHubAliasName)
        {
            string leaseName = "C" + companyId + "_I" + iotHubId + "_" + iotHubAliasName;
            if (leaseName.Contains(" "))
                leaseName = leaseName.Replace(' ', REPLACE_SPACE_TO_DASH);

            if (leaseName.Contains("_"))
                leaseName = leaseName.Replace('_', REPLACE_SPACE_TO_DASH);

            leaseName = leaseName.ToLower();
            return leaseName;
        }

        private string getHostName(string companyId, string iotHubId, string partition, string iotHubAliasName)
        {
            string hostName = "C" + companyId + "_I" + iotHubId + "_P" + partition + "_" + iotHubAliasName;
            if (hostName.Contains(" "))
                hostName = hostName.Replace(' ', REPLACE_SPACE_TO_DASH);

            if (hostName.Contains("_"))
                hostName = hostName.Replace('_', REPLACE_SPACE_TO_DASH);

            hostName = hostName.ToLower();
            return hostName;
        }

        private List<SimpleIoTDeviceMessageCatalog> findAllMessageCatalogIdInIoTDevices(ICollection<IoTDevice> iotDevices)
        {
            List<SimpleIoTDeviceMessageCatalog> simpleDevMsgCatalogList = new List<SimpleIoTDeviceMessageCatalog>();

            foreach (IoTDevice iotDevice in iotDevices)
            {
                SimpleIoTDeviceMessageCatalog simpleDevMsgCatalog = new SimpleIoTDeviceMessageCatalog();

                simpleDevMsgCatalog.DeviceId = iotDevice.IoTHubDeviceID;
                simpleDevMsgCatalog.MessageCatalogIds = new List<int>();

                foreach (Equipment equipment in iotDevice.Equipment)
                {
                    foreach (EquipmentClassMessageCatalog equipmentClassMessageCatalog in equipment.EquipmentClass.EquipmentClassMessageCatalog)
                    {
                        if (simpleDevMsgCatalog.MessageCatalogIds.Contains(equipmentClassMessageCatalog.MessageCatalogID) == false)
                            simpleDevMsgCatalog.MessageCatalogIds.Add(equipmentClassMessageCatalog.MessageCatalogID);
                    }
                }

                simpleDevMsgCatalogList.Add(simpleDevMsgCatalog);
            }

            showSimpleIoTDeviceMessageCatalogList(simpleDevMsgCatalogList);

            return simpleDevMsgCatalogList;
        }

        private Dictionary<int, MonitorFrequency> findMonitorFrequenceInMinSecByMessageId(ICollection<IoTDevice> iotDevices)
        {
            Dictionary<int, MonitorFrequency> dictionary = new Dictionary<int, MonitorFrequency>();

            foreach (IoTDevice iotDevice in iotDevices)
            {
                foreach (Equipment equipment in iotDevice.Equipment)
                {
                    foreach (EquipmentClassMessageCatalog ecmc in equipment.EquipmentClass.EquipmentClassMessageCatalog)
                    {
                        if (dictionary.ContainsKey(ecmc.MessageCatalogID) == false)
                        {
                            MonitorFrequency mf = new MonitorFrequency();
                            mf.timeInMilliSecond = ecmc.MessageCatalog.MonitorFrequenceInMinSec;
                            mf.lastFeedInTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                            dictionary.Add(ecmc.MessageCatalogID, mf);
                        }
                    }
                }
            }

            showMonitorFrequenceInMinSecByMessageId(dictionary);

            return dictionary;
        }

        private void showMonitorFrequenceInMinSecByMessageId(Dictionary<int, MonitorFrequency> d)
        {
            foreach (KeyValuePair<int, MonitorFrequency> item in d)
            {
                MonitorFrequency mf = item.Value;
                _consoleLog.Info("*** MessageCatalogID:{0} MonitorFrequenceInMilliSec: {1} ---", item.Key, mf.timeInMilliSecond);
                _consoleLog.BlobLogInfo("*** MessageCatalogID:{0} MonitorFrequenceInMilliSec: {1} ---", item.Key, mf.timeInMilliSecond);
            }
        }

        private Dictionary<string, MessageTransformer> findAllMessageTransformers(ICollection<IoTDevice> iotDevices)
        {
            Dictionary<string, MessageTransformer> allMessageTransformers = new Dictionary<string, MessageTransformer>();

            foreach (IoTDevice iotDevice in iotDevices)
            {
                string deviceId = iotDevice.IoTHubDeviceID;

                if (iotDevice.EnableMessageConvert)
                {
                    try
                    {
                        string transformJsonFile = iotDevice.MessageConvertScript;
                        JObject transformJObject = JObject.Parse(transformJsonFile);
                        MessageTransformer messageTransformer = new MessageTransformer();
                        JToken id;
                        if (transformJObject.TryGetValue("messageCatalogId", out id))
                        {
                            messageTransformer.MessageCatalogID = id.Value<int>();

                            // Remove this property
                            transformJObject.Remove("messageCatalogId");

                            messageTransformer.TransformJson = transformJObject.ToString();
                            allMessageTransformers.Add(deviceId, messageTransformer);

                            _consoleLog.Info("Enable Message Convert - Device ID: {0}", deviceId);
                        }
                        else
                        {
                            _consoleLog.Error("Could not find MessageCatalogID in Script file. - Device ID: {0}", deviceId);
                            _consoleLog.BlobLogError("Could not find MessageCatalogID in Script file. - Device ID: {0}", deviceId);
                        }

                    }
                    catch (Exception)
                    {
                        _consoleLog.Error("Could not parse Message Convert Script - Device ID: {0}", deviceId);
                        _consoleLog.BlobLogError("Could not parse Message Convert Script - Device ID: {0}", deviceId);
                    }
                }

            }

            return allMessageTransformers;
        }

        private Dictionary<int, List<EventRuleCatalogEngine>> findAllEventRules(IoTHub ioTHub)
        {
            Dictionary<int, List<EventRuleCatalogEngine>> messageIdAlarmRules = new Dictionary<int, List<EventRuleCatalogEngine>>();
            Dictionary<int, MessageCatalog> mcDictionary = new Dictionary<int, MessageCatalog>();

            foreach (IoTDevice iotDevice in ioTHub.IoTDevice)
            {
                foreach (Equipment equipment in iotDevice.Equipment)
                {
                    foreach (EquipmentClassMessageCatalog ecmc in equipment.EquipmentClass.EquipmentClassMessageCatalog)
                    {
                        if (mcDictionary.ContainsKey(ecmc.MessageCatalogID) != true)
                            mcDictionary.Add(ecmc.MessageCatalogID, ecmc.MessageCatalog);
                    }
                }
            }

            foreach (KeyValuePair<int, MessageCatalog> item in mcDictionary)
            {
                List<EventRuleCatalogEngine> arcEngineList = new List<EventRuleCatalogEngine>();
                foreach (EventRuleCatalog erc in item.Value.EventRuleCatalog)
                {
                    if (erc.ActiveFlag)
                    {
                        _consoleLog.MessageEventDebug("EventRuleCatalogId={0}", erc.Id);

                        EventRuleCatalogEngine are = new EventRuleCatalogEngine();
                        are.EventRuleCatalogId = erc.Id;
                        are.EventRuleCatalog = erc;
                        are.RuleEngineItems = createRuleEngineItem(erc);
                        are.LastTriggerTime = new DateTime(2017, 1, 1);
                        are.Triggered = false;

                        arcEngineList.Add(are);
                    }
                }

                messageIdAlarmRules.Add(item.Key, arcEngineList);

            }

            return messageIdAlarmRules;
        }

        private CompatibleEventHub findCompatibleEventHub(IoTHub iotHub)
        {
            return new CompatibleEventHub(
                iotHub.IoTHubEndPoint, // messages/events
                iotHub.IoTHubConnectionString, // IoT Hub Connection String
                iotHub.EventConsumerGroup, // Consumer Group
                iotHub.EventHubStorageConnectionString); // Storage Connection String
        }

        private Dictionary<string, RuleEngineItem> createRuleEngineItem(EventRuleCatalog eventRuleCatalog)
        {
            List<DetailForRuleEngineModel> detailForRuleEngineModelList = new List<DetailForRuleEngineModel>();

            foreach (EventRuleItem erItem in eventRuleCatalog.EventRuleItem)
            {
                DetailForRuleEngineModel returnData = new DetailForRuleEngineModel();
                returnData.Id = erItem.Id;
                returnData.EventRuleCatalogId = eventRuleCatalog.Id;
                returnData.Ordering = erItem.Ordering;
                returnData.MessageElementId = erItem.MessageElementId;
                returnData.EqualOperation = erItem.EqualOperation;
                returnData.Value = erItem.Value;
                returnData.BitWiseOperation = erItem.BitWiseOperation;
                returnData.MessageElementDataType = erItem.MessageElement1.ElementDataType;

                if (erItem.MessageElement != null)
                    returnData.MessageElementFullName = erItem.MessageElement.ElementName + "_" + erItem.MessageElement1.ElementName;
                else
                    returnData.MessageElementFullName = erItem.MessageElement1.ElementName;

                detailForRuleEngineModelList.Add(returnData);
            }

            Dictionary<string, RuleEngineItem> ruleEngineItems = new Dictionary<string, RuleEngineItem>();

            int index = 0;
            foreach (var detailForRuleEngineModel in detailForRuleEngineModelList)
            {
                RuleEngineItem rei = new RuleEngineItem();
                rei.ElementName = detailForRuleEngineModel.MessageElementFullName;
                rei.DataType = AlarmRuleItemEngineUtility.GetSupportDataType(detailForRuleEngineModel.MessageElementDataType);
                rei.OrderOperation = detailForRuleEngineModel.BitWiseOperation;
                rei.Result = false;

                _consoleLog.MessageEventDebug("--ElementName={0}, BitWiseOperation={1}", rei.ElementName, rei.OrderOperation);

                if (rei.DataType == SupportDataTypeEnum.String &&
                    (string.IsNullOrEmpty(detailForRuleEngineModel.Value) || detailForRuleEngineModel.Value.ToLower().Equals("null")))
                {
                    detailForRuleEngineModel.Value = "null";
                }

                if (rei.DataType == SupportDataTypeEnum.Numeric || rei.DataType == SupportDataTypeEnum.Bool)
                {
                    rei.Equality = createCompiledRuleFunc(rei.DataType, detailForRuleEngineModel.EqualOperation, detailForRuleEngineModel.Value);
                }
                else
                {
                    // SupportDataTypeEnum.String
                    rei.Equality = null;
                    rei.StringRightValue = detailForRuleEngineModel.Value;

                    if (detailForRuleEngineModel.EqualOperation.Equals("=") || detailForRuleEngineModel.EqualOperation.Equals("!="))
                        rei.StringEqualOperation = detailForRuleEngineModel.EqualOperation;
                    else
                        throw new ArgumentNullException("String equal operation is not supported - " + detailForRuleEngineModel.EqualOperation);

                    _consoleLog.MessageEventDebug("----ruleText=({0} {1} {2})", rei.ElementName, detailForRuleEngineModel.EqualOperation, rei.StringRightValue);
                }

                // Add the index to avoid the duplicate key
                ruleEngineItems.Add(rei.ElementName + "-" + index, rei);
                index++;
            }

            return ruleEngineItems;
        }

        private Func<DynamicMessageElement, bool> createCompiledRuleFunc(SupportDataTypeEnum supportedDataType, string op, string right)
        {
            EqualityRule rule;

            switch (supportedDataType)
            {
                case SupportDataTypeEnum.Bool:
                    rule = new EqualityRule("StringValue", AlarmRuleItemEngineUtility.GetEqualityOperation(op), right.ToLower());
                    break;
                case SupportDataTypeEnum.Numeric:
                    decimal rightValue = Decimal.Parse(right);
                    if (rightValue >= 0)
                        rule = new EqualityRule("DecimalValue", AlarmRuleItemEngineUtility.GetEqualityOperation(op), right);
                    else
                    {
                        rule = new EqualityRule("DecimalValue", AlarmRuleItemEngineUtility.GetEqualityOperation(op), "0-" + Decimal.Negate(rightValue).ToString());
                    }
                    break;
                case SupportDataTypeEnum.String:
                    rule = new EqualityRule("StringValue", AlarmRuleItemEngineUtility.GetEqualityOperation(op), right);
                    break;
                default:
                    throw new NotSupportedException();
            }

            RuleBase rb = null;
            using (var mem = new MemoryStream())
            {
                Serializer.Serialize(mem, rule);
                mem.Position = 0;
                rb = Serializer.Deserialize<RuleBase>(mem);
            }

            string ruleText;
            Func<DynamicMessageElement, bool> compiledRule = rb.CompileRule<DynamicMessageElement>(out ruleText);

            //_consoleLog.MessageAlarmDebug("----ruleText={0}", ruleText);
            //_consoleLog.MessageAlarmDebug("----ruleText="+ruleText);

            return compiledRule;
        }

        private void showSimpleIoTDeviceMessageCatalogList(List<SimpleIoTDeviceMessageCatalog> simpleIoTDeviceMessageCatalogList)
        {
            int count = 0;
            foreach (SimpleIoTDeviceMessageCatalog simpleDeviceMsgCatalog in simpleIoTDeviceMessageCatalogList)
            {
                _consoleLog.Info("--- Device [{0}] {1} ---", count, simpleDeviceMsgCatalog.DeviceId);
                count++;

                int msgCount = 0;
                foreach (int msgCatalogId in simpleDeviceMsgCatalog.MessageCatalogIds)
                {
                    _consoleLog.Info("[{0}] msgCatalogId {1}", msgCount, msgCatalogId);
                    msgCount++;
                }
            }
        }
    }
}
