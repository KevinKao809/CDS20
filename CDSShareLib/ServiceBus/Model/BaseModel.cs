using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSShareLib.ServiceBus.Model
{
    public abstract class BaseModel
    {
        public virtual string job
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public virtual string entity
        {
            get { throw new NotImplementedException(); }
        }
        public int entityId { get; set; }
        public virtual string task
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public int taskId { get; set; }
        public virtual object content
        {
            get { throw new NotImplementedException(); }
        }
        public string requester { get; set; }
        public string requesterEmail { get; set; }
        public string requestDateTime { get { return DateTime.UtcNow.ToString("o"); } }
    }
}
