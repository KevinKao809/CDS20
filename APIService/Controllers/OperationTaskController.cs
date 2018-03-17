using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

using System.Text;
using sfShareLib;
using sfAPIService.Models;
using sfAPIService.Filter;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using CDSShareLib.Helper;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
    [RoutePrefix("admin-api/OperationTask")]
    public class OperationTaskController : ApiController
    {
        /// <summary>
        /// Client_Id : SuperAdmin, Admin - OK
        /// </summary>
        /// <param name="hours">1 ~ 168, default 168</param>
        /// <param name="taskStatus">Submit, Completed, Warning, Fail, All, default All</param>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<OperationTaskModel.Format_Detail>))]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin, admin")]
        public IHttpActionResult GetAllOperationTask([FromUri] string taskStatus = "All", [FromUri] int hours = 168)
        {
            if (hours < 1 || hours > 168)
                hours = 168;
            int companyId = Global.GetCompanyIdFromToken();
            OperationTaskModel model = new OperationTaskModel();
            
            //CompanyId = 0 is SuperAdmin
            if (companyId > 0)
                return Content(HttpStatusCode.OK, model.GetAllByCompanyId(companyId, taskStatus, hours));
            else
                return Content(HttpStatusCode.OK, model.GetAll(taskStatus, hours));
        }

        /// <summary>
        /// Client_Id : SuperAdmin, Admin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(OperationTaskModel.Format_Detail))]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin, admin")]
        public IHttpActionResult GetOperationTaskById(int id)
        {
            try
            {
                OperationTaskModel model = new OperationTaskModel();
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
        public IHttpActionResult CreateOperationTask([FromBody]OperationTaskModel.Format_Create dataModel)
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
                OperationTaskModel model = new OperationTaskModel();
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
        public IHttpActionResult UpdateOperationTask(int id, [FromBody]OperationTaskModel.Format_Update dataModel)
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
                OperationTaskModel model = new OperationTaskModel();
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
        public IHttpActionResult DeleteOperationTask(int id)
        {
            try
            {
                OperationTaskModel model = new OperationTaskModel();
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
