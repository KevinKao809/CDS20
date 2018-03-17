using CDSShareLib.Helper;
using IoTHubReceiver.Utilities;
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

namespace IoTHubReceiver.Model
{
    public class Heartbeat
    {
        private static String _superadminHeartbeatURL;
        private static String _adminHeartbeatURL;
        private static int _heartbeatIntervalInSec = 10;      //Set with Default, it will be overwrite on initial progrma from DB value
        private static long _messageQuotaPerDay = -1;
        private static long _messageConsumedCount = -1;
        private static DateTime _messageConsumedDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        static Timer _HBTimer = null;
        static int _ProcessId = Process.GetCurrentProcess().Id;
        static PerformanceCounter _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        static PerformanceCounter _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        static WebHelper webHelper = new WebHelper();
        private static ConsoleLog _consoleLog;
        private static CdsInfo _cdsInfo;

        public static void Start(CdsInfo cdsInfo)
        {
            _consoleLog = IoTHubReceiver._consoleLog;
            _cdsInfo = cdsInfo;

            CdsBackendSetting cdsBackendSetting = cdsInfo.cdsBackendSetting;
            _superadminHeartbeatURL = cdsBackendSetting.SuperAdminHeartbeatURL;
            _adminHeartbeatURL = cdsBackendSetting.AdminHeartbeatURL;
            _heartbeatIntervalInSec = cdsBackendSetting.HeartbeatIntervalInSec;

            _HBTimer = new Timer(new TimerCallback(PushHeartbeatSignal));
            _HBTimer.Change(_heartbeatIntervalInSec * 1000, _heartbeatIntervalInSec * 1000);
            _consoleLog.Info("Heartbeat Interval: " + _heartbeatIntervalInSec + " seconds");
        }

        public static void UpdateHBInterval(int seconds)
        {
            if (_HBTimer != null)
            {
                _heartbeatIntervalInSec = seconds;
                _HBTimer.Change(_heartbeatIntervalInSec * 1000, _heartbeatIntervalInSec * 1000);
                _consoleLog.Info("Change Heartbeat Interval: " + _heartbeatIntervalInSec + " seconds");
                _consoleLog.BlobLogInfo("Test Change Heartbeat Interval: " + _heartbeatIntervalInSec + " seconds");
            }
        }

        private static void PushHeartbeatSignal(object state)
        {
            string jsonHB = GetHeartbeatStatus();
            try
            {
                webHelper.PostContent(_superadminHeartbeatURL, jsonHB);
                webHelper.PostContent(_adminHeartbeatURL, jsonHB);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("IoTHubReceiver Task Exception on send Heartbeat: " + ex.Message);
                _consoleLog.Error(logMessage.ToString());
            }
        }

        private static string GetHeartbeatStatus()
        {
            dynamic HeartbeatMessage = new ExpandoObject();
            HeartbeatMessage.companyId = _cdsInfo.CompanyId;
            HeartbeatMessage.topic = "Process Heartbeat";
            HeartbeatMessage.name = "IoT Hub Receiver";
            HeartbeatMessage.iotHubId = _cdsInfo.IoTHubId;
            HeartbeatMessage.partition = _cdsInfo.PartitionNum;
            HeartbeatMessage.label = _cdsInfo.Label;
            HeartbeatMessage.machine = Environment.MachineName;
            HeartbeatMessage.processId = _ProcessId;

            if (IoTHubMessageReceiver._isRunning)
                HeartbeatMessage.status = "Running (" + IoTHubMessageReceiver._runStatus + ")";
            else
                HeartbeatMessage.status = "Shutdown";

            HeartbeatMessage.cpu = Math.Round(_cpuCounter.NextValue(), 2) + " %";
            HeartbeatMessage.ramAvail = _ramCounter.NextValue() + " MB";

            HeartbeatMessage.messageQuotaPerDay = _messageQuotaPerDay;
            HeartbeatMessage.messageConsumed = _messageConsumedCount;
            HeartbeatMessage.messageConsumedDate = _messageConsumedDate.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            HeartbeatMessage.timestampSource = DateTime.UtcNow;
            var jsonString = JsonConvert.SerializeObject(HeartbeatMessage);
            return jsonString;
        }

        public static void updateConsumedMessage(long messageQuotaPerDay, long messageConsumedCount, DateTime messageConsumedDate)
        {
            _messageQuotaPerDay = messageQuotaPerDay;
            _messageConsumedCount = messageConsumedCount;
            _messageConsumedDate = messageConsumedDate;
        }
    }
}
