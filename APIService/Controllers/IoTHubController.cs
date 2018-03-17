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
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin")]
    [RoutePrefix("admin-api/IoTHub")]
    public class IoTHubController : ApiController
    {
        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<IoTHubModel.Format_Detail>))]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        public IHttpActionResult GetAllIoTHub()
        {
            int companyId = Global.GetCompanyIdFromToken();
            IoTHubModel model = new IoTHubModel();
            return Content(HttpStatusCode.OK, model.GetAllByCompanyId(companyId));
        }

        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IoTHubModel.Format_Detail))]
        public IHttpActionResult GetIoTHubById(int id)
        {
            try
            {
                IoTHubModel model = new IoTHubModel();
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

        ///// <summary>
        ///// Client_Id : SuperAdmin - OK
        ///// </summary>
        //[HttpPost]
        //public IHttpActionResult CreateIoTHub([FromBody]IoTHubModel.Format_Create dataModel)
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
        //        IoTHubModel model = new IoTHubModel();
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

        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult UpdateIoTHub(int id, [FromBody]IoTHubModel.Format_Update dataModel)
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
                IoTHubModel model = new IoTHubModel();
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
        public IHttpActionResult DeleteIoTHub(int id)
        {
            try
            {
                IoTHubModel model = new IoTHubModel();
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
