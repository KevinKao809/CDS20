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
using CDSShareLib.Helper;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
    [RoutePrefix("admin-api/EventRuleCatalog")]
    public class EventRuleCatalogController : ApiController
    {
        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EventRuleCatalogModel.Format_Detail))]
        public IHttpActionResult GetEventRuleCatalogByCompanyId()
        {
            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                EventRuleCatalogModel model = new EventRuleCatalogModel();
                return Content(HttpStatusCode.OK, model.GetAllByCompanyId(companyId));
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
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EventRuleCatalogModel.Format_Detail))]
        public IHttpActionResult GetEventRuleCatalogById(int id)
        {
            try
            {
                EventRuleCatalogModel model = new EventRuleCatalogModel();
                return Content(HttpStatusCode.OK, model.GetById(id));
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
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpPost]
        public IHttpActionResult CreateEventRuleCatalog([FromBody]EventRuleCatalogModel.Format_Create dataModel)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(dataModel);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || dataModel == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                EventRuleCatalogModel model = new EventRuleCatalogModel();
                int id = model.Create(companyId, dataModel);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success(id));
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult UpdateEventRuleCatalog(int id, [FromBody]EventRuleCatalogModel.Format_Update dataModel)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(dataModel);
            string logAPI = "[Patch] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || dataModel == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            try
            {
                EventRuleCatalogModel model = new EventRuleCatalogModel();
                model.Update(id, dataModel);

                return Content(HttpStatusCode.OK, HttpResponseFormat.Success());
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteEventRuleCatalog(int id)
        {
            try
            {
                EventRuleCatalogModel model = new EventRuleCatalogModel();
                model.DeleteById(id);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success());
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                string logAPI = "[Delete] " + Request.RequestUri.ToString();
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                Global._appLogger.Error(logAPI + logMessage);
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("{id}/Action")]
        public IHttpActionResult GetAllAction(int id)
        {
            EventInActionModels alarmNotificationModel = new EventInActionModels();
            return Ok(alarmNotificationModel.GetAllApplicationByEventRuleCatalogId(id));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpPut]
        [Route("{id}/Action")]
        public IHttpActionResult AttachAction(int id, [FromBody] EventInActionModels.Edit EventInAction)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(EventInAction);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                EventInActionModels EventRuleCatalogModel = new EventInActionModels();
                EventRuleCatalogModel.AttachApplication(id, EventInAction);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}/Rule")]
        public IHttpActionResult GetAllRules(int id)
        {
            try
            {
                EventRuleItemModel model = new EventRuleItemModel();
                return Ok(model.GetAllByEventRuleCatalogId(id));
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
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpPut]
        [Route("{id}/Rule")]
        public IHttpActionResult UpdateAllRules(int id, [FromBody] EventRuleItemModel.Format_Update dataModel)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(dataModel);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || dataModel == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            try
            {
                EventRuleItemModel model = new EventRuleItemModel();
                model.UpdateAllByEventRuleCatalogId(id, dataModel);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success(id));
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }            
        }
    }
}
