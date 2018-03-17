using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus;
using SMSApp.Model;
using System.Text;
using Newtonsoft.Json.Linq;
using CDSShareLib.Helper;

namespace SMSApp
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class SMSApp : StatelessService
    {
        public static LogHelper _appLogger = null;
        public static String _sbConnectionString, _sbSMSQueue;
        public static QueueClient _sbQueueClient = null;
        public static bool _isRunning = false;
        public static int _incomeMessage, _processedMessage, _failMessage, _ignoreMessage;
        static string _twilioAccountId, _twilioToken, _twilioPhoneNumber;

        public SMSApp(StatelessServiceContext context)
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
                String logStorageContainer = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SMSAppLogStorageContainerApp");
                LogLevel logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SMSAppLogLevel"));
                _appLogger = new LogHelper(logStorageConnectionString, logStorageContainer, logLevel);
                _appLogger.Info("Initial Program Start...");
                _sbConnectionString = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ServiceBusConnectionString");
                _sbSMSQueue = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SMSQueue");
                _twilioAccountId = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SMSTwilioAccountId");
                _twilioToken = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SMSTwillioToken");
                _twilioPhoneNumber = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SMSTwilloPhoneNumber");

                Heartbeat._superadminHeartbeatURL = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SuperAdminHeartbeatURL");
                Heartbeat._sbNameSpaceMgr = NamespaceManager.CreateFromConnectionString(_sbConnectionString);
                Heartbeat._heartbeatIntervalinSec = int.Parse(AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SMSAppHeartbeatIntervalInSec"));
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
            _sbQueueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _sbSMSQueue);
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
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("SMS Task onMessage: " + messageBody);
                    _incomeMessage++;
                    JObject jsonMessage = JObject.Parse(messageBody);

                    if (jsonMessage["taskId"] == null || jsonMessage["task"] == null)
                    {
                        _appLogger.Warn("Incomplete message:" + messageBody);
                        _ignoreMessage++;
                    }
                    else
                    {
                        taskId = int.Parse(jsonMessage["taskId"].ToString());
                        task = jsonMessage["task"].ToString().ToLower();
                        _appLogger.Info("Received task: " + task);
                        if (task == "send sms")
                        {
                            string smsProvider = jsonMessage["smsProvider"].ToString().ToLower();
                            switch (smsProvider)
                            {
                                case "twilio":
                                    TwilioThread twillo = new TwilioThread(_twilioAccountId, _twilioToken, _twilioPhoneNumber, jsonMessage, taskId);
                                    Thread twilloThread = new Thread(new ThreadStart(twillo.ThreadProc));
                                    twilloThread.IsBackground = false;
                                    twilloThread.Start();
                                    break;
                            }
                            _processedMessage++;
                        }
                        else
                        {
                            _ignoreMessage++;
                        }
                    }
                    message.Complete();
                }
                catch (Exception ex)
                {
                    // Indicates a problem, unlock message in subscription.
                    _failMessage++;
                    message.Complete();
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("SMS Task Exception: " + ex.Message);
                    logMessage.AppendLine("SMS Task Message: " + messageBody);
                    _appLogger.Error(logMessage);
                    AzureSQLHelper.OperationTaskModel operationTask = new AzureSQLHelper.OperationTaskModel();
                    operationTask.UpdateTaskByFail(taskId, ex.Message);
                }
            }, options);
        }
    }
}
