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
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin")]
    [RoutePrefix("admin-api/BackendTask")]
    public class BackendTaskController : CDSApiController
    {
        //************************************************* Company *************************************************//
        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpPost]
        [Route("Company/{companyId}/CosmosDBCollection")]
        public IHttpActionResult CreateCompanyCosmosDBCollection([FromUri]int companyId, [FromBody]BackendTaskModel.Format_Base dataModel)
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
                BackendTaskModel model = new BackendTaskModel();
                model.CreateCompanyCosmosDBCollection(companyId, dataModel);
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
        [HttpPut]
        [Route("Company/{companyId}/CosmosDBCollection")]
        public IHttpActionResult UpdateCompany([FromUri]int companyId, [FromBody]BackendTaskModel.Format_Base dataModel)
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
                BackendTaskModel model = new BackendTaskModel();
                model.UpdateCompanyCosmosDBCollection(companyId, dataModel);
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
        [Route("Company/{companyId}/CosmosDBCollection")]
        public IHttpActionResult DeleteCompany([FromUri]int companyId, [FromBody]BackendTaskModel.Format_Base dataModel)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(dataModel);
            string logAPI = "[Delete] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || dataModel == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            try
            {
                BackendTaskModel model = new BackendTaskModel();
                model.DeleteCompanyCosmosDBCollection(companyId, dataModel);
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

        //************************************************* IoT Hub *************************************************//
        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpPost]
        [Route("Company/{companyId}/IoTHub/{iotHubId}/LaunchReceiver")]
        public IHttpActionResult LaunchIoTHubReceiver([FromUri]int companyId, [FromUri]int iotHubId, [FromBody]BackendTaskModel.Format_LaunchIoTHubReceiver dataModel)
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
                BackendTaskModel model = new BackendTaskModel();
                model.LaunchIoTHubReceiver(companyId, iotHubId, dataModel);
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
        [HttpPost]
        [Route("Company/{companyId}/IoTHub/{iotHubId}/ShutdownReceiver")]
        public IHttpActionResult ShutdownIoTHubReceiver([FromUri]int companyId, [FromUri]int iotHubId, [FromBody]BackendTaskModel.Format_ShutdownIoTHubReceiver dataModel)
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
                BackendTaskModel model = new BackendTaskModel();
                model.ShuudownIoTHubReceiver(companyId, iotHubId, dataModel);
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
        [HttpPost]
        [Route("IoTDevice/{iotDeviceId}")]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        public IHttpActionResult RegisterIoTDevice([FromUri]int iotDeviceId, [FromBody]BackendTaskModel.Format_Base dataModel)
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
                BackendTaskModel model = new BackendTaskModel();
                model.RegisterIoTDevice(UserToken.CompanyId, iotDeviceId, dataModel);
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
        [HttpPut]
        [Route("IoTDevice/{iotDeviceId}")]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        public IHttpActionResult UpdateIoTDevice([FromUri]int iotDeviceId, [FromBody]BackendTaskModel.Format_UpdateIoTDevice dataModel)
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
                BackendTaskModel model = new BackendTaskModel();
                model.UpdateIoTDevice(UserToken.CompanyId, iotDeviceId, dataModel);
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
        [Route("IoTDevice/{iotDeviceId}")]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        public IHttpActionResult DeleteIoTDevice([FromUri]int iotDeviceId, [FromBody]BackendTaskModel.Format_Base dataModel)
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
                BackendTaskModel model = new BackendTaskModel();
                model.DeleteIoTDevice(UserToken.CompanyId, iotDeviceId, dataModel);
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


    }
}
