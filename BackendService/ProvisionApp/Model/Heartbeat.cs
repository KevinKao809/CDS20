using CDSShareLib.Helper;
using Microsoft.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Text;
using System.Threading;

namespace ProvisionApp.Model
{
    public class Heartbeat
    {
        public static String _superadminHeartbeatURL;
        public static NamespaceManager _sbNameSpaceMgr = null;
        public static int _heartbeatIntervalinSec = 10;      //Set with Default, it will be overwrite on initial progrma from DB value
        static Timer _HBTimer = null;
        static int _ProcessId = Process.GetCurrentProcess().Id;
        static PerformanceCounter _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        static PerformanceCounter _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        static WebHelper webHelper = new WebHelper();

        public static void Start()
        {
            _HBTimer = new Timer(new TimerCallback(PushHeartbeatSignal));
            _HBTimer.Change(_heartbeatIntervalinSec*1000, _heartbeatIntervalinSec*1000);
            ProvisionApp._appLogger.Debug("Heartbeat Interval: " + _heartbeatIntervalinSec + " seconds");
        }

        private static void PushHeartbeatSignal(object state)
        {
            string jsonHB = GetHeartbeatStatus();
            try
            {
                webHelper.PostContent(_superadminHeartbeatURL, jsonHB);
                ProvisionApp._appLogger.Debug(string.Format("Push Heartbeat: {0}, {1}", _superadminHeartbeatURL, jsonHB));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("Provision Task Exception on send Heartbeat: " + ex.Message);
                ProvisionApp._appLogger.Error(logMessage);
            }
        }

        private static string GetHeartbeatStatus()
        {
            dynamic HeartbeatMessage = new ExpandoObject();
            HeartbeatMessage.topic = "Process Heartbeat";
            HeartbeatMessage.name = "Provision Task";
            HeartbeatMessage.machine = Environment.MachineName;
            HeartbeatMessage.processId = _ProcessId;
            HeartbeatMessage.incomeMessage = ProvisionApp._incomeMessage;
            HeartbeatMessage.processedMessage = ProvisionApp._processedMessage;
            HeartbeatMessage.failMessage = ProvisionApp._failMessage;
            HeartbeatMessage.ignoreMessage = ProvisionApp._ignoreMessage;

            if (ProvisionApp._isRunning)
                HeartbeatMessage.status = "Running";
            else
                HeartbeatMessage.status = "Stop";

            if (_sbNameSpaceMgr != null)
                HeartbeatMessage.queueLength = _sbNameSpaceMgr.GetQueue(ProvisionApp._sbProvisionQueue).MessageCount;
            else
                HeartbeatMessage.queueLength = -1;  //Unknow

            HeartbeatMessage.cpu = Math.Round(_cpuCounter.NextValue(), 2) + " %";
            HeartbeatMessage.ramAvail = _ramCounter.NextValue() + " MB";

            HeartbeatMessage.timestampSource = DateTime.UtcNow;
            var jsonString = JsonConvert.SerializeObject(HeartbeatMessage);
            return jsonString;
        }
    }
}
