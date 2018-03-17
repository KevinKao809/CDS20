using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceBus.Messaging;
using EventActionApp.Model;
using Microsoft.ServiceBus;
using Newtonsoft.Json.Linq;
using System.Text;
using CDSShareLib.Helper;

namespace EventActionApp
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class EventActionApp : StatelessService
    {
        public static LogHelper _appLogger = null;
        public static String _sbConnectionString, _sbEventActionQueue;
        public static String _sbEmailQueue, _sbSMSQueue, _sbHTTPQueue;
        public static String _rtEventFeedInURL;
        public static QueueClient _sbQueueClient = null;
        public static bool _isRunning = false;
        public static int _incomeMessage, _processedMessage, _failMessage, _ignoreMessage;

        public EventActionApp(StatelessServiceContext context)
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
                String logStorageContainer = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EventActionLogStorageContainerApp");
                LogLevel logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EventActionLogLevel"));
                _appLogger = new LogHelper(logStorageConnectionString, logStorageContainer, logLevel);
                _appLogger.Info("Initial Program Start...");
                _sbConnectionString = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ServiceBusConnectionString");
                _sbEventActionQueue = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EventActionQueue");
                _sbEmailQueue = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EmailQueue");
                _sbSMSQueue = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SMSQueue");
                _sbHTTPQueue = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("HTTPQueue");
                _rtEventFeedInURL = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("RTEventFeedInURL");

                Heartbeat._superadminHeartbeatURL = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SuperAdminHeartbeatURL");
                Heartbeat._sbNameSpaceMgr = NamespaceManager.CreateFromConnectionString(_sbConnectionString);
                Heartbeat._heartbeatIntervalinSec = int.Parse(AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EventActionAppHeartbeatIntervalinSec"));
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
            _sbQueueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _sbEventActionQueue);
            OnMessageOptions options = new OnMessageOptions();
            options.MaxConcurrentCalls = 1;
            options.AutoComplete = true;
            string messageBody = "";
            _isRunning = true;

            _sbQueueClient.OnMessage((message) =>
            {
                try
                {
                    // Process message from queue.
                    messageBody = message.GetBody<string>();
                    _appLogger.Info("EventAction Task onMessage: " + messageBody);
                    _incomeMessage++;
                    JObject jsonMessage = JObject.Parse(messageBody);
                    
                    EventThread eventAction = new EventThread(jsonMessage);
                    Thread eventActionThread = new Thread(new ThreadStart(eventAction.ThreadProc));
                    eventActionThread.IsBackground = false;
                    eventActionThread.Start();                    
                    _processedMessage++;
                }
                catch (Exception ex)
                {
                    // Indicates a problem, unlock message in subscription.
                    _failMessage++;                    
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("EventAction Task Exception: " + ex.Message);
                    logMessage.AppendLine("EventAction Task Message: " + messageBody);
                    _appLogger.Error(logMessage);
                }
            }, options);
        }
    }
}
