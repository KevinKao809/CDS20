using CDSShareLib.Helper;
using Microsoft.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMSApp.Model
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
            _HBTimer.Change(_heartbeatIntervalinSec * 1000, _heartbeatIntervalinSec * 1000);
            SMSApp._appLogger.Info("Heartbeat Interval: " + _heartbeatIntervalinSec + " seconds");
        }
        private static void PushHeartbeatSignal(object state)
        {
            string jsonHB = GetHeartbeatStatus();
            try
            {
                webHelper.PostContent(_superadminHeartbeatURL, jsonHB);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("SMS Task Exception on send Heartbeat: " + ex.Message);
                SMSApp._appLogger.Error(logMessage);
            }
        }

        private static string GetHeartbeatStatus()
        {
            dynamic HeartbeatMessage = new ExpandoObject();
            HeartbeatMessage.topic = "Process Heartbeat";
            HeartbeatMessage.name = "SMS Task";
            HeartbeatMessage.machine = Environment.MachineName;
            HeartbeatMessage.processId = _ProcessId;
            HeartbeatMessage.incomeMessage = SMSApp._incomeMessage;
            HeartbeatMessage.processedMessage = SMSApp._processedMessage;
            HeartbeatMessage.failMessage = SMSApp._failMessage;
            HeartbeatMessage.ignoreMessage = SMSApp._ignoreMessage;

            if (SMSApp._isRunning)
                HeartbeatMessage.status = "Running";
            else
                HeartbeatMessage.status = "Stop";

            if (_sbNameSpaceMgr != null)
                HeartbeatMessage.queueLength = _sbNameSpaceMgr.GetQueue(SMSApp._sbSMSQueue).MessageCount;
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
