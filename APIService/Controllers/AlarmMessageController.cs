using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using sfShareLib;
using sfAPIService.Models;

using sfAPIService.Filter;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CDSShareLib.Helper;

namespace sfAPIService.Controllers
{
    [AllowAnonymous]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/AlarmMessage")]
    public class AlarmMessageController : ApiController
    {
        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [Route("Company/{companyId}")]
        public IHttpActionResult GetAlarmMessageByCompanyId(int companyId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                CompanyModel companyModel = new CompanyModel();
                CompanyModel.Format_Detail company = companyModel.GetById(companyId);
                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, companySubscription.CosmosDBConnectionString);                
                return Ok(docDBHelpler.GetAlarmMessageByCompanyId(companyId, top, hours, order));
            }
            catch (CDSException cdsEx)
            {
                IHttpActionResult response;
                HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.BadRequest);
                string body = new JavaScriptSerializer().Serialize(CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
                responseMsg.Content = new StringContent(body, Encoding.UTF8, "application/json");
                response = ResponseMessage(responseMsg);
                return response;
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
        /// Roles : admin, SuperAdmin
        /// </summary>
        [Route("Factory/{factoryId}")]
        public IHttpActionResult GetAlarmMessageByFactoryId(int factoryId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                FactoryModels factoryModel = new FactoryModels();
                int companyId = factoryModel.getFactoryById(factoryId).CompanyId;
                CompanyModel companyModel = new CompanyModel();
                CompanyModel.Format_Detail company = companyModel.GetById(companyId);
                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, companySubscription.CosmosDBConnectionString);                
                return Ok(docDBHelpler.GetAlarmMessageByFactoryId(factoryId, top, hours, order));
            }
            catch (CDSException cdsEx)
            {
                IHttpActionResult response;
                HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.BadRequest);
                string body = new JavaScriptSerializer().Serialize(CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
                responseMsg.Content = new StringContent(body, Encoding.UTF8, "application/json");
                response = ResponseMessage(responseMsg);
                return response;
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
        /// Roles : admin, SuperAdmin
        /// </summary>
        [Route("Equipment/{equipmentId}")]
        public IHttpActionResult GetAlarmMessageByEquipmentId(int equipmentId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                EquipmentModels equipmentModel = new EquipmentModels();
                EquipmentModels.Detail equipment = equipmentModel.getEquipmentById(equipmentId);                
                CompanyModel companyModel = new CompanyModel();
                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(equipment.CompanyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(equipment.CompanyId, companySubscription.CosmosDBConnectionString);
                return Ok(docDBHelpler.GetAlarmMessageByEquipmentId(equipment.EquipmentId, top, hours, order));
            }
            catch (CDSException cdsEx)
            {
                IHttpActionResult response;
                HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.BadRequest);
                string body = new JavaScriptSerializer().Serialize(CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
                responseMsg.Content = new StringContent(body, Encoding.UTF8, "application/json");
                response = ResponseMessage(responseMsg);
                return response;
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }            
        }
    }
}
