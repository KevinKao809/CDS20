using ExternalApplicationAgent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace ExternalApplicationAgent.Controllers
{
    public class SMSController : ApiController
    {
        [HttpPost]
        public IHttpActionResult PostSMS(SMSModel sms)
        {
            if (!ModelState.IsValid || sms == null)
            {
                return Content(HttpStatusCode.BadRequest, new { error = "Invalid Input Data" });
            }
            RTMessageNotification rtMessage = new RTMessageNotification();
            try
            {
                string messageString = new JavaScriptSerializer().Serialize(sms);
                rtMessage.InformReceivedMessage(messageString);
                sms.Send();
                rtMessage.InformSendSMSResult("Completed", sms);
            }
            catch (Exception ex)
            {
                rtMessage.InformSendSMSResult("Fail", sms);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            return Json(new { result = "OK" });
        }
    }
}
