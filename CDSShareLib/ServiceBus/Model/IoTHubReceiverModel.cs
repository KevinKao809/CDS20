using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSShareLib.ServiceBus.Model
{
    public class IoTHubReceiverModel : BaseModel
    {
        public override string job { get; set; }
        public override string entity { get { return "iothub"; } }
        public override string task { get; set; }
        public new ContentFormat content { get; set; }
        public class ContentFormat
        {
            public string version { get; set; }
            public int companyId { get; set; }
            public int iotHubId { get; set; }
        }
    }
}
