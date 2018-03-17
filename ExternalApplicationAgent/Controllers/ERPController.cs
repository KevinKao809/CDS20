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
    public class ERPController : ApiController
    {
        [HttpPost]
        public IHttpActionResult ERPNotify(ERPModel ERPEvent)
        {
            if (!ModelState.IsValid || ERPEvent == null)
            {
                return Content(HttpStatusCode.BadRequest, new { error = "Invalid Input Data" });
            }
            RTMessageNotification rtMessage = new RTMessageNotification();
            try
            {
                string messageString = new JavaScriptSerializer().Serialize(ERPEvent);
                rtMessage.InformReceivedMessage(messageString);
                rtMessage.InformSendERPResult("Completed", ERPEvent);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex);
            }
            return Json(new { result = "OK" });
        }
    }
}
