using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;
using sfShareLib;

using System.Threading.Tasks;
using sfAPIService.Models;
using System.Web.Script.Serialization;
using sfAPIService.Filter;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using CDSShareLib.Helper;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
    [RoutePrefix("admin-api/Equipment")]
    public class EquipmentController : ApiController
    {
        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EquipmentModel.Format_Detail>))]
        public IHttpActionResult GetAllEquipmentByCompanyId()
        {
            EquipmentModel EquipmentModel = new EquipmentModel();
            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                List<EquipmentModel.Format_Detail> EquipmentList = EquipmentModel.GetAllByCompanyId(companyId);
                return Content(HttpStatusCode.OK, EquipmentList);
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
        /// Client_Id : SuperAdmin, Admin - OK
        /// </summary>
        /// <param name="metaData">Whether return MetaData or not, Default false</param>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EquipmentModel.Format_Detail))]
        public IHttpActionResult GetEquipmentById(int id, [FromUri] bool metaData = false)
        {
            EquipmentModel EquipmentModel = new EquipmentModel();
            try
            {
                EquipmentModel.Format_Detail Equipment = EquipmentModel.GetById(id, metaData);
                return Content(HttpStatusCode.OK, Equipment);
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
        /// Client_Id : SuperAdmin, Admin - OK
        /// </summary>
        [HttpPost]
        public IHttpActionResult CreateEquipmentFormData([FromBody] EquipmentModel.Format_Create Equipment)
        {
            int companyId = Global.GetCompanyIdFromToken();

            string logForm = "Form : " + JsonConvert.SerializeObject(Equipment);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || Equipment == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            EquipmentModel EquipmentModel = new EquipmentModel();
            try
            {
                int id = EquipmentModel.Create(companyId, Equipment);
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
        /// Client_Id : SuperAdmin, Admin - OK
        /// </summary>
        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult UpdateEquipment(int id, [FromBody] EquipmentModel.Format_Update Equipment)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(Equipment);
            string logAPI = "[Patch] " + Request.RequestUri.ToString();
            if (!ModelState.IsValid || Equipment == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            EquipmentModel EquipmentModel = new EquipmentModel();
            try
            {
                EquipmentModel.Update(id, Equipment);
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
        /// Client_Id : SuperAdmin, Admin - OK
        /// </summary>
        [HttpPut]
        [Route("{id}/Image")]
        public async Task<IHttpActionResult> UploadEquipmentPhotoFile(int id)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return Content(HttpStatusCode.UnsupportedMediaType, HttpResponseFormat.UnsupportedMediaType());

            try
            {
                int companyId = Global.GetCompanyIdFromToken();

                //FileHelper fileHelper = new FileHelper();
                BlobStorageHelper storageHelper = new BlobStorageHelper(Global._systemStorageName, Global._systemStorageKey, Global._imageStorageContainer);
                string root = Path.GetTempPath();
                var provider = new MultipartFormDataStreamProvider(root);

                // Read the form data.
                string fileAbsoluteUri = "";
                await Request.Content.ReadAsMultipartAsync(provider);
                char[] trimChar = { '\"' };

                //FileData
                foreach (MultipartFileData fileData in provider.FileData)
                {
                    //string formColumnName = fileData.Headers.ContentDisposition.Name;
                    //string fileExtenionName = fileData.Headers.ContentDisposition.FileName.Replace("\"", "").Split('.')[1];
                    string fileExtension = Path.GetExtension(fileData.Headers.ContentDisposition.FileName.Replace("\"", "").ToLower());
                    if (fileExtension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase)
                        || fileExtension.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase)
                        || fileExtension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ImageHelper imageHelper = new ImageHelper();
                        string uploadFilePath = String.Format("company-{0}/Equipment/{1}", companyId, id);
                        fileAbsoluteUri = imageHelper.PublishImage(fileData.LocalFileName, storageHelper, uploadFilePath, Global._equipmentPhotoWidthHeight, Global._imageBgColor, Global._imageFormat);

                        //string uploadFilePath = String.Format("company-{0}/Equipment/{1}-default-{2}{3}", companyId, id, (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, fileExtension);
                        //fileAbsoluteUri = storageHelper.SaveFiletoStorage(fileData.LocalFileName, uploadFilePath);
                    }
                    else
                        return Content(HttpStatusCode.BadRequest, HttpResponseFormat.Error("Unsupport File Type."));
                }

                if (fileAbsoluteUri.Equals(""))
                    return Content(HttpStatusCode.BadRequest, HttpResponseFormat.Error("File is empty"));

                //Edit Equipment logo path
                EquipmentModel model = new EquipmentModel();
                model.UpdatePhotoUrl(id, fileAbsoluteUri);

                return Content(HttpStatusCode.OK, new
                {
                    imageURL = fileAbsoluteUri
                });
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                string logAPI = "[Put] " + Request.RequestUri.ToString();
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : SuperAdmin, Admin - OK
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            EquipmentModel EquipmentModel = new EquipmentModel();
            try
            {
                EquipmentModel.DeleteById(id);
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

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.IO;
//using System.Text;
//using sfShareLib;
//using sfAPIService.Models;
//using System.Threading.Tasks;

//using System.Web.Script.Serialization;
//using sfAPIService.Filter;

//namespace sfAPIService.Controllers
//{
//    [Authorize]
//    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
//    [RoutePrefix("admin-api/Equipment")]
//    public class EquipmentController : ApiController
//    {
//        /// <summary>
//        /// Roles : admin, superadmin
//        /// </summary>
//        [HttpGet]
//        public IHttpActionResult GetAll()
//        {
//            EquipmentModels equipmentModel = new Models.EquipmentModels();
//            return Ok(equipmentModel.GetAllEquipment());
//        }

//        /// <summary>
//        /// Roles : admin, superadmin
//        /// </summary>
//        [HttpGet]
//        [Route("Company/{companyId}")]
//        public IHttpActionResult GetAllByCompanyId(int companyId)
//        {
//            EquipmentModels equipmentModel = new Models.EquipmentModels();
//            return Ok(equipmentModel.GetAllEquipmentByCompanyId(companyId));
//        }

//        /// <summary>
//        /// Roles : admin, superadmin
//        /// </summary>
//        [HttpGet]
//        [Route("Equipment/{EquipmentId}")]
//        public IHttpActionResult GetAllByEquipmentId(int EquipmentId)
//        {
//            EquipmentModels equipmentModel = new Models.EquipmentModels();
//            return Ok(equipmentModel.GetAllEquipmentByEquipmentId(EquipmentId));
//        }

//        /// <summary>
//        /// Roles : admin, superadmin
//        /// </summary>
//        [HttpGet]
//        public IHttpActionResult GeById(int id)
//        {
//            EquipmentModels equipmentModel = new EquipmentModels();
//            try
//            {
//                EquipmentModels.Detail company = equipmentModel.getEquipmentById(id);
//                return Ok(company);
//            }
//            catch
//            {
//                return NotFound();
//            }
//        }

//        /// <summary>
//        /// Roles : admin, superadmin
//        /// </summary>
//        [HttpPost]
//        public IHttpActionResult AddFormData([FromBody]EquipmentModels.Edit Equipment)
//        {
//            string logForm = "Form : " + Global._jsSerializer.Serialize(Equipment);
//            string logAPI = "[Post] " + Request.RequestUri.ToString();

//            if (!ModelState.IsValid || Equipment == null)
//            {
//                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
//                return BadRequest("Invalid data");
//            }

//            try
//            {
//                EquipmentModels equipmentModel = new EquipmentModels();
//                int newEquipmentId = equipmentModel.addEquipment(Equipment);
//                return Json(new { id = newEquipmentId});
//            }
//            catch (Exception ex)
//            {
//                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
//                logMessage.AppendLine(logForm);
//                Global._appLogger.Error(logAPI + logMessage);

//                return Content(HttpStatusCode.InternalServerError, ex);
//            }
//        }

//        /// <summary>
//        /// Roles : admin, superadmin
//        /// </summary>
//        [HttpPut]
//        public IHttpActionResult EditFormData(int id, [FromBody] EquipmentModels.Edit Equipment)
//        {
//            JavaScriptSerializer js = new JavaScriptSerializer();
//            string logForm = "Form : " + js.Serialize(Equipment);
//            string logAPI = "[Put] " + Request.RequestUri.ToString();

//            if (!ModelState.IsValid || Equipment == null)
//            {
//                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
//                return BadRequest("Invalid data");
//            }

//            try
//            {
//                EquipmentModels equipmentModel = new EquipmentModels();
//                equipmentModel.updateEquipment(id, Equipment);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
//                logMessage.AppendLine(logForm);
//                Global._appLogger.Error(logAPI + logMessage);

//                return Content(HttpStatusCode.InternalServerError, ex);
//            }
//        }

//        /// <summary>
//        /// Roles : admin, superadmin
//        /// </summary>
//        [HttpDelete]
//        public IHttpActionResult Delete(int id)
//        {
//            try
//            {
//                EquipmentModels equipmentModel = new EquipmentModels();
//                equipmentModel.deleteEquipment(id);
//                return Ok("Success");
//            }
//            catch (Exception ex)
//            {
//                string logAPI = "[Delete] " + Request.RequestUri.ToString();
//                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
//                Global._appLogger.Error(logAPI + logMessage);
//                return InternalServerError();
//            }
//        }

//        /// <summary>
//        /// Roles : admin, superadmin
//        /// </summary>
//        [HttpPut]
//        [Route("{id}/Image")]
//        public async Task<HttpResponseMessage> UploadLogoFile(int id)
//        {
//            // Check if the request contains multipart/form-data.
//            if (!Request.Content.IsMimeMultipartContent())
//                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

//            EquipmentModels equipmentModel = new EquipmentModels();
//            FileHelper fileHelper = new FileHelper();
//            string root = Path.GetTempPath();
//            var provider = new MultipartFormDataStreamProvider(root);

//            try
//            {
//                EquipmentModels.Detail equipment = equipmentModel.getEquipmentById(id);
//            }
//            catch
//            {
//                return Request.CreateResponse(HttpStatusCode.NotFound);
//            }

//            try
//            {
//                // Read the form data.
//                string fileAbsoluteUri = "";
//                await Request.Content.ReadAsMultipartAsync(provider);

//                //FileData
//                foreach (MultipartFileData fileData in provider.FileData)
//                {
//                    string formColumnName = fileData.Headers.ContentDisposition.Name;
//                    string fileExtenionName = fileData.Headers.ContentDisposition.FileName.Split('.')[1];
//                    if (fileHelper.CheckImageExtensionName(formColumnName, fileExtenionName))
//                    {
//                        string uploadFilePath = "company-" + equipmentModel.getCompanyId(id) + "/equipment/" + id + "-default." + fileHelper.LowerAndFilterString(fileExtenionName);
//                        fileAbsoluteUri = fileHelper.SaveFiletoStorage(DBHelper.Common.GetCDSConfigValueByKey("SystemStorageName"), DBHelper.Common.GetCDSConfigValueByKey("SystemStorageKey"),fileData.LocalFileName, uploadFilePath, "images");
//                    }
//                }

//                if (fileAbsoluteUri.Equals(""))
//                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File is empty or wrong extension name");

//                //Edit company logo path
//                equipmentModel.updateEquipmentLogoURL(id, fileAbsoluteUri);
//                return Request.CreateResponse(HttpStatusCode.OK, new { imageURL = fileAbsoluteUri });
//            }
//            catch (System.Exception ex)
//            {
//                string logAPI = "[Put] " + Request.RequestUri.ToString();
//                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
//                Global._appLogger.Error(logAPI + logMessage);
//                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
//            }
//        }

//        /// <summary>
//        /// Roles : admin, superadmin
//        /// </summary>
//        [Route("{equipmentId}/Message")]
//        public IHttpActionResult GetMessageByEquipmentId(string equipmentId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
//        {
//            try
//            {
//                EquipmentModels equipmentModel = new EquipmentModels();
//                int companyId = equipmentModel.getCompanyId(equipmentId);
//                CompanyModel companyModel = new CompanyModel();
//                CompanyModel.Format_Detail company = companyModel.GetById(companyId);

//                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(companyId);
//                if (companySubscription == null)
//                    throw new Exception("can't find valid subscription plan.");

//                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, companySubscription.CosmosDBConnectionString);
//                return Ok(docDBHelpler.GetMessageByEquipmentId(equipmentId, top, hours, order));
//            }
//            catch (Exception ex)
//            {
//                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
//                string logAPI = "[Get] " + Request.RequestUri.ToString();
//                Global._appLogger.Error(logAPI + logMessage);

//                return Content(HttpStatusCode.InternalServerError, ex);
//            }
//        }
//    }
//}
