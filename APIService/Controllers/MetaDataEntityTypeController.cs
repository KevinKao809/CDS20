using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using sfShareLib;

using sfAPIService.Models;
using System.Web.Script.Serialization;
using sfAPIService.Filter;
using Swashbuckle.Swagger.Annotations;
using Newtonsoft.Json;
using System.Reflection;

namespace sfAPIService.Controllers
{
    [RoutePrefix("admin-api/MetaDataEntityType")]
    public class MetaDataEntityTypeController : CDSApiController
    {
        /// <summary>
        /// Client_Id : AllowAnonymous
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<string>))]
        public IHttpActionResult GetAllMetaDataEntityType()
        {
            List<string> entityTypeList = new List<string>();

            foreach (FieldInfo field in typeof(Global.MetaDataEntityType).GetFields())
            {
                entityTypeList.Add(field.GetValue(null).ToString());
            }
            return Content(HttpStatusCode.OK, entityTypeList);
        }
        
    }
}
