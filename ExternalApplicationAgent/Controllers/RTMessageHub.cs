using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Web.Mvc;
using ExternalApplicationAgent.Models;

namespace ExternalApplicationAgent.Controllers
{
    [HubName("RTMessageHub")]
    public class RTMessageHub : Hub
    {
        public void Register()
        {            
            PublishMessage("{\"topic\":\"welcome\"}");
        }

        private void PublishMessage(string message)
        {
            Clients.All.onReceivedMessage(message);
        } 
    }

    public class RTMessageNotification
    {
        public static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<RTMessageHub>();
        public void InformReceivedMessage(string message)
        {
            hubContext.Clients.All.onReceivedMessage(message);
        }

        public void InformSendEmailResult(string resultMessage, EmailModel email)
        {
            string message = "{" + String.Format("\"DateTime\":\"{0}\",\"Status\":\"{1}\", \"Sender\":\"{2}\", \"Receiver\":\"{3}\", \"Subject\":\"{4}\"",DateTime.UtcNow.ToString(), resultMessage, email.senderEmail, email.receverEmail, email.subject) + "}";
            hubContext.Clients.All.onSendEmailResult(message);
        }

        public void InformSendSMSResult(string resultMessage, SMSModel sms)
        {
            string message = "{" + String.Format("\"DateTime\":\"{0}\",\"Status\":\"{1}\", \"Sender\":\"{2}\", \"Receiver\":\"{3}\", \"Content\":\"{4}\"", DateTime.UtcNow.ToString(), resultMessage, sms.senderPhoneNumber, sms.receiverPhoneNumber, sms.smsContent) + "}";
            hubContext.Clients.All.onSendSMSResult(message);
        }

        public void InformSendERPResult(string resultMessage, ERPModel ERPEvent)
        {
            string message = "{" + String.Format("\"DateTime\":\"{0}\",\"Status\":\"{1}\", \"equipmentId\":\"{2}\", \"equipmentName\":\"{3}\", \"eventCode\":\"{4}\", \"eventMessage\":\"{5}\"", ERPEvent.dateTimeString, resultMessage, ERPEvent.equipmentId, ERPEvent.equipmentName, ERPEvent.eventCode, ERPEvent.eventMessage) + "}";
            hubContext.Clients.All.onSendERPResult(message);
        }
    }
}