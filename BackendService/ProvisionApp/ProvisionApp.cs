using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceBus.Messaging;
using ProvisionApp.Model;
using Microsoft.ServiceBus;
using System.Text;
using Newtonsoft.Json.Linq;
using CDSShareLib.Helper;
using CDSShareLib.ServiceBus;

namespace ProvisionApp
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProvisionApp : StatelessService
    {
        public static LogHelper _appLogger = null;
        public static String _sbConnectionString, _sbProvisionQueue;
        public static QueueClient _sbQueueClient = null;
        public static bool _isRunning = false;
        public static int _incomeMessage, _processedMessage, _failMessage, _ignoreMessage;

        public ProvisionApp(StatelessServiceContext context)
            : base(context)
        { }

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
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            /***** Start here *****/
            try
            {
                InitialProgram();
                Heartbeat.Start();
                ListenOnServiceBusQueue();
            }
            catch (Exception)
            {
            }
        }

        void InitialProgram()
        {
            try
            {
                String logStorageConnectionString = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SystemStorageConnectionString");
                String logStorageContainer = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ProvisionLogStorageContainerApp");
                LogLevel logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ProvisionLogLevel"));
                _appLogger = new LogHelper(logStorageConnectionString, logStorageContainer, logLevel);
                _appLogger.Info("Initial Program Start...");
                _sbConnectionString = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ServiceBusConnectionString");
                _sbProvisionQueue = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ProvisionQueue");

                Heartbeat._superadminHeartbeatURL = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SuperAdminHeartbeatURL");
                Heartbeat._sbNameSpaceMgr = NamespaceManager.CreateFromConnectionString(_sbConnectionString);
                Heartbeat._heartbeatIntervalinSec = int.Parse(AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ProvisionAppHeartbeatIntervalinSec"));
                _appLogger.Info("Initial Program Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception on Initial Program. Error:" + ex.Message);
            }
        }

        void ListenOnServiceBusQueue()
        {
            /* Create Queue client and listen on message  */
            _sbQueueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _sbProvisionQueue);
            OnMessageOptions options = new OnMessageOptions();
            options.MaxConcurrentCalls = 1;
            options.AutoComplete = false;
            string messageBody = "";
            _isRunning = true;

            _sbQueueClient.OnMessage((message) =>
            {
                string task;
                int taskId = 0;
                try
                {
                    // Process message from queue.
                    messageBody = message.GetBody<string>();
                    _appLogger.Info("Provision Task onMessage: " + messageBody);
                    _incomeMessage++;
                    JObject jsonMessage = JObject.Parse(messageBody);

                    if (jsonMessage["taskId"] == null || jsonMessage["task"] == null)
                    {
                        _appLogger.Warn("Incomplete message:" + messageBody);
                        _ignoreMessage++;
                        message.Complete();
                    }
                    else
                    {
                        taskId = int.Parse(jsonMessage["taskId"].ToString());
                        task = jsonMessage["task"].ToString().ToLower();
                        _appLogger.Info("Received task: " + task);
                        switch (task)
                        {
                            case TaskName.CosmosdbCollection_Create:
                            case TaskName.CosmosdbCollection_Delete:
                            case TaskName.CosmosdbCollection_Update:
                                string CosmosDBConnectionString = jsonMessage["cosmosDBConnectionString"].ToString();
                                if (String.IsNullOrEmpty(CosmosDBConnectionString))
                                    CosmosDBConnectionString = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("CosmosDBConnectionString");

                                DocumentDBThread docDB = new DocumentDBThread(CosmosDBConnectionString, jsonMessage, task, taskId);
                                Thread docDBthread = new Thread(new ThreadStart(docDB.ThreadProc));
                                docDBthread.IsBackground = false;
                                docDBthread.Start();
                                message.Complete();
                                break;
                            case TaskName.IoTDevice_Register:
                            case TaskName.IoTDevice_Delete:
                                {
                                    string IoTHubConnectionString = jsonMessage["Content"]["IothubConnectionString"].ToString();
                                    if (String.IsNullOrEmpty(IoTHubConnectionString))
                                    {
                                        _appLogger.Warn("IoT Hub Connection is NULL.");
                                        message.Complete();
                                    }
                                    else
                                    {
                                        IoTHubDeviceRegisterThread iotDeviceRegister = new IoTHubDeviceRegisterThread(IoTHubConnectionString, jsonMessage, task, taskId);
                                        Thread iotDeviceRegisterThread = new Thread(new ThreadStart(iotDeviceRegister.ThreadProc));
                                        iotDeviceRegisterThread.IsBackground = false;
                                        iotDeviceRegisterThread.Start();
                                        message.Complete();
                                    }
                                }
                                break;
                            case TaskName.IoTHubReceiver_Launch:
                            case TaskName.IoTHubReceiver_Shutdown:
                                message.Complete();         //The following task may take longer, let message complete first
                                IoTHubReceiverThread iotHubReceiver = new IoTHubReceiverThread(messageBody);
                                Thread iotHubReceiverThread = new Thread(new ThreadStart(iotHubReceiver.ThreadProc));
                                iotHubReceiverThread.IsBackground = false;
                                iotHubReceiverThread.Start();
                                break;                            
                        }
                        _processedMessage++;
                    }
                }
                catch (Exception ex)
                {
                    // Indicates a problem, unlock message in subscription.
                    _failMessage++;
                    message.Complete();
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("Provision Task Exception: " + ex.Message);
                    logMessage.AppendLine("Provision Task Message: " + messageBody);
                    _appLogger.Error(logMessage);
                    AzureSQLHelper.OperationTaskModel operationTask = new AzureSQLHelper.OperationTaskModel();
                    operationTask.UpdateTaskByFail(taskId, ex.Message);
                }
            }, options);
        }
    }
}
