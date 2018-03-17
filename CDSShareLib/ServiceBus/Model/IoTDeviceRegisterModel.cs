using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSShareLib.ServiceBus.Model
{
    public class IoTDeviceRegisterModel : BaseModel
    {
        public override string job { get { return "provisioning iotdevice"; } }
        public override string entity { get { return "iotdevice"; } }
        public override string task { get { return TaskName.IoTDevice_Register; } }
        public new ContentFormat content { get; set; }
        public class ContentFormat
        {
            public string iothubConnectionString { get; set; }
            public string iothubDeviceId { get; set; }
            public string authenticationType { get; set; }
            public string iothubDeviceKey { get; set; }
            public string certificateThumbprint { get; set; }
        }
    }
}
