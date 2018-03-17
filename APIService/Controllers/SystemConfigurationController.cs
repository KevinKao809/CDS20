using CDSShareLib.Helper;
using Newtonsoft.Json;
using sfAPIService.Filter;
using sfAPIService.Models;
using sfShareLib;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace sfAPIService.Controllers
{
    [AllowAnonymous]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin")]
    [RoutePrefix("admin-api/SystemConfiguration")]
    public class SystemConfigurationController : ApiController
    {
        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SystemConfigurationModel.Format_Detail>))]
        public IHttpActionResult GetAllSystemConfiguration()
        {
            int companyId = Global.GetCompanyIdFromToken();
            SystemConfigurationModel model = new SystemConfigurationModel();
            return Content(HttpStatusCode.OK, model.GetAll(companyId));
        }

        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SystemConfigurationModel.Format_Detail))]
        public IHttpActionResult GetSystemConfigurationById(int id)
        {
            try
            {
                SystemConfigurationModel model = new SystemConfigurationModel();
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
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpPost]
        public IHttpActionResult CreateSystemConfiguration([FromBody]SystemConfigurationModel.Format_Create dataModel)
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
                SystemConfigurationModel model = new SystemConfigurationModel();
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
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult UpdateSystemConfiguration(int id, [FromBody]SystemConfigurationModel.Format_Update dataModel)
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
                SystemConfigurationModel model = new SystemConfigurationModel();
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
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteSystemConfiguration(int id)
        {
            try
            {
                SystemConfigurationModel model = new SystemConfigurationModel();
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
    }
}
