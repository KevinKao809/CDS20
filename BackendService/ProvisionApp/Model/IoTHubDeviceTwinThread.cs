using CDSShareLib.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisionApp.Model
{
    public class IoTHubDeviceTwinThread
    {
        public string _ConnectionString;
        public string _IoTDeviceId;
        public string _DesiredPropeties;
        public JObject _DeviceConfiguration;    //include CDS_SystemConfig, CDS_SystemConfig, CDS_LastUpdatedTimestamp
        public string _Action;
        public int _TaskId;
        public JObject _JsonMessage;

        public IoTHubDeviceTwinThread(string connectionString, JObject jsonMessage, string action, int taskId)
        {
            _ConnectionString = connectionString;
            _IoTDeviceId = jsonMessage["Content"]["iothubDeviceId"].ToString();            
            _DeviceConfiguration = jsonMessage["Content"]["deviceConfiguration"].Value<JObject>();
            if (string.IsNullOrEmpty(_IoTDeviceId) || _DeviceConfiguration == null)
                throw new Exception("IoT Device ID and Desired Propetries are can't be null");

            _Action = action;
            _TaskId = taskId;
            _JsonMessage = jsonMessage;
        }
        public async void ThreadProc()
        {
            AzureSQLHelper.OperationTaskModel operationTask = new AzureSQLHelper.OperationTaskModel();
            try
            {
                IoTHubHelper iotHubHelper = new IoTHubHelper(_ConnectionString);
                await iotHubHelper.UpdateTwinDesiredProperty(_IoTDeviceId, _DeviceConfiguration);
                ProvisionApp._appLogger.Info("[IoT Hub Device] " + _Action + " success: DeviceId-" + _IoTDeviceId);
                operationTask.UpdateTaskBySuccess(_TaskId);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("[IoT Hub Device] " + _Action + " Failed: DeviceId-" + _IoTDeviceId);
                logMessage.AppendLine("\tMessage:" + JsonConvert.SerializeObject(this));
                logMessage.AppendLine("\tException:" + ex.Message);
                ProvisionApp._appLogger.Error(logMessage);
                operationTask.UpdateTaskByFail(_TaskId, ex.Message);
            }            
        }
    }
}
