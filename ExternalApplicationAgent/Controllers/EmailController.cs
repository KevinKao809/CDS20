using ExternalApplicationAgent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace ExternalApplicationAgent.Controllers
{
    public class EmailController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> PostEmail(EmailModel email)
        {
            if (!ModelState.IsValid || email == null)
            {
                return Content(HttpStatusCode.BadRequest, new { error = "Invalid Input Data" });
            }
            RTMessageNotification rtMessage = new RTMessageNotification();
            try
            {                
                string messageString = new JavaScriptSerializer().Serialize(email);
                rtMessage.InformReceivedMessage(messageString);
                await email.Send();
                rtMessage.InformSendEmailResult("Completed", email);
            }
            catch (Exception ex)
            {
                rtMessage.InformSendEmailResult("Fail", email);
                return Content(HttpStatusCode.InternalServerError, ex);
            }
            return Json(new { result = "OK" });
        }
     }
}
