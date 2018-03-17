using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using sfShareLib;
using sfAPIService.Models;
using StackExchange.Redis;
using sfAPIService.Filter;
using System.Security.Claims;
using System.Threading;
using System.Text;
using Swashbuckle.Swagger.Annotations;
using CDSShareLib.Helper;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "external")]
    [RoutePrefix("cdstudio")]
    public class ExternalApiController : CDSApiController
    {
        /// <summary>
        /// Client_Id : External - OK
        /// </summary>     
        [HttpGet]
        [Route("Company")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CompanyModel.Format_DetailForExternal))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetCompanyById()
        {
            int companyId = Global.GetCompanyIdFromToken();
            RedisKey cacheKey = "external_company_" + companyId;
            string cacheValue = null;// RedisCacheHelper.GetValueByKey(cacheKey);

            if (cacheValue == null)
            {                
                try
                {
                    CompanyModel model = new CompanyModel();
                    //RedisCacheHelper.SetKeyValue(cacheKey, JsonConvert.SerializeObject(company));
                    return Content(HttpStatusCode.OK, model.GetCompanyByIdForExternal(UserToken.CompanyId));
                }
                catch (CDSException cdsEx)
                {
                    return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.InternalServerError, ex);
                }
            }
            else
            {
                return Ok(new JavaScriptSerializer().Deserialize<Object>(cacheValue));
            }
            
        }

        /// <summary>
        /// Client_Id : External - OK
        /// </summary>    
        [HttpGet]
        [Route("Factory")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<FactoryModel.Format_DetailForExternal>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllFactoryByCompanyId()
        {           
            try
            {
                //RedisCacheHelper.SetKeyValue(cacheKey, JsonConvert.SerializeObject(company));
                FactoryModel model = new FactoryModel();
                return Content(HttpStatusCode.OK, model.External_GetAllByCompanyId(UserToken.CompanyId));
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : External - OK
        /// </summary>
        [HttpGet]
        [Route("Factory/{factoryId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FactoryModel.Format_DetailForExternal))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetFactoryById(int factoryId)
        {           
            try
            {
                FactoryModel model = new FactoryModel();
                //RedisCacheHelper.SetKeyValue(cacheKey, JsonConvert.SerializeObject(company));
                return Content(HttpStatusCode.OK, model.External_GetById(factoryId, UserToken.CompanyId));
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : External - OK
        /// </summary>
        /// <param name="metaData">Whether return MetaData or not, Default false</param>
        [HttpGet]
        [Route("Factory/{factoryId}/Equipment")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EquipmentModel.Format_DetailForExternal>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllEquipmentByFactoryId(int factoryId, [FromUri] bool metaData = false)
        {
            try
            {
                EquipmentModel model = new EquipmentModel();
                return Content(HttpStatusCode.OK, model.External_GetAllByFactoryId(factoryId, metaData, UserToken.CompanyId));
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : External - OK
        /// </summary>
        [HttpGet]
        [Route("Factory/{factoryId}/IoTDevice")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EquipmentModel.Format_DetailForExternal>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllIoTDeviceByFactoryId(int factoryId)
        {
            try
            {
                IoTDeviceModel model = new IoTDeviceModel();
                return Content(HttpStatusCode.OK, model.External_GetAllByFactoryId(factoryId, UserToken.CompanyId));
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("Factory/{factoryId}/AlarmMessage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAlarmMessageByFactoryId(int factoryId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                if (!General.IsFactoryUnderCompany(factoryId, companyId))
                    return Unauthorized();

                CompanyModel companyModel = new CompanyModel();
                CompanyModel.Format_Detail company = companyModel.GetById(companyId);

                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(companyId);
                if (companySubscription == null)
                    throw new Exception("can't find valid subscription plan.");

                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, companySubscription.CosmosDBConnectionString);
                return Ok(docDBHelpler.GetAlarmMessageByFactoryId(factoryId, top, hours, order, companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Global._appLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        /// /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("Factory/{factoryId}/Message")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetMessageByFactoryId(int factoryId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                if (!General.IsFactoryUnderCompany(factoryId, companyId))
                    return Unauthorized();

                CompanyModel companyModel = new CompanyModel();
                CompanyModel.Format_Detail company = companyModel.GetById(companyId);

                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(companyId);
                if (companySubscription == null)
                    throw new Exception("can't find valid subscription plan.");

                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, companySubscription.CosmosDBConnectionString);
                return Ok(docDBHelpler.GetMessageByFactoryId(factoryId, top, hours, order, companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : External - OK
        /// </summary>
        /// <param name="metaData">Whether return MetaData or not, Default false</param>
        [HttpGet]
        [Route("Equipment")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EquipmentModel.Format_DetailForExternal>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllEquipmentByCompanyId([FromUri] bool metaData = false)
        {
            try
            {
                EquipmentModel equipmentModel = new EquipmentModel();
                return Ok(equipmentModel.External_GetAllByCompanyId(UserToken.CompanyId, metaData));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Global._appLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Client_Id : External - OK
        /// </summary>
        /// <param name="metaData">Whether return MetaData or not, Default false</param>
        [HttpGet]
        [Route("Equipment/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EquipmentModel.Format_DetailForExternal))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetEquipmentById(int id, [FromUri] bool metaData = false)
        {
            
            EquipmentModel model = new EquipmentModel();
            try
            {                
                return Content(HttpStatusCode.OK, model.External_GetById(id, metaData, UserToken.CompanyId));
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("Equipment/{equipmentId}/AlarmMessage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAlarmMessageByEquipmentId(int equipmentId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                if (!General.IsEquipmentUnderCompany(equipmentId, companyId))
                    return Unauthorized();

                CompanyModel companyModel = new CompanyModel();
                CompanyModel.Format_Detail company = companyModel.GetById(companyId);

                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(companyId);
                if (companySubscription == null)
                    throw new Exception("can't find valid subscription plan.");

                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, companySubscription.CosmosDBConnectionString);
                EquipmentModels equipmentModel = new EquipmentModels();

                return Ok(docDBHelpler.GetAlarmMessageByEquipmentId(equipmentModel.getEquipmentById(equipmentId).EquipmentId, top, hours, order, companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("Equipment/{equipmentId}/Message")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetMessageByEquipmentId(int equipmentId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                if (!General.IsEquipmentUnderCompany(equipmentId, companyId))
                    return Unauthorized();

                CompanyModel companyModel = new CompanyModel();
                CompanyModel.Format_Detail company = companyModel.GetById(companyId);

                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(companyId);
                if (companySubscription == null)
                    throw new Exception("can't find valid subscription plan.");

                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, companySubscription.CosmosDBConnectionString);
                EquipmentModels equipmentModel = new EquipmentModels();

                return Ok(docDBHelpler.GetMessageByEquipmentId(equipmentModel.getEquipmentById(equipmentId).EquipmentId, top, hours, order, companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("AlarmMessage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAlarmMessageByCompanyId([FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {            
            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                CompanyModel companyModel = new CompanyModel();
                CompanyModel.Format_Detail company = companyModel.GetById(companyId);

                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(companyId);
                if (companySubscription == null)
                    throw new Exception("can't find valid subscription plan.");

                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, companySubscription.CosmosDBConnectionString);
                return Ok(docDBHelpler.GetAlarmMessageByCompanyId(companyId, top, hours, order));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }
        /// <summary>
        /// Roles : external
        /// </summary>
        /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("Message")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetMessageByCompanyId([FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                CompanyModel companyModel = new CompanyModel();
                CompanyModel.Format_Detail company = companyModel.GetById(companyId);

                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(companyId);
                if (companySubscription == null)
                    throw new Exception("can't find valid subscription plan.");

                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, companySubscription.CosmosDBConnectionString);
                return Ok(docDBHelpler.GetMessageByCompanyId(companyId, top, hours, order));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : External - OK
        /// </summary>    
        [HttpGet]
        [Route("IoTDevice")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<IoTDeviceModel.Format_DetailForExternal>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllIoTDeviceByCompanyId()
        {
            try
            {
                IoTDeviceModel model = new IoTDeviceModel();
                return Content(HttpStatusCode.OK, model.External_GetAllByCompanyId(UserToken.CompanyId));
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : External - OK
        /// </summary>
        [HttpGet]
        [Route("IoTDevice/{iotDeviceId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IoTDeviceModel.Format_DetailForExternal))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetIoTDeviceById(int iotDeviceId)
        {
            try
            {
                IoTDeviceModel model = new IoTDeviceModel();
                return Content(HttpStatusCode.OK, model.External_GetById(iotDeviceId, UserToken.CompanyId));
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
