using CDSShareLib;
using CDSShareLib.Helper;
using IoTHubReceiver.Helper;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubReceiver.Model
{
    class EventProcessorFactoryModel
    {
        public DocDBHelper docDBHelper { get; set; }
        public BlobStorageHelper TelemetryBlobStorageHelper { get; set; }
        public BlobStorageHelper EventBlobStorageHelper { get; set; }
        public QueueClient EventQueueClient { get; set; }
        public QueueClient InfraQueueClient { get; set; }
        public List<SimpleIoTDeviceMessageCatalog> SimpleIoTDeviceMessageCatalogList { get; set; }
        public Dictionary<int, List<EventRuleCatalogEngine>> EventRulesInMessageId { get; set; }
        public Dictionary<string, MessageTransformer> MessageTransformerInDeviceId { get; set; }
        public Dictionary<int, MonitorFrequency> MonitorFrequenceInMinSecByMessageId { get; set; }
        public CdsInfo CdsInfo { get; set; }
    }

    public class MonitorFrequency
    {
        public int timeInMilliSecond;
        public DateTime lastFeedInTime;
    }

    public class MessageTransformer
    {
        public int MessageCatalogID;
        public string TransformJson;
    }

    public class CdsBackendSetting
    {
        public String SuperAdminHeartbeatURL { get; set; }
        public String AdminHeartbeatURL { get; set; }
        public int HeartbeatIntervalInSec { get; set; }
        public string LogStorageConnectionString { get; set; }        
        public string LogStorageContainer { get; set; }
        public LogLevel LogLevel { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string ServiceBusProcessCommandTopic { get; set; }
        public string ServiceBusProvisionQueue { get; set; }
        public string ServiceBusEventActionQueue { get; set; }
        public string RTMessageFeedInURL { get; set; }
    }

    public class CdsInfo
    {
        public string CompanyId { get; set; }
        public string IoTHubId { get; set; }
        public string PartitionNum { get; set; }
        public string Label { get; set; }
        public string IoTHubAlias { get; set; }
        public CdsBackendSetting cdsBackendSetting { get; set; }
        public CompanyInSubscriptionPlan CompanyInSubscriptionPlan { get; set; }
    }
}
