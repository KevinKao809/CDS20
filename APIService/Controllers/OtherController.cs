using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace sfAPIService.Controllers
{
    public class OtherController : ApiController
    {
        /// <summary>
        /// AllowAnonymous - OK
        /// </summary>
        [HttpGet]
        [Route("~/Heartbeat")]
        public IHttpActionResult Heartbeat()
        {
            return Ok("success");
        }
    }
}
