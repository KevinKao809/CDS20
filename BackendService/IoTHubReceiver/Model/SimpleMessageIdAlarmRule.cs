using CDSShareLib;
using IoTHubReceiver.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubReceiver.Model
{
    public class EventRuleCatalogEngine
    {
        public int EventRuleCatalogId { get; set; }
        public EventRuleCatalog EventRuleCatalog { get; set; }
        public Dictionary<string, RuleEngineItem> RuleEngineItems { get; set; }
        public DateTime LastTriggerTime { get; set; }
        public bool Triggered { get; set; }
    }

    public class RuleEngineItem
    {
        public string ElementName { get; set; }
        public SupportDataTypeEnum DataType { get; set; }
        public string OrderOperation { get; set; }
        public bool Result { get; set; }
        public Func<DynamicMessageElement, bool> Equality { get; set; }
        public string StringRightValue;
        public string StringEqualOperation;
    }

    public class DynamicMessageElement
    {
        public string Name { get; set; }
        public string StringValue { get; set; }
        public decimal DecimalValue { get; set; }
    }
}
