using CDSShareLib.Helper;
using IoTHubReceiver.Enums;
using IoTHubReceiver.Helper;
using IoTHubReceiver.Model;
using IoTHubReceiver.Utilities;
using JUST;
using Microsoft.Azure.Documents;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubReceiver
{
    class IoTHubMessageEventProcessor : IEventProcessor
    {
        private const string MESSAGE_PROPERTY_TYPE = "MessageType";
        private const string MESSAGE_PROPERTY_TYPE_COMMAND = "Command";
        private const string MESSAGE_PROPERTY_TYPE_MESSAGE = "Telemetry";
        private const string MESSAGE_PROPERTY_MESSAGECATALOGID = "MessageCatalogId";
        private const string CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND = "CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND";
        private const string SF_LASTUPDATED_TIMESTAMP = "SF_LastUpdatedTimestamp";

        //private static RealTimeMessageSender _RTMessageSender;
        private static WebHelper webHelper = new WebHelper();
        private static string _RTMessageFeedInURL;
        private static ConsoleLog _consoleLog;
        private EventProcessorFactoryModel _epfm;
        private int _companyId;
        private RedisCacheHelper _redisCacheHelper = new RedisCacheHelper();
        private RedisKey _cachekey_message_quota_per_day;
        private RedisKey _cachekey_message_total_consumed_count;
        private RedisKey _cachekey_message_consumed_date;
        private long _messageQuotaPerDay;
        private long _messageConsumedCount;
        private DateTime _messageConsumedDate;

        private long _currentMsgCountBySizeInBytes;
        private long _msgSizeInBytes;
        private bool _eventProcessingEnabled;
        private bool _storeHotMessageEnabled;
        private bool _storeColdMessageEnabled;

        public IoTHubMessageEventProcessor(EventProcessorFactoryModel epfm)
        {
            _consoleLog = IoTHubReceiver._consoleLog;

            _epfm = epfm;
            _companyId = Int32.Parse(epfm.CdsInfo.CompanyId);

            _cachekey_message_quota_per_day = "C_" + _companyId + "_MessageQuotaPerDay";
            _cachekey_message_total_consumed_count = "C_" + _companyId + "_MessageConsumed";
            _cachekey_message_consumed_date = "C_" + _companyId + "_MessageConsumedDate";

            _msgSizeInBytes = 4096;// Hard Code now.

            _storeHotMessageEnabled = epfm.CdsInfo.CompanyInSubscriptionPlan.StoreHotMessage;
            _storeColdMessageEnabled = epfm.CdsInfo.CompanyInSubscriptionPlan.StoreColdMessage;

            //_RTMessageSender = new RealTimeMessageSender(epfm.CdsInfo.cdsBackendSetting.RTMessageFeedInURL);
            _RTMessageFeedInURL = epfm.CdsInfo.cdsBackendSetting.RTMessageFeedInURL;
        }

        Stopwatch checkpointStopWatch;
        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            _consoleLog.Info("IoTHubEventProcessor Shutting Down. Partition '{0}',  and Owner: {1} with Reason: '{2}'.", context.Lease.PartitionId, context.Lease.Owner ?? string.Empty, reason);
            _consoleLog.BlobLogInfo("IoTHubEventProcessor Shutting Down. Partition '{0}',  and Owner: {1} with Reason: '{2}'.", context.Lease.PartitionId, context.Lease.Owner ?? string.Empty, reason);

            if ((reason == CloseReason.Shutdown) || (reason == CloseReason.LeaseLost))
            {
                checkpointToRedisCache();
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            _consoleLog.Info("IoTHubEventProcessor initialized.  Partition: '{0}', Offset: '{1}' and Owner: {2}.", context.Lease.PartitionId, context.Lease.Offset, context.Lease.Owner ?? string.Empty);
            _consoleLog.BlobLogInfo("IoTHubEventProcessor initialized.  Partition: '{0}', Offset: '{1}' and Owner: {2}.", context.Lease.PartitionId, context.Lease.Offset, context.Lease.Owner ?? string.Empty);

            setEventProcessingEnabled(checkAndUpdateConsumedMessageCache(DateTime.UtcNow, false));
            setCurrentMsgCountBySizeInBytes(0);

            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            /* Enabled by Message Quota */
            if (eventProcessingEnabled())
            {
                await processEvents(messages);
            }
            else
            {
                _consoleLog.Info("(Disabled) Message Processing");
            }
            //Call checkpoint every 5 minutes, so that worker can resume processing from 5 minutes back if it restarts.
            if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            //if (this.checkpointStopWatch.Elapsed > TimeSpan.FromSeconds(30))
            {
                checkpointToRedisCache();
                await context.CheckpointAsync();
                this.checkpointStopWatch.Restart();
            }
        }

        private async Task processEvents(IEnumerable<EventData> messages)
        {
            foreach (EventData eventData in messages)
            {
                try
                {
                    updateCurrentMsgCountBySizeInBytes(eventData.SerializedSizeInBytes);

                    string deviceId = getDeviceId(eventData);
                    if (deviceId == null)
                        continue;

                    if (isTwinChangedEvent(eventData))
                    {
                        processTwinChangedEvent(eventData);
                    }
                    else
                    {
                        await processMessageEvent(deviceId, eventData);
                    }

                }
                catch (Exception ex)
                {
                    _consoleLog.Warn("(Skipped) Unsupported Message:{0}", ex.Message.ToString());
                    _consoleLog.BlobLogWarn("(Skipped) Unsupported Message:{0}", ex.Message.ToString());
                }
            }
        }

        private void processTwinChangedEvent(EventData eventData)
        {
            string data = Encoding.UTF8.GetString(eventData.GetBytes());
            JObject payload = JObject.Parse(data);

            //_consoleLog.Info("(twinChangedEvent) payload:{0}", payload.ToString());
            var desired = payload["properties"]["desired"];
            var reported = payload["properties"]["reported"];

            if (desired != null)
            {
                //_consoleLog.Info("---- desired:{0}", desired.ToString());
            }

            if (reported != null)
            {
                //ConsoleLog.WriteToConsole("---- reported:{0}", reported.ToString());
                var systemConfig = reported["SF_SystemConfig"];
                var customizedConfig = reported["SF_CustomizedConfig"];

                if (systemConfig != null)
                    _consoleLog.Info("---- reported: System Config was updated");

                if (customizedConfig != null)
                    _consoleLog.Info("---- reported: Customized Config was updated");
            }

            // to do something here




        }

        private async Task processMessageEvent(string deviceId, EventData eventData)
        {
            string data = Encoding.UTF8.GetString(eventData.GetBytes());

            // Convert the message if needed
            if (checkConvertMessageIfNeed(deviceId, eventData))
            {
                await convertMessageByScript(deviceId, data);
            }
            else
            {
                JObject payload = JObject.Parse(data);

                string messageType = getMessageProperty(eventData, MESSAGE_PROPERTY_TYPE);
                if (messageType != null)
                {
                    // New version of message processing ( + device management)
                    switch (messageType)
                    {
                        case MESSAGE_PROPERTY_TYPE_COMMAND:
                            //ConsoleLog.WriteToConsole("MESSAGE_PROPERTY_TYPE_COMMAND");
                            sendCommandToServiceBusQueue(_epfm, deviceId, payload);
                            break;
                        case MESSAGE_PROPERTY_TYPE_MESSAGE:
                            //ConsoleLog.WriteToConsole("MESSAGE_PROPERTY_TYPE_MESSAGE");
                            await processMessages(deviceId,
                                getMessageCatalogIdInMessageProperty(eventData),
                                payload);
                            break;
                    }
                }
            }
        }

        private bool checkConvertMessageIfNeed(string deviceId, EventData eventData)
        {
            bool transformNeeded = false;

            transformNeeded = _epfm.MessageTransformerInDeviceId.ContainsKey(deviceId);

            if (transformNeeded)
            {
                // Add one more rule for CDS format
                if (getMessageProperty(eventData, MESSAGE_PROPERTY_TYPE) == MESSAGE_PROPERTY_TYPE_MESSAGE &&
                    getMessageProperty(eventData, MESSAGE_PROPERTY_MESSAGECATALOGID) != null)
                {
                    transformNeeded = false;// This is CDS format
                }
            }

            return transformNeeded;
        }

        private async Task convertMessageByScript(string deviceId, string data)
        {
            var token = JToken.Parse(data);
            if (token is JArray)
            {
                IEnumerable<JObject> elementList = token.ToObject<List<JObject>>();

                foreach (var element in elementList)
                {
                    JObject transformObj = doTransform(deviceId, element.ToString());
                    if (transformObj != null)
                    {
                        await processMessages(deviceId, getMessageCatalogIdFromTransformer(deviceId), transformObj);
                    }
                }
            }
            else if (token is JObject)
            {
                JObject element = token.ToObject<JObject>();

                JObject transformObj = doTransform(deviceId, element.ToString());
                if (transformObj != null)
                {
                    await processMessages(deviceId, getMessageCatalogIdFromTransformer(deviceId), transformObj);
                }
            }
            else
            {
                _consoleLog.Warn("DeviceId: {0}, convertMessageByScript is not valid JSON format", deviceId);
                _consoleLog.BlobLogWarn("DeviceId: {0}, convertMessageByScript is not valid JSON format", deviceId);
            }
        }

        private JObject doTransform(string deviceId, string data)
        {
            try
            {
                string transformObj = JsonTransformer.Transform(getJsonStringFromTransformer(deviceId), data);

                JObject outputObject = JObject.Parse(transformObj);

                // Check mandatory properties
                if (!checkMandatoyProperties(outputObject))
                {
                    _consoleLog.Warn("Cloud not find the mandatory property'{0}', output: '{1}'", deviceId, outputObject.ToString());
                    _consoleLog.BlobLogWarn("Cloud not find the mandatory property'{0}', output: '{1}'", deviceId, outputObject.ToString());
                    return null;
                }

                JObject output = new JObject();
                foreach (JProperty jp in outputObject.Properties())
                {
                    // Igonre the PROPERTY NOT FOUND of property
                    if (!jp.Value.ToString().Equals(CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND))
                        output.Add(jp.Name, jp.Value);
                }

                //_consoleLog.Info("JsonTransformer.  deviceId: '{0}', output: '{1}'", deviceId, output);

                return output;
            }
            catch (Exception e)
            {
                _consoleLog.Error("Failed to transform msessages.  deviceId: {0}, payload: {1}, e={2}'", deviceId, data, e.ToString());
                _consoleLog.BlobLogError("Failed to transform msessages.  deviceId: {0}, payload: {1}, e={2}'", deviceId, data, e.ToString());
                return null;
            }
        }

        private async Task processMessages(string deviceId, int messageCatalogId, JObject payload)
        {
            if (hasExpiredDate())
            {
                _consoleLog.Warn("(Ignored) [{0}] Expried Date Message ID: {1}", deviceId, messageCatalogId);
                _consoleLog.BlobLogWarn("(Ignored) [{0}] Expried Date Message ID: {1}", deviceId, messageCatalogId);
                return;
            }

            if (vaildateDeviceMessageCatalog(deviceId, messageCatalogId))
            {
                /* Feed the message to Web API */
                if (validateFeedInMessageTimeWindows(messageCatalogId))
                    feedInMessage(deviceId, messageCatalogId, (JObject)payload.DeepClone());

                string messageDocumentId = "null";
                /* Put the message to CosmosDB (Hot Data) */
                if (_storeHotMessageEnabled)
                    messageDocumentId = await putMessage2CosmosDb(deviceId, messageCatalogId, payload);

                /* Put the message to Blob Storage (Cold Data) */
                if (_storeColdMessageEnabled)
                    await putMessage2BlobStorage(messageDocumentId, deviceId, payload);

                /* Run the event rules by message id */
                await runEventRules(_epfm, deviceId, messageCatalogId, payload, messageDocumentId);
            }
            else
            {
                _consoleLog.Warn("(Ignored) [{0}] Unsupported Message ID: {1}", deviceId, messageCatalogId);
                _consoleLog.BlobLogWarn("(Ignored) [{0}] Unsupported Message ID: {1}", deviceId, messageCatalogId);
            }
        }

        private bool validateFeedInMessageTimeWindows(int messageCatalogId)
        {
            bool ret = false;
            Dictionary<int, MonitorFrequency> d = _epfm.MonitorFrequenceInMinSecByMessageId;
            if (d.ContainsKey(messageCatalogId))
            {
                MonitorFrequency mf = d[messageCatalogId];
                DateTime nextFeedInTime = mf.lastFeedInTime.AddMilliseconds(mf.timeInMilliSecond);
                DateTime now = DateTime.UtcNow;
                if (now >= nextFeedInTime)
                {
                    /* Feed In*/
                    mf.lastFeedInTime = now;
                    ret = true;
                }
            }

            return ret;
        }

        private bool vaildateDeviceMessageCatalog(string deviceId, int msgCatalogId)
        {
            SimpleIoTDeviceMessageCatalog simpleDeviceMsgCatalog;
            var L2EQuery = from a in _epfm.SimpleIoTDeviceMessageCatalogList
                           where a.DeviceId == deviceId
                           select a;

            if (L2EQuery.Count() == 0)
            {
                _consoleLog.Warn("Device {0} doens't found on this IoT Hub", deviceId);
                _consoleLog.BlobLogWarn("Device {0} doens't found on this IoT Hub", deviceId);
                return false;           //Device Id doens't found on this IoT Hub
            }
            else
                simpleDeviceMsgCatalog = L2EQuery.FirstOrDefault<SimpleIoTDeviceMessageCatalog>();

            var L2EIdQuery = from b in simpleDeviceMsgCatalog.MessageCatalogIds
                             where b == msgCatalogId
                             select b;
            if (L2EIdQuery.Count() == 0)
            {
                _consoleLog.Warn("Message Catalog Id {0} doens't found on this IoT Device", msgCatalogId);
                _consoleLog.BlobLogWarn("Message Catalog Id {0} doens't found on this IoT Device", msgCatalogId);
                return false;           //Message Catalog Id doens't found on this IoT Device
            }
            else
                return true;
        }

        private void feedInMessage(string deviceId, int messageCatalogId, JObject feedInPayload)
        {
            try
            {
                JObject feedInData = new JObject();

                feedInData.Add("companyId", _companyId);
                feedInData.Add("iotDeviceId", deviceId);
                feedInData.Add("messageCatalogId", messageCatalogId);
                feedInData.Add("messageType", DocumentType.TelemetryDocument);
                feedInData.Add("messageContent", feedInPayload);

                //string result = _RTMessageSender.PostRealTimeMessage(feedInData.ToString());

                string result = webHelper.PostContent(_RTMessageFeedInURL, feedInData.ToString());

                //_consoleLog.Info("FeedIn Result:{0}", result);
            }
            catch (Exception ex)
            {
                _consoleLog.Error("feedInMessage Exception: deviceId={0}, messageCatalogId={1}, payload={2} , ex={3}", deviceId, messageCatalogId, feedInPayload, ex.Message);
                _consoleLog.BlobLogError("feedInMessage Exception: deviceId={0}, messageCatalogId={1}, payload={2} , ex={3}", deviceId, messageCatalogId, feedInPayload, ex.Message);
            }
        }

        private async Task<string> putMessage2CosmosDb(string deviceId, int messageCatalogId, JObject payload)
        {
            TelemetryDocument messageDocument = new TelemetryDocument
            {
                //Id = xxxx, // Marked the Id property will generate a random GUID automatically.
                companyId = _companyId,
                iotDeviceId = deviceId,
                messageCatalogId = messageCatalogId,
                messageType = DocumentType.TelemetryDocument,
                messageContent = payload
            };

            DocDBHelper cosmosDbHelper = _epfm.docDBHelper;
            Document document = await cosmosDbHelper.putDocumentAsync(messageDocument);

            //_consoleLog.Info("Message Document.Id={0}", document.Id);
            //_consoleLog.BlobLogInfo("Message Document.Id={0}", document.Id);
            return document.Id;
        }

        private async Task<string> putEvent2CosmosDb(string deviceId, DocDBHelper docDBHelper, EventMessageModel eventMessageModel)
        {
            EventDocument eventDocument = new EventDocument
            {
                companyId = _companyId,
                iotDeviceId = deviceId,
                messageCatalogId = eventMessageModel.MessageId,
                messageType = DocumentType.EventDocument,
                eventRuleCatalogId = eventMessageModel.EventRuleId,
                eventRuleCatalogName = eventMessageModel.EventRuleName,
                eventRuleCatalogDescription = eventMessageModel.EventRuleDescription,
                triggeredTime = eventMessageModel.TriggeredTime, // Machine Local Time
                eventSent = eventMessageModel.EventSent,
                messageDocumentId = eventMessageModel.MessageDocumentId,
                messageContent = eventMessageModel.Payload
            };

            Document document = await docDBHelper.putDocumentAsync(eventDocument);

            //_consoleLog.Info("Event Document.Id={0}", document.Id);

            return document.Id;
        }

        private async Task putMessage2BlobStorage(string messageDocumentId, string deviceId, JObject payload)
        {
            DateTime now = DateTime.UtcNow;
            string[] dateString = now.ToString("MM/dd/yyyy").Split('/');
            string blobName = dateString[2] + "/" + dateString[0] + "/" + dateString[1] + "/" + deviceId + "/" + messageDocumentId + ".json";
            await _epfm.TelemetryBlobStorageHelper.Save(blobName, JsonConvert.SerializeObject(payload));

            //_consoleLog.Info("putMessage2BlobStorage: {0}", blobName);
            //_consoleLog.BlobLogInfo("putMessage2BlobStorage: {0}", blobName);
        }

        private async Task putEvent2BlobStorage(string eventDocumentId, string deviceId, EventMessageModel eventMessageModel)
        {
            DateTime now = DateTime.UtcNow;
            string[] dateString = now.ToString("MM/dd/yyyy").Split('/');
            string blobName = dateString[2] + "/" + dateString[0] + "/" + dateString[1] + "/" + deviceId + "/" + eventDocumentId + ".json";

            var eventData = new
            {
                companyId = _companyId,
                iotDeviceId = deviceId,
                messageCatalogId = eventMessageModel.MessageId,
                messageType = DocumentType.EventDocument,
                eventRuleCatalogId = eventMessageModel.EventRuleId,
                eventRuleCatalogName = eventMessageModel.EventRuleName,
                eventRuleCatalogDescription = eventMessageModel.EventRuleDescription,
                triggeredTime = eventMessageModel.TriggeredTime, // Machine Local Time
                eventSent = eventMessageModel.EventSent,
                messageDocumentId = eventMessageModel.MessageDocumentId,
                eventDocumentId = eventDocumentId,
                messageContent = eventMessageModel.Payload
            };

            await _epfm.EventBlobStorageHelper.Save(blobName, JsonConvert.SerializeObject(eventData));

            //_consoleLog.Info("putEvent2BlobStorage: {0}", blobName);
            //_consoleLog.BlobLogInfo("putEvent2BlobStorage: {0}", blobName);
        }

        private async Task runEventRules(EventProcessorFactoryModel epfm, string deviceId, int messageCatalogId, JObject payload, string messageDocumentId)
        {
            try
            {
                var ts = payload["msgTimestamp"];
                DateTime msgTimestamp = DateTime.Parse(ts.ToString());

                Dictionary<int, List<EventRuleCatalogEngine>> eventRulesInMessageId = epfm.EventRulesInMessageId;
                if (eventRulesInMessageId.ContainsKey(messageCatalogId) == true)
                {
                    List<EventRuleCatalogEngine> eventRuleCatalogEngineList = eventRulesInMessageId[messageCatalogId];

                    foreach (EventRuleCatalogEngine eventRuleCatalogEngine in eventRuleCatalogEngineList)
                    {
                        if (eventRuleCatalogEngine.RuleEngineItems.Count > 0)
                        {
                            // Get all results of equations
                            foreach (KeyValuePair<string, RuleEngineItem> ruleEngineItem in eventRuleCatalogEngine.RuleEngineItems)
                            {
                                runSingleRuleItem(ruleEngineItem.Value, payload);
                            }

                            // Get the result of bitwise operation
                            bool eventTriggered = compileBitWiseRules(eventRuleCatalogEngine.RuleEngineItems.Count - 1, eventRuleCatalogEngine.RuleEngineItems);
                            if (eventTriggered)
                            {
                                string now = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                bool eventSent = checkEventTimeWindow(eventRuleCatalogEngine, msgTimestamp);

                                //_consoleLog.MessageEventDebug("EventRuleCatalogId={0}, eventSent={1}, LastTriggerTime={2}",
                                //    eventRuleCatalogEngine.EventRuleCatalogId, eventSent, eventRuleCatalogEngine.LastTriggerTime.ToString("yyyy-MM-ddTHH:mm:ss"));
                                //_consoleLog.BlobLogInfo("EventRuleCatalogId={0}, eventSent={1}, LastTriggerTime={2}",
                                //    eventRuleCatalogEngine.EventRuleCatalogId, eventSent, eventRuleCatalogEngine.LastTriggerTime.ToString("yyyy-MM-ddTHH:mm:ss"));

                                EventMessageModel eventMessageModel = new EventMessageModel(eventRuleCatalogEngine.EventRuleCatalog,
                                    now,
                                    eventSent,
                                    messageDocumentId,
                                    payload);

                                // Send the Event to Service Bus if it matchs the time window
                                if (eventSent)
                                    sendEventToServiceBusQueue(deviceId, epfm.EventQueueClient, eventMessageModel);

                                // Put the event to Cosmos DB (Hot Data)
                                string eventDocumentId = await putEvent2CosmosDb(deviceId, epfm.docDBHelper, eventMessageModel);

                                // Put the event to Blob (Cold Data)
                                await putEvent2BlobStorage(eventDocumentId, deviceId, eventMessageModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _consoleLog.Error("runEventRules Exception: {0}", ex.ToString());
                _consoleLog.BlobLogError("runEventRules Exception: {0}", ex.ToString());
                return;
            }
        }

        private void runSingleRuleItem(RuleEngineItem ruleEngineItem, JObject payload)
        {
            string elementName = ruleEngineItem.ElementName;

            try
            {
                var value = payload[elementName];

                DynamicMessageElement dm = new DynamicMessageElement();
                dm.Name = elementName;
                switch (ruleEngineItem.DataType)
                {
                    case SupportDataTypeEnum.Bool:
                        dm.StringValue = value.ToString().ToLower();
                        break;
                    case SupportDataTypeEnum.String:
                        dm.StringValue = (string)value;
                        if (string.IsNullOrEmpty(dm.StringValue))
                            dm.StringValue = "null";
                        break;
                    case SupportDataTypeEnum.Numeric:
                        dm.DecimalValue = (decimal)value;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                if (ruleEngineItem.DataType == SupportDataTypeEnum.Numeric || ruleEngineItem.DataType == SupportDataTypeEnum.Bool)
                    ruleEngineItem.Result = ruleEngineItem.Equality(dm);
                else
                {
                    // SupportDataTypeEnum.String
                    bool equal = string.Equals(dm.StringValue, ruleEngineItem.StringRightValue);
                    if (ruleEngineItem.StringEqualOperation.Equals("="))
                        ruleEngineItem.Result = equal;
                    else if (ruleEngineItem.StringEqualOperation.Equals("!="))
                        ruleEngineItem.Result = !equal;
                    else
                        throw new ArgumentNullException("String equal operation is not supported - " + ruleEngineItem.StringEqualOperation);
                }
            }
            catch (Exception)
            {
                ruleEngineItem.Result = false;
            }
        }

        private bool compileBitWiseRules(int offset, Dictionary<string, RuleEngineItem> ruleEngineItems)
        {
            RuleEngineItem rei = ruleEngineItems.ElementAt(offset).Value;

            if (offset == 0)
            {
                return rei.Result;
            }
            else
            {
                offset--;
                RuleEngineItem previousRei = ruleEngineItems.ElementAt(offset).Value;
                return AlarmRuleItemEngineUtility.ComplieBoolRule(rei.Result, previousRei.OrderOperation, compileBitWiseRules(offset, ruleEngineItems));
            }
        }

        private bool checkEventTimeWindow(EventRuleCatalogEngine ercEngine, DateTime msgTimestamp)
        {
            if (ercEngine.Triggered)
            {
                if (ercEngine.EventRuleCatalog.AggregateInSec <= 0)
                {
                    ercEngine.LastTriggerTime = msgTimestamp;
                    return true;// Always triggered
                }
                else
                {
                    DateTime nextAcceptableTime = ercEngine.LastTriggerTime.AddSeconds(ercEngine.EventRuleCatalog.AggregateInSec);
                    int result = DateTime.Compare(msgTimestamp, nextAcceptableTime);
                    if (result > 0)
                    {
                        // the message timestamp is later than the next accpetable time
                        ercEngine.LastTriggerTime = msgTimestamp;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                ercEngine.Triggered = true;
                ercEngine.LastTriggerTime = msgTimestamp;
                return true;
            }
        }

        private void sendEventToServiceBusQueue(string deviceId, QueueClient queueClient, EventMessageModel eventMessageModel)
        {
            try
            {
                var eventMessage = new
                {
                    companyId = _companyId,
                    iotDeviceId = deviceId,
                    messageCatalogId = eventMessageModel.MessageId,
                    messageType = DocumentType.EventDocument,
                    eventRuleCatalogId = eventMessageModel.EventRuleId,
                    eventRuleCatalogName = eventMessageModel.EventRuleName,
                    eventRuleCatalogDescription = eventMessageModel.EventRuleDescription,
                    triggeredTime = eventMessageModel.TriggeredTime,
                    messageDocumentId = eventMessageModel.MessageDocumentId,
                    messageContent = eventMessageModel.Payload
                };

                var messageString = JsonConvert.SerializeObject(eventMessage);
                var msg = new BrokeredMessage(messageString);
                queueClient.Send(msg);
            }
            catch (Exception ex)
            {
                _consoleLog.MessageEventError("sendEventToServiceBusQueue Exception: messageId={0}, EventRuleId={1}, triggeredTime={2} payload={3} ex={4}",
                    eventMessageModel.MessageId, eventMessageModel.EventRuleId, eventMessageModel.TriggeredTime, eventMessageModel.Payload, ex.ToString());
                _consoleLog.BlobLogError("sendEventToServiceBusQueue Exception: messageId={0}, EventRuleId={1}, triggeredTime={2} payload={3} ex={4}",
                    eventMessageModel.MessageId, eventMessageModel.EventRuleId, eventMessageModel.TriggeredTime, eventMessageModel.Payload, ex.ToString());
            }
        }

        private int getMessageCatalogIdFromTransformer(string deviceId)
        {
            return _epfm.MessageTransformerInDeviceId[deviceId].MessageCatalogID;
        }

        private string getJsonStringFromTransformer(string deviceId)
        {
            return _epfm.MessageTransformerInDeviceId[deviceId].TransformJson;
        }

        private bool checkMandatoyProperties(JObject data)
        {
            JToken companyId, msgTimestamp, equipmentId, equipmentRunStatus;

            if (data.TryGetValue("companyId", out companyId) &&
                data.TryGetValue("msgTimestamp", out msgTimestamp) &&
                data.TryGetValue("equipmentId", out equipmentId) &&
                data.TryGetValue("equipmentRunStatus", out equipmentRunStatus))
            {
                string ts = msgTimestamp.Value<string>();

                if (companyId.Value<string>().Equals(CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND) ||
                    ts.Equals(CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND) ||
                    equipmentId.Value<string>().Equals(CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND) ||
                    equipmentRunStatus.Value<string>().Equals(CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND))
                    return false;
                else
                {
                    // Check Timestamp format
                    DateTime dDate;
                    if (DateTime.TryParse(ts, out dDate))
                        return true;
                    else
                    {
                        _consoleLog.Warn("Failed to parse msgTimestamp to datetime format.");
                        _consoleLog.BlobLogWarn("Failed to parse msgTimestamp to datetime format.");
                        return false;
                    }
                }
            }
            else
                return false;
        }

        private void checkpointToRedisCache()
        {
            try
            {
                setEventProcessingEnabled(checkAndUpdateConsumedMessageCache(DateTime.UtcNow, true));
                setCurrentMsgCountBySizeInBytes(0);
                /* Update it to Heartbeat */
                Heartbeat.updateConsumedMessage(_messageQuotaPerDay, _messageConsumedCount, _messageConsumedDate);
            }
            catch (Exception ex)
            {
                _consoleLog.Error("checkpointToRedisCache Exception:{0}", ex.Message.ToString());
                _consoleLog.BlobLogError("checkpointToRedisCache Exception:{0}", ex.Message.ToString());
            }
        }

        private bool checkAndUpdateConsumedMessageCache(DateTime utcNow, bool updateCache)
        {
            bool enabled;
            if (isNewTimePeriod(utcNow))
            {
                _consoleLog.Info("isNewTimePeriod utcNow:{0}", utcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                _consoleLog.BlobLogInfo("isNewTimePeriod utcNow:{0}", utcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff"));

                if (updateCache)
                {
                    /* Reset */
                    updateCacheConsumedMessage(getCurrentMsgCountBySizeInBytes(), utcNow);
                }

                enabled = true;// New Day
            }
            else
            {
                long cacheConsumedCount = getCacheConsumedCount();
                _messageQuotaPerDay = getCacheQuotaPerDay();
                if (isCacheQuotaPerDayValid(_messageQuotaPerDay))
                {
                    if (isCacheQuotasPerDayOver(cacheConsumedCount, _messageQuotaPerDay))
                        enabled = false;/* Over the quota */
                    else
                    {
                        if (updateCache)
                        {
                            long totalCount = getCurrentMsgCountBySizeInBytes() + cacheConsumedCount;
                            updateCacheConsumedMessage(totalCount, utcNow);
                            enabled = isCacheQuotasPerDayOver(totalCount, _messageQuotaPerDay) == false;
                        }
                        else
                            enabled = true;
                    }
                }
                else
                {
                    // No quota limitation
                    if (updateCache)
                    {
                        long totalCount = getCurrentMsgCountBySizeInBytes() + cacheConsumedCount;
                        updateCacheConsumedMessage(totalCount, utcNow);
                    }
                    enabled = true;
                }
            }

            return enabled;
        }

        private bool isNewTimePeriod(DateTime utcNow)
        {
            /* The last time of consumed message */
            
            string lastTimestampOfConsumedMsg = _redisCacheHelper.GetValueByKey(_cachekey_message_consumed_date);
            if (lastTimestampOfConsumedMsg == null)
            {
                _consoleLog.Info("Redis Cache {0} was NOT found", _cachekey_message_consumed_date.ToString());
                _consoleLog.BlobLogInfo("Redis Cache {0} was NOT found", _cachekey_message_consumed_date.ToString());
                // THIS IS A NEW DAY!
                return true;
            }
            else
            {
                DateTime lastTimestamp = DateTime.Parse((string)lastTimestampOfConsumedMsg);
                /* The period is 1 day */
                DateTime timeup = DateTime.Parse(lastTimestamp.AddDays(1).ToString("yyyy-MM-ddT00:00:00.000"));
                /* This is a sample for that the period is 1 minute */
                //DateTime timeup = DateTime.Parse(lastTimestamp.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:00.000"));

                if (DateTime.Compare(utcNow, timeup) >= 0)
                {
                    // THIS IS A NEW DAY!
                    return true;
                }
                else
                    return false;
            }
        }

        private void updateCacheConsumedMessage(long count, DateTime utcNow)
        {
            _messageConsumedCount = count;
            _redisCacheHelper.SetKeyValue(_cachekey_message_total_consumed_count, count.ToString());
            //_consoleLog.Info("Redis Cache {0} is {1}", _cachekey_message_total_consumed_count.ToString(), count);

            _messageConsumedDate = utcNow;
            _redisCacheHelper.SetKeyValue(_cachekey_message_consumed_date, utcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            //_consoleLog.Info("Redis Cache {0} was updated {1}", _cachekey_message_consumed_date.ToString(), utcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
        }

        private void setCurrentMsgCountBySizeInBytes(long count)
        {
            _currentMsgCountBySizeInBytes = count;
        }

        private long getCurrentMsgCountBySizeInBytes()
        {
            return _currentMsgCountBySizeInBytes;
        }

        private void updateCurrentMsgCountBySizeInBytes(long sizeInBytes)
        {
            /* Get the message count including message payload and properties */
            setCurrentMsgCountBySizeInBytes(getCurrentMsgCountBySizeInBytes() + calculateMessageCount(sizeInBytes, _msgSizeInBytes));
        }

        private long calculateMessageCount(long sizeInBytes, long msgSizeInBytes)
        {
            long msgCount = sizeInBytes / msgSizeInBytes;
            long remaining = sizeInBytes % msgSizeInBytes != 0 ? 1 : 0;

            msgCount += remaining;

            //_consoleLog.Info("sizeInBytes:{0}, msgCount:{1}", sizeInBytes, msgCount);

            return msgCount;
        }

        private long getCacheConsumedCount()
        {
            string consumedCount = _redisCacheHelper.GetValueByKey(_cachekey_message_total_consumed_count);
            return consumedCount == null ? 0 : long.Parse(consumedCount);
        }

        private bool isCacheQuotaPerDayValid(long cacheQuotaPerDay)
        {
            return cacheQuotaPerDay == -1 ? false : true;
        }

        private bool isCacheQuotasPerDayOver(long totalCount, long quotaPerDay)
        {
            return totalCount >= quotaPerDay ? true : false;
        }

        private long getCacheQuotaPerDay()
        {
            /* Quotas */
            string s = _redisCacheHelper.GetValueByKey(_cachekey_message_quota_per_day);
            return s == null ? -1 : long.Parse(s);
        }

        private void setEventProcessingEnabled(bool enabled)
        {
            _eventProcessingEnabled = enabled;
        }

        private bool eventProcessingEnabled()
        {
            return _eventProcessingEnabled;
        }

        private bool hasExpiredDate()
        {
            return DateTime.UtcNow > _epfm.CdsInfo.CompanyInSubscriptionPlan.ExpiredDate;
        }

        private string getDeviceId(EventData eventData)
        {
            string key = "iothub-connection-device-id";
            object pulledObject = null;
            if (eventData.SystemProperties.TryGetValue(key, out pulledObject))
                return pulledObject.ToString();
            else
            {
                _consoleLog.BlobLogError("System Properties was NOT found: {0} - SequenceNumber={1}", key, eventData.SequenceNumber);
                _consoleLog.Error("System Properties was NOT found: {0} - SequenceNumber={1}", key, eventData.SequenceNumber);
                return null;
            }
        }

        private bool isTwinChangedEvent(EventData eventData)
        {
            bool bTwinChanged = false;
            try
            {
                if ("twinChangeEvents".Equals(getMessageSystemProperty(eventData, "iothub-message-source")))
                    bTwinChanged = true;
            }
            catch
            {
                bTwinChanged = false;
            }

            return bTwinChanged;
        }

        private string getMessageSystemProperty(EventData eventData, string propertyName)
        {
            object pulledObject = null;
            if (eventData.SystemProperties.TryGetValue(propertyName, out pulledObject))
                return pulledObject.ToString();
            else
            {
                //_consoleLog.BlobLogError("System Properties was NOT found: {0} - SequenceNumber={1}", propertyName, eventData.SequenceNumber);
                _consoleLog.Error("System Properties was NOT found: {0} - SequenceNumber={1}", propertyName, eventData.SequenceNumber);
                return null;
            }
        }

        private string getMessageProperty(EventData eventData, string propertyName)
        {
            object pulledObject = null;
            if (eventData.Properties.TryGetValue(propertyName, out pulledObject))
                return pulledObject.ToString();
            else
            {
                //_consoleLog.BlobLogError("Properties was NOT found: {0} - SequenceNumber={1}", propertyName, eventData.SequenceNumber);
                _consoleLog.Error("Properties was NOT found: {0} - SequenceNumber={1}", propertyName, eventData.SequenceNumber);
                return null;
            }
        }

        private void sendCommandToServiceBusQueue(EventProcessorFactoryModel epfm, string deviceId, JObject command)
        {
            QueueClient queueClient = epfm.InfraQueueClient;
            string iothubAlias = epfm.CdsInfo.IoTHubAlias;

            try
            {
                JObject msgObj = new JObject();
                msgObj.Add("job", "device management");
                msgObj.Add("entity", "iotdevice");
                msgObj.Add("entityId", deviceId);
                msgObj.Add("task", "update device reported property to db");
                msgObj.Add("iothubDeviceId", deviceId);
                msgObj.Add("requester", "IoTHubReceiver_" + iothubAlias);
                msgObj.Add("requestDateTime", DateTime.UtcNow);
                msgObj.Add("iothubAlias", iothubAlias);
                msgObj.Add("iothubIsPrimary", true);

                JObject payload = new JObject();
                payload.Add(SF_LASTUPDATED_TIMESTAMP, command[SF_LASTUPDATED_TIMESTAMP]);
                msgObj.Add("deviceConfiguration", payload);

                var messageString = JsonConvert.SerializeObject(msgObj);
                var msg = new BrokeredMessage(messageString);
                queueClient.Send(msg);
            }
            catch (Exception ex)
            {
                _consoleLog.MessageEventError("sendCommandToServiceBusQueue Exception: payload={0} ex={1}", command, ex.ToString());
                _consoleLog.BlobLogError("sendCommandToServiceBusQueue Exception:  payload={0} ex={1}", command, ex.ToString());
            }
        }

        private int getMessageCatalogIdInMessageProperty(EventData eventData)
        {
            try
            {
                return Int32.Parse(getMessageProperty(eventData, MESSAGE_PROPERTY_MESSAGECATALOGID));
            }
            catch
            {
                var deviceID = getDeviceId(eventData);
                _consoleLog.Warn("MessageCatalogId Property was not found! Device ID: {0}", deviceID);
                _consoleLog.BlobLogWarn("MessageCatalogId Property was not found! Device ID: {0}", deviceID);
                throw;
            }
        }
    }

    class IoTHubMessageProcessorFactory : IEventProcessorFactory
    {
        private EventProcessorFactoryModel epfm;

        public IoTHubMessageProcessorFactory(EventProcessorFactoryModel epfm)
        {
            this.epfm = epfm;

        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            if(epfm.CdsInfo.PartitionNum.ToLower().Equals("all"))
            {
                return new IoTHubMessageEventProcessor(epfm);
            }
            else if (context.Lease.PartitionId.Equals(epfm.CdsInfo.PartitionNum))
            {
                return new IoTHubMessageEventProcessor(epfm);
            }
            else
                return null;
        }
    }
}
