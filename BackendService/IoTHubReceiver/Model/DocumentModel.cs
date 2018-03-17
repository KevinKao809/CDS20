using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IoTHubReceiver.Model
{
    public class DocumentType
    {
        public static string TelemetryDocument = "Telemetry";
        public static string EventDocument = "Event";
    }

    public class TelemetryDocument
    {
        /* Each document in DocumentDB has an id property which uniquely identifies 
        * the document but that id field is of type string. 
        * When creating a document, you may choose not to specify a value for this field 
        * and DocumentDB assigns an id automatically but this value is a GUID. */
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int companyId { get; set; }
        public string iotDeviceId { get; set; }
        public int messageCatalogId { get; set; }
        public string messageType { get; set; }
        public JObject messageContent { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class EventDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int companyId { get; set; }
        public string iotDeviceId { get; set; }
        public int messageCatalogId { get; set; }
        public string messageType { get; set; }
        public int eventRuleCatalogId { get; set; }
        public string eventRuleCatalogName { get; set; }
        public string eventRuleCatalogDescription { get; set; }
        public JObject messageContent { get; set; }
        public string triggeredTime { get; set; }
        public bool eventSent { get; set; }
        public string messageDocumentId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
