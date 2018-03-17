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

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
    [RoutePrefix("admin-api/ExternalDashboard")]
    public class ExternalDashboardController : CDSApiController
    {
        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ExternalDashboardModel.Format_Detail>))]
        public IHttpActionResult GetAllExternalDashboard()
        {
            ExternalDashboardModel model = new ExternalDashboardModel();
            return Content(HttpStatusCode.OK, model.GetAllByCompanyId(UserToken.CompanyId));
        }

        ///// <summary>
        ///// Client_Id : Admin - OK
        ///// </summary>
        //[HttpGet]
        //[Route("{id}")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(ExternalDashboardModel.Format_Detail))]
        //public IHttpActionResult GetExternalDashboardById(int id)
        //{
        //    try
        //    {
        //        ExternalDashboardModel model = new ExternalDashboardModel();
        //        return Content(HttpStatusCode.OK, model.GetById(id));
        //    }
        //    catch (CDSException cdsEx)
        //    {
        //        return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.InternalServerError, ex);
        //    }
        //}

        ///// <summary>
        ///// Client_Id : Admin - OK
        ///// </summary>
        //[HttpPost]
        //public IHttpActionResult CreateExternalDashboard([FromBody]ExternalDashboardModel.Format_Create dataModel)
        //{
        //    string logForm = "Form : " + JsonConvert.SerializeObject(dataModel);
        //    string logAPI = "[Post] " + Request.RequestUri.ToString();

        //    if (!ModelState.IsValid || dataModel == null)
        //    {
        //        Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
        //        return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
        //    }

        //    try
        //    {
        //        int companyId = Global.GetCompanyIdFromToken();
        //        ExternalDashboardModel model = new ExternalDashboardModel();
        //        int id = model.Create(companyId, dataModel);
        //        return Content(HttpStatusCode.OK, HttpResponseFormat.Success(id));
        //    }
        //    catch (CDSException cdsEx)
        //    {
        //        return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
        //        logMessage.AppendLine(logForm);
        //        Global._appLogger.Error(logAPI + logMessage);

        //        return Content(HttpStatusCode.InternalServerError, ex);
        //    }
        //}

        ///// <summary>
        ///// Client_Id : Admin - OK
        ///// </summary>
        //[HttpPatch]
        //[Route("{id}")]
        //public IHttpActionResult UpdateExternalDashboard(int id, [FromBody]ExternalDashboardModel.Format_Update dataModel)
        //{
        //    string logForm = "Form : " + JsonConvert.SerializeObject(dataModel);
        //    string logAPI = "[Patch] " + Request.RequestUri.ToString();

        //    if (!ModelState.IsValid || dataModel == null)
        //    {
        //        Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
        //        return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
        //    }

        //    try
        //    {
        //        ExternalDashboardModel model = new ExternalDashboardModel();
        //        model.Update(id, dataModel);

        //        return Content(HttpStatusCode.OK, HttpResponseFormat.Success());
        //    }
        //    catch (CDSException cdsEx)
        //    {
        //        return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
        //        logMessage.AppendLine(logForm);
        //        Global._appLogger.Error(logAPI + logMessage);

        //        return Content(HttpStatusCode.InternalServerError, ex);
        //    }
        //}

        ///// <summary>
        ///// Client_Id : Admin - OK
        ///// </summary>
        //[HttpDelete]
        //[Route("{id}")]
        //public IHttpActionResult DeleteExternalDashboard(int id)
        //{
        //    try
        //    {
        //        ExternalDashboardModel model = new ExternalDashboardModel();
        //        model.DeleteById(id);
        //        return Content(HttpStatusCode.OK, HttpResponseFormat.Success());
        //    }
        //    catch (CDSException cdsEx)
        //    {
        //        return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
        //    }
        //    catch (Exception ex)
        //    {
        //        string logAPI = "[Delete] " + Request.RequestUri.ToString();
        //        StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
        //        Global._appLogger.Error(logAPI + logMessage);
        //        return Content(HttpStatusCode.InternalServerError, ex);
        //    }
        //}
    }
}
