using CDSShareLib.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace ProvisionApp.Model
{
    public class IoTHubDeviceRegisterThread
    {
        public string _ConnectionString;
        public string _IoTDeviceId;
        public string _AuthenticationType;
        public string _Action;
        public int _TaskId;
        public JObject _JsonMessage;

        public IoTHubDeviceRegisterThread(string connectionString, JObject jsonMessage, string action, int taskId)
        {
            _ConnectionString = connectionString;
            _IoTDeviceId = jsonMessage["Content"]["iothubDeviceId"].ToString();
            if (string.IsNullOrEmpty(_IoTDeviceId))
                throw new Exception("IoT Device ID can't be null");

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
                switch (_Action)
                {
                    case "register iothub register":
                        _AuthenticationType = _JsonMessage["Content"]["authenticationType"].ToString().ToLower();
                        if (_AuthenticationType == "key")
                        {
                            string IoTHubDeviceKey = _JsonMessage["Content"]["iothubDeviceKey"].ToString();
                            await iotHubHelper.RegisterDeviceByKey(_IoTDeviceId, IoTHubDeviceKey);
                        }
                        else
                        {
                            string CertificateThumbprint = _JsonMessage["Content"]["certificateThumbprint"].ToString();
                            await iotHubHelper.RegisterDeviceByCertThumbprint(_IoTDeviceId, CertificateThumbprint);
                        }
                        break;
                    case "remove iothub register":
                        await iotHubHelper.RemoveDevice(_IoTDeviceId);
                        break;                   
                }
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
