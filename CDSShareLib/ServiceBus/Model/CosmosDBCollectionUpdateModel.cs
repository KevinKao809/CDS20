using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSShareLib.ServiceBus.Model
{
    public class CosmosDBCollectionUpdateModel : BaseModel
    {
        public override string job { get { return "provisioning"; } }
        public override string entity { get { return "company"; } }
        public override string task { get { return TaskName.CosmosdbCollection_Update; } }
        public new ContentFormat content { get; set; }
        public string cosmosDBConnectionString { get; set; }
        public class ContentFormat
        {
            public int companyId { get; set; }
            public string collectionTTL { get; set; }
            public string collectionRU { get; set; }
        }
    }
}
