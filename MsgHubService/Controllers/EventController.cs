using MsgHubService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsgHubService.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("Event")]
    public class EventController : Controller
    {
        [HttpPost]
        [Route("FeedIn")]
        // Post: Event
        public ActionResult FeedIn()
        {
            /* Verify Caller's IP at allow range */
            if (!CDSValidation.AllowIP(Request.UserHostAddress))
            {
                Startup._appLogger.Info(string.Format("Unauthorized Caller IP {0}", Request.UserHostAddress));
                return this.Content("{\"Result\":\"Fail (Unauthorized)\"}", "application/json");
            }

            MemoryStream memstream = new MemoryStream();
            string feedInData = null;
            Request.InputStream.CopyTo(memstream);
            Startup._appLogger.Info(Request.UserHostAddress);
            memstream.Position = 0;
            using (StreamReader reader = new StreamReader(memstream))
            {
                feedInData = reader.ReadToEnd();
                Startup._appLogger.Info(string.Format("Event Received: {0}", feedInData));
            }
            try
            {
                dynamic jsonMessages = JsonConvert.DeserializeObject(feedInData);
                if (jsonMessages is JArray)
                {
                    foreach (var messageObj in jsonMessages)
                        processOneFeedInMessage(messageObj);
                }
                else
                {
                    dynamic messageObj = JsonConvert.DeserializeObject(feedInData);
                    processOneFeedInMessage(messageObj);
                }
                return this.Content("{\"Result\":\"OK\"}", "application/json");
            }
            catch (Exception ex)
            {
                Startup._appLogger.Error(string.Format("Event Exception: {0}", ex.Message));
                return this.Content("{\"Result\":\"Fail\"}", "application/json");
            }
        }

        /* The message come to here is: */
        /* - Event   */
        private void processOneFeedInMessage(dynamic messageObj)
        {
            String messageString = JsonConvert.SerializeObject(messageObj);
            int iCompanyId = messageObj.companyId;
            Startup._hubContext.Clients.Group(iCompanyId.ToString()).onReceivedMessage(messageString);
        }
    }
}