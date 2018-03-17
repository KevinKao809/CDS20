using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfShareLib.ServiceBus
{
    /*
    public class Helper
    {
        private string ConnectionString { get; set; }
        public Helper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public void SendToQueue(string queueName, string message)
        {
            try
            {
                var client = QueueClient.CreateFromConnectionString(this.ConnectionString, queueName);
                client.Send(new BrokeredMessage(message));

                //StringBuilder logMessage = new StringBuilder();
                //logMessage.AppendLine("Service Bus Message:" + msg);
                //Global._sfAppLogger.Info(logMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Send message to service bus quere failed: " + ex.Message);
            }
        }
        public void SendToTopic(string topicName, string message)
        {
        }
    }
    */
}
