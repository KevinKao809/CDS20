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
    [RoutePrefix("SuperAdminHeartbeat")]
    public class SuperAdminHeartbeatController : Controller
    {
        [HttpPost]
        [Route("FeedIn")]
        // Post: SuperAdminHeartbeat
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
                Startup._appLogger.Info(string.Format("SuperAdminHeartbeat Received: {0}", feedInData));
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
                Startup._appLogger.Error(string.Format("SuperAdminHeartbeat Exception: {0}", ex.Message));
                return this.Content("{\"Result\":\"Fail\"}", "application/json");
            }
        }

        private void processOneFeedInMessage(dynamic messageObj)
        {
            int iCompanyId = 0;     //Set SuperAdmin as Company ID = 0;
            Startup._hubContext.Clients.Group(iCompanyId.ToString()).onReceivedMessage(JsonConvert.SerializeObject(messageObj));
        }
    }
}