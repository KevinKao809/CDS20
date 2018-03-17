using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.Threading.Tasks;

namespace sfAPIService.Models
{
    public class DeviceViewModels
    {
    }
    public class DeviceModels
    {
        public class Detail
        {
            public int DeviceId { get; set; }
            public string IoTHubName { get; set; }
            public string IoTHubProtocol { get; set; }
            public string IoTHubAuthenticationType { get; set; }
            public string CertFileName { get; set; }
            public string KeyFileName { get; set; }
            public string Password { get; set; }
            public string DeviceKey { get; set; }
            public string ContainerName { get; set; }
        }

        public async Task<Detail> GetIoTDeviceByDeviceId(int id)
        {
            DeviceUtility deviceHelper = new DeviceUtility();
            CDStudioEntities dbEnty = new CDStudioEntities();
            var device = dbEnty.IoTDevice.Find(id);
            if (device == null)
                throw new Exception("404");

            Detail returnDeviceInfo = new Detail()
            {
                DeviceId = device.Id,
                IoTHubProtocol = device.IoTHubProtocol,
                IoTHubAuthenticationType = device.AuthenticationType
            };

            //Confirm connectionstring
            if (await deviceHelper.CheckIoTHubConnectionString(device.IoTHub.IoTHubConnectionString, device.IoTHubDeviceID))
            {
                returnDeviceInfo.IoTHubName = device.IoTHub.IoTHubConnectionString.Split(';')[0].Split('=')[1];
                returnDeviceInfo.ContainerName = device.IoTHub.UploadContainer;
            }            
            else
                throw new Exception("None vaild IoT Hub");

            if (returnDeviceInfo.IoTHubAuthenticationType == "Key")
            {
                returnDeviceInfo.DeviceKey = device.IoTHubDeviceKey;
            }
            else if (returnDeviceInfo.IoTHubAuthenticationType == "Certificate")
            {                
                returnDeviceInfo.CertFileName = device.DeviceCertificate.CertFile;
                returnDeviceInfo.KeyFileName = device.DeviceCertificate.KeyFile;
                returnDeviceInfo.Password = device.DeviceCertificate.Password;
            }

            return returnDeviceInfo;
        }
    }

}