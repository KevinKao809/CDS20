using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IoTHubReceiver.Model;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json.Linq;
using IoTHubReceiver.Utilities;
using CDSShareLib.Helper;
using CDSShareLib.ServiceBus.Model;
using Newtonsoft.Json;
using CDSShareLib.ServiceBus;

namespace IoTHubReceiver
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class IoTHubReceiver : StatelessService
    {
        public static bool _ignoreCDS20DB = false;
        public static ConsoleLog _consoleLog;
        private CdsInfo _cdsInfo;
        private static IoTHubMessageReceiver _IoTHubMessageReceiver;
        static SubscriptionClient _sbSubscriptionClient;

        public IoTHubReceiver(StatelessServiceContext context)
            : base(context)
        {
            _cdsInfo = loadEnvironmentVariables();

            _cdsInfo.cdsBackendSetting = loadCdsBackendSettingFromDB();

            _consoleLog = setupConsoleLog(_cdsInfo);

            _consoleLog.Info("CompandId: " + _cdsInfo.CompanyId + ", IoTHubId: " + _cdsInfo.IoTHubId + ", PartitionNumber: " + _cdsInfo.PartitionNum + ", Label: " + _cdsInfo.Label);
            _consoleLog.BlobLogInfo("CompandId: " + _cdsInfo.CompanyId + ", IoTHubId: " + _cdsInfo.IoTHubId + ", PartitionNumber: " + _cdsInfo.PartitionNum + ", Label: " + _cdsInfo.Label);
        }

        private ConsoleLog setupConsoleLog(CdsInfo cdsInfo)
        {
            CdsBackendSetting cdsBackendSetting = cdsInfo.cdsBackendSetting;

            return new ConsoleLog(this.Context,
                new LogHelper(
                    cdsBackendSetting.LogStorageConnectionString,
                    cdsBackendSetting.LogStorageContainer,
                    cdsBackendSetting.LogLevel),
                cdsInfo);
        }

        private CdsInfo loadEnvironmentVariables()
        {
            CdsInfo cdsInfo = new CdsInfo();
            string companyId = Environment.GetEnvironmentVariable("input_CompanyId");
            string iotHubId = Environment.GetEnvironmentVariable("input_IoTHubId");
            string ioTHubPartitionNumber = Environment.GetEnvironmentVariable("input_Partition");
            string label = Environment.GetEnvironmentVariable("input_Label");

            if (string.IsNullOrEmpty(companyId))
            {
                throw new ArgumentException("Can't find CompandId from SystemEnvironment");
            }

            if (string.IsNullOrEmpty(iotHubId))
            {
                throw new ArgumentException("Can't find IoTHubId from SystemEnvironment");
            }

            if (string.IsNullOrEmpty(ioTHubPartitionNumber))
            {
                throw new ArgumentException("Can't find IoTHubPartitionNumber from SystemEnvironment");
            }

            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentException("Can't find Label from SystemEnvironment");
            }

            cdsInfo.CompanyId = companyId;
            cdsInfo.IoTHubId = iotHubId;
            cdsInfo.PartitionNum = ioTHubPartitionNumber;
            cdsInfo.Label = label;

            return cdsInfo;
        }

        private CdsBackendSetting loadCdsBackendSettingFromDB()
        {
            CdsBackendSetting cdsBackendSetting = new CdsBackendSetting();
            try
            {
                if (_ignoreCDS20DB)
                {
                    cdsBackendSetting.LogStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=cds20systemstoragedev;AccountKey=64SCgh/hP9PjCCY6YljAcf4E/kCCh58pUoscfzxnUYfNSwRw8ZYLgN+5wVgWUC5njqax6A1c1BK/GT45hNEwWQ==;EndpointSuffix=core.windows.net"; // "sfwebadminstorage"
                    cdsBackendSetting.LogStorageContainer = "log-backend-iothubeventprocessor";
                    cdsBackendSetting.LogLevel = LogLevel.Info;

                    cdsBackendSetting.ServiceBusConnectionString = "Endpoint=sb://sfservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=pO5mNKGGhR06BPKXhf8Dk7chLXm7TfjoQjF+8Zvw5XY=";
                    cdsBackendSetting.ServiceBusEventActionQueue = "alarmops";
                    cdsBackendSetting.ServiceBusProvisionQueue = "infraops";
                    cdsBackendSetting.ServiceBusProcessCommandTopic = "processcommand";
                    cdsBackendSetting.RTMessageFeedInURL = "http://admin.dev.iot-cds.net/Monitor/RTMessageFeedIn";

                    cdsBackendSetting.SuperAdminHeartbeatURL = "http://superadmin.dev.iot-cds.net/Monitor/RTMessageFeedIn";
                    cdsBackendSetting.AdminHeartbeatURL = "http://admin.dev.iot-cds.net/Monitor/RTMessageFeedIn";
                }
                else
                {
                    /* Log */
                    cdsBackendSetting.LogStorageConnectionString = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SystemStorageConnectionString");
                    cdsBackendSetting.LogStorageContainer = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("IoTHubReceiverLogStorageContainerApp");
                    cdsBackendSetting.LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("IoTHubReceiverLogLevel"));

                    /* Service Bus */
                    cdsBackendSetting.ServiceBusConnectionString = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ServiceBusConnectionString");
                    cdsBackendSetting.ServiceBusEventActionQueue = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EventActionQueue");
                    cdsBackendSetting.ServiceBusProvisionQueue = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ProvisionQueue");
                    cdsBackendSetting.ServiceBusProcessCommandTopic = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("IoTHubReceiverTopic");

                    /* Feed In URL */
                    cdsBackendSetting.RTMessageFeedInURL = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("RTMessageFeedInURL");

                    /* Heartbeat */
                    cdsBackendSetting.SuperAdminHeartbeatURL = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SuperAdminHeartbeatURL");
                    cdsBackendSetting.AdminHeartbeatURL = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("AdminHeartbeatURL");
                }

                cdsBackendSetting.HeartbeatIntervalInSec = getHeartbeatIntervalInSec();
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot load the backend setting from database: " + ex.Message);
            }

            return cdsBackendSetting;
        }

        private int getHeartbeatIntervalInSec()
        {
            if (_ignoreCDS20DB)
            {
                return 10;
            }
            else
            {
                return int.Parse(AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("IoTHubReceiverHeartbeatIntervalInSec"));
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            /***** Start here *****/
            try
            {
                Heartbeat.Start(_cdsInfo);

                /* Main IoTHub Receiver Here */
                _IoTHubMessageReceiver = new IoTHubMessageReceiver(_cdsInfo);
                await _IoTHubMessageReceiver.Start();

                ListenOnServiceBusTopic(_cdsInfo);
            }
            catch (Exception ex)
            {
                _consoleLog.Error("RunAsync Exception:{0}", ex.Message);
                _consoleLog.BlobLogError("RunAsync Exception:{0}", ex.Message);

                if (_IoTHubMessageReceiver != null)
                    _IoTHubMessageReceiver.Stop().Wait();

                //throw ex;
            }
        }

        protected override async Task OnCloseAsync(CancellationToken cancellationToken)
        {
            _consoleLog.Info("OnCloseAsync");
            _consoleLog.BlobLogInfo("OnCloseAsync");

            if (_IoTHubMessageReceiver != null)
                _IoTHubMessageReceiver.Stop().Wait();

            await base.OnCloseAsync(cancellationToken);
        }

        private void ListenOnServiceBusTopic(CdsInfo cdsInfo)
        {
            CdsBackendSetting azureCS = cdsInfo.cdsBackendSetting;
            /* Create Topic Subscription Client, and bind with Message Property on companyid = xx */
            var namespaceManager = NamespaceManager.CreateFromConnectionString(azureCS.ServiceBusConnectionString);
            string subscriptionName = "C_" + cdsInfo.CompanyId + "_IoTHubId_" + cdsInfo.IoTHubId + "_P_" + cdsInfo.PartitionNum;
            SqlFilter messageFilter = new SqlFilter("Process = 'IoTHubReceiver' AND IoTHubId = '" + cdsInfo.IoTHubId + "'");

            /* If the subscription not exist, create it. */
            if (!namespaceManager.SubscriptionExists(azureCS.ServiceBusProcessCommandTopic, subscriptionName))
                namespaceManager.CreateSubscription(azureCS.ServiceBusProcessCommandTopic, subscriptionName, messageFilter);

            /* Create subscription client and listen on message  */
            _sbSubscriptionClient = SubscriptionClient.CreateFromConnectionString(azureCS.ServiceBusConnectionString, azureCS.ServiceBusProcessCommandTopic, subscriptionName);
            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = true;

            IoTHubReceiverModel _IoTHubReceiverMsg = null;

            _sbSubscriptionClient.OnMessage(async (message) =>
            {
                AzureSQLHelper.OperationTaskModel operationTask = new AzureSQLHelper.OperationTaskModel();

                try
                {
                    string messagePayload = message.GetBody<string>();
                    _IoTHubReceiverMsg = JsonConvert.DeserializeObject<IoTHubReceiverModel>(messagePayload);

                    // Process message from subscription.                    
                    _consoleLog.Info("onMessage: {0}", messagePayload);
                    _consoleLog.BlobLogInfo("onMessage: {0}", messagePayload);
                    
                    _consoleLog.Info("Received Task:" + _IoTHubReceiverMsg.task);
                    _consoleLog.BlobLogInfo("Received Task:" + _IoTHubReceiverMsg.task);

                    switch (_IoTHubReceiverMsg.task)
                    {
                        //case "start":
                        //    await _IoTHubMessageReceiver.Start();
                        //    break;
                        //case "stop":
                        //    await _IoTHubMessageReceiver.Stop();
                        //    break;
                        case TaskName.IoTHubReceiver_Restart:
                            reloadHeartbeatInterval();
                            _IoTHubMessageReceiver.Stop().Wait();
                            _IoTHubMessageReceiver.Start().Wait();
                            break;
                        //case "shutdown":
                        //    message.Complete();
                        //    await _IoTHubMessageReceiver.Stop();
                        //    operationTask.UpdateTaskBySuccess(_IoTHubReceiverMsg.taskId);
                        //    Environment.Exit(0);
                        //    break;
                    }
                    operationTask.UpdateTaskBySuccess(_IoTHubReceiverMsg.taskId);
                }
                catch (Exception ex)
                {
                    // Indicates a problem, unlock message in subscription.
                    _consoleLog.Error("Exception: {0}", ex.Message);
                    _consoleLog.BlobLogError("Exception: {0}", ex.Message);
                    operationTask.UpdateTaskByFail(_IoTHubReceiverMsg.taskId, ex.Message);
                }
            }, options);
        }

        private void reloadHeartbeatInterval()
        {
            _cdsInfo.cdsBackendSetting.HeartbeatIntervalInSec = getHeartbeatIntervalInSec();
            Heartbeat.UpdateHBInterval(_cdsInfo.cdsBackendSetting.HeartbeatIntervalInSec);
        }
    }
}
