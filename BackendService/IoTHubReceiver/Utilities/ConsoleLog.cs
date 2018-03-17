using CDSShareLib.Helper;
using IoTHubReceiver.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubReceiver.Utilities
{
    public class ConsoleLog
    {
        private bool _showDebugLog = true;
        private StatelessServiceContext _context = null;
        private LogHelper _appLogger = null;
        string _iotHubReceiverLongName;

        public ConsoleLog(StatelessServiceContext serviceContext, LogHelper appLogger, CdsInfo cdsInfo)
        {
            this._context = serviceContext;
            this._appLogger = appLogger;

            _iotHubReceiverLongName = "C" + cdsInfo.CompanyId + "_I" + cdsInfo.IoTHubId + "_P" + cdsInfo.PartitionNum + "_" + cdsInfo.Label;
            if (_iotHubReceiverLongName.Length > 35)
                _iotHubReceiverLongName = _iotHubReceiverLongName.Substring(0, 35) + "..";
        }

        public void Info(string format, params object[] args)
        {
            if (_showDebugLog)
                writeConsoleLog(format, args);
        }

        public void Error(string format, params object[] args)
        {
            writeConsoleLog("[ERROR] " + format, args);
        }

        public void Warn(string format, params object[] args)
        {
            writeConsoleLog("[Warn] " + format, args);
        }

        public void CosmosDBDebug(string format, params object[] args)
        {
            Info("[CosmosDB] " + format, args);
        }

        public void MessageEventDebug(string format, params object[] args)
        {
            Info("[Event Debug] " + format, args);
        }

        public void MessageEventError(string format, params object[] args)
        {
            Error("[Event Error] " + format, args);
        }

        public void BlobLogInfo(string format, params object[] args)
        {
            if (_appLogger != null)
                _appLogger.Info(build(format, args));
        }

        public void BlobLogDebug(string format, params object[] args)
        {
            if (_appLogger != null)
                _appLogger.Debug(build(format, args));
        }

        public void BlobLogWarn(string format, params object[] args)
        {
            if (_appLogger != null)
                _appLogger.Warn(build(format, args));
        }

        public void BlobLogError(string format, params object[] args)
        {
            if (_appLogger != null)
                _appLogger.Error(build(format, args));
        }

        private StringBuilder build(string format, params object[] args)
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.Append("(" + _iotHubReceiverLongName + ") ");
            logMessage.AppendFormat(format, args);

            return logMessage;
        }

        private void writeConsoleLog(string message, params object[] args)
        {
            if (_context != null)
                ServiceEventSource.Current.ServiceMessage(_context, message, args);
        }
    }
}
