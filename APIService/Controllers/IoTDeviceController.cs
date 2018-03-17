using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Dynamic;

using Newtonsoft.Json.Linq;
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
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/IoTDevice")]
    public class IoTDeviceController : CDSApiController
    {
        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<IoTDeviceModel.Format_Detail>))]
        public IHttpActionResult GetAllIoTDevice()
        {
            IoTDeviceModel model = new IoTDeviceModel();
            return Content(HttpStatusCode.OK, model.GetAllByCompanyId(UserToken.CompanyId));
        }

        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IoTDeviceModel.Format_Detail))]
        public IHttpActionResult GetIoTDeviceById(int id)
        {
            try
            {
                IoTDeviceModel model = new IoTDeviceModel();
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
        public IHttpActionResult CreateIoTDevice([FromBody]IoTDeviceModel.Format_Create dataModel)
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
                IoTDeviceModel model = new IoTDeviceModel();
                int id = model.Create(UserToken.CompanyId, dataModel);
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
        public IHttpActionResult UpdateIoTDevice(int id, [FromBody]IoTDeviceModel.Format_Update dataModel)
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
                IoTDeviceModel model = new IoTDeviceModel();
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
        public IHttpActionResult DeleteIoTDevice(int id)
        {
            try
            {
                IoTDeviceModel model = new IoTDeviceModel();
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
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpPut]
        [Route("{id}/ResetPassword")]
        public IHttpActionResult ResetPassword(int id, [FromBody] PasswordModel.Format_Reset dataModel)
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
                IoTDeviceModel model = new IoTDeviceModel();
                model.ResetPassword(id, dataModel);
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
        [HttpGet]
        [Route("{id}/Command")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IoTDeviceCommandModel.Format_UpdateByIoTDevice))]
        public IHttpActionResult UpdateDeviceCommand(int id)
        {
            IoTDeviceCommandModel model = new IoTDeviceCommandModel();
            return Content(HttpStatusCode.OK, model.GetAllByIoTDeviceId(id));
        }

        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpPut]
        [Route("{id}/Command")]
        public IHttpActionResult UpdateDeviceCommand(int id, [FromBody] IoTDeviceCommandModel.Format_UpdateByIoTDevice dataModel)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(dataModel);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            try
            {
                IoTDeviceCommandModel model = new IoTDeviceCommandModel();
                model.UpdateByIoTDevieId(id, dataModel);
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


        ///// <summary>
        ///// Roles : admin, superadmin
        ///// </summary>
        //[HttpGet]
        //[Route("{deviceId}/Configuration")]
        //public IHttpActionResult GetAllConfigurationByDeviceId(string deviceId)
        //{
        //    IoTDeviceConfigurationValueModels iotDCVModel = new IoTDeviceConfigurationValueModels();

        //    //List<IoTDeviceConfigurationValueModels.Detail> configValues = new List<IoTDeviceConfigurationValueModels.Detail>();
        //    //configValues.AddRange(iotDCVModel.GetAllSystemConfiguration(deviceId));
        //    //configValues.AddRange(iotDCVModel.GetAllCustomizedConfiguration(deviceId));            

        //    //return Ok(configValues);
        //    IoTDeviceConfigurationValueModels model = new IoTDeviceConfigurationValueModels();

        //    return Ok(model.GetAll(deviceId));
        //}

        /*
        [HttpPut]
        [Route("{deviceId}/Configuration/System")]
        public IHttpActionResult EditAllConfiguration(string deviceId, IoTDeviceSystemConfigurationValueModel.Edit configurationList)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(configurationList);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                IoTDeviceSystemConfigurationValueModel iotDCVModel = new IoTDeviceSystemConfigurationValueModel();
                iotDCVModel.UpdateAllConfiguration(deviceId, configurationList);
                DBHelper._IoTDevice dbHelpler = new DBHelper._IoTDevice();
                dbHelpler.UpdateDeviceConfigurationStatusAndProperty(deviceId, 0);
                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPut]
        [Route("{deviceId}/Configuration/Customized")]
        public IHttpActionResult AddCustomizedConfiguration(string deviceId, [FromBody] IoTDeviceCustomizedConfigurationValueModel.Edit configuration)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(configuration);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                IoTDeviceCustomizedConfigurationValueModel iotDCCVModel = new IoTDeviceCustomizedConfigurationValueModel();
                iotDCCVModel.RefreshAllByIoTDeviceId(deviceId, configuration);

                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }
        */

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        //[HttpGet]
        //[Route("{deviceId}/Message")]
        //public IHttpActionResult GetAllMessageByDeviceId(int deviceId)
        //{
        //    IoTDeviceMessageCatalogModels iotDMCModels = new IoTDeviceMessageCatalogModels();
        //    return Ok(iotDMCModels.GetAllMessageCatalogByIoTDeviceId(deviceId));
        //}

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        //[HttpPut]
        //[Route("{deviceId}/Message")]
        //public IHttpActionResult AttachMessage(int deviceId, IoTDeviceMessageCatalogModels.Edit iotDMC)
        //{
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    string logForm = "Form : " + js.Serialize(iotDMC);
        //    string logAPI = "[Put] " + Request.RequestUri.ToString();

        //    if (!ModelState.IsValid)
        //        return BadRequest();

        //    try
        //    {
        //        IoTDeviceMessageCatalogModels iotDMCModel = new IoTDeviceMessageCatalogModels();
        //        iotDMCModel.AttachMessage(deviceId, iotDMC);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
        //        Global._appLogger.Error(logAPI + logMessage);

        //        return Content(HttpStatusCode.InternalServerError, ex);
        //    }
        //}

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        //[HttpGet]
        //[Route("{deviceId}/MessageTemplate")]
        //public IHttpActionResult GetAllMessageTemplateByDeviceId(int deviceId)
        //{
        //    IoTDeviceModels iotDeviceModels = new IoTDeviceModels();
        //    IoTDeviceMessageCatalogModels iotDMCModels = new IoTDeviceMessageCatalogModels();
        //    MessageCatalogModels msgCatalogModels = new MessageCatalogModels();

        //    List<object> objectList = new List<object>();
        //    var msgCatalogs = iotDMCModels.GetAllMessageCatalogByIoTDeviceId(deviceId);
        //    foreach (var msgCatalog in msgCatalogs)
        //    {
        //        objectList.Add(msgCatalogModels.GetMessageCatalogTemplate(msgCatalog.MessageCatalogId, deviceId));
        //    }
        //    return Ok(objectList);
        //}
    }
}
