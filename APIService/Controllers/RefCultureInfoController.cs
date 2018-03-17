using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using sfShareLib;
using sfAPIService.Models;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using CDSShareLib.Helper;

namespace sfAPIService.Controllers
{
    public class RefCultureInfoController : ApiController
    {
        /// <summary>
        /// AllowAnonymous - OK
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(RefCultureInfoModel.Format_Detail))]
        public IHttpActionResult GetAll()
        {
            RedisKey cacheKey = "cultureCodesJson";
            string cacheValue = null;
            if (Global._enableRedisCache)
                cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);
            if (cacheValue == null)
            {
                RefCultureInfoModel model = new RefCultureInfoModel();
                return Ok(model.GetAll());
            }
            else
            {
                return Ok(JsonConvert.DeserializeObject(cacheValue));
                //return Ok(new JavaScriptSerializer().Deserialize<List<RefCultureInfoModels>>(cacheValue));
            }
        }  
    }
}
