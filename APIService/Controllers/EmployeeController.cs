using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using System.IO;
using System.Threading.Tasks;
using sfAPIService.Models;
using System.Web.Helpers;
using sfShareLib;
using System.Web.Script.Serialization;
using System.Text;
using StackExchange.Redis;
using sfAPIService.Filter;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using CDSShareLib.Helper;

namespace sfAPIService.Controllers
{
    [Authorize]
    [RoutePrefix("admin-api/Employee")]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    public class EmployeeController : ApiController
    {
        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EmployeeModel.Format_Detail>))]
        public IHttpActionResult GetAllEmployeeByCompanyId()
        {
            int companyId = Global.GetCompanyIdFromToken();
            EmployeeModel model = new EmployeeModel();

            return Content(HttpStatusCode.OK, model.GetAllByCompanyId(companyId));
        }

        /// <summary>
        /// Client_Id : SuperAdmin, Admin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}")]

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EmployeeModel.Format_Detail))]
        public IHttpActionResult GetEmployeeById(int id)
        {
            RedisKey cacheKey = "employee_" + id;
            string cacheValue = null;
            if (Global._enableRedisCache)
                cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);

            if (cacheValue == null)
            {
                try
                {
                    EmployeeModel model = new EmployeeModel();
                    var employee = model.GetById(id);
                    //RedisCacheHelper.SetKeyValue(cacheKey, new JavaScriptSerializer().Serialize(employee));
                    return Content(HttpStatusCode.OK, employee);
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
                return Ok(JsonConvert.DeserializeObject<Object>(cacheValue));
            }
        }

        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpPost]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        public IHttpActionResult CreateEmployee([FromBody]EmployeeModel.Format_Create employee)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(employee);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || employee == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                EmployeeModel model = new EmployeeModel();
                int id = model.Create(companyId, employee);
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
        public IHttpActionResult UpdateEmployee(int id, [FromBody]EmployeeModel.Format_Update employee)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(employee);
            string logAPI = "[Patch] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || employee == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                EmployeeModel model = new EmployeeModel();
                model.Update(id, employee);

                //RedisCacheHelper.DeleteEmployeeCache(id);
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
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteEmployee(int id)
        {
            try
            {
                EmployeeModel model = new EmployeeModel();
                model.DeleteById(id);

                //RedisCacheHelper.DeleteEmployeeCache(id);
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
        /// Client_Id : SuperAdmin, Admin - OK
        /// </summary>
        [HttpPut]
        [Route("{id}/Image")]
        public async Task<IHttpActionResult> UploadLogoFile(int id)
        {
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return Content(HttpStatusCode.UnsupportedMediaType, HttpResponseFormat.UnsupportedMediaType());

            EmployeeModel model = new EmployeeModel();
            //FileHelper fileHelper = new FileHelper();
            BlobStorageHelper storageHelper = new BlobStorageHelper(Global._systemStorageName, Global._systemStorageKey, Global._imageStorageContainer);
            string root = Path.GetTempPath();
            var provider = new MultipartFormDataStreamProvider(root);    

            try
            {
                // Read the form data.
                string PhotoAbsoluteUri = "", IconAbsoluteUri = "";
                await Request.Content.ReadAsMultipartAsync(provider);

                //FileData
                foreach (MultipartFileData fileData in provider.FileData)
                {
                    //string formColumnName = fileData.Headers.ContentDisposition.Name;
                    //string fileExtenionName = fileData.Headers.ContentDisposition.FileName.Replace("\"", "").Split('.')[1];
                    string fileExtension = Path.GetExtension(fileData.Headers.ContentDisposition.FileName.ToLower());
                    if (fileExtension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase)
                        || fileExtension.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase)
                        || fileExtension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int companyId = model.GetCompanyId(id);

                        ImageHelper imageHelper = new ImageHelper();
                        string uploadFilePath = String.Format("company-{0}/employee/{}", companyId, id);

                        PhotoAbsoluteUri = imageHelper.PublishImage(fileData.LocalFileName, storageHelper, uploadFilePath, Global._employeePhotoWidthHeight, Global._imageBgColor, Global._imageFormat);
                        IconAbsoluteUri = imageHelper.PublishImage(fileData.LocalFileName, storageHelper, uploadFilePath, Global._employeeIconWidthHeight, Global._imageBgColor, Global._imageFormat);
                        
                        //string uploadFilePath = String.Format("company-{0}/employee/{1}-default{2}", companyId, id, fileExtension);
                        //fileAbsoluteUri = storageHelper.SaveFiletoStorage(fileData.LocalFileName, uploadFilePath);
                    }                    
                }

                if (PhotoAbsoluteUri.Equals(""))
                    return Content(HttpStatusCode.BadRequest, HttpResponseFormat.Error("File is empty or wrong extension name."));

                //Edit employee photo path                
                model.UpdatePhotoURL(id, PhotoAbsoluteUri, IconAbsoluteUri);

                //RedisCacheHelper.DeleteEmployeeCache(id);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success(PhotoAbsoluteUri));
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (System.Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpPut]
        [Route("{id}/ChangePassword")]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        public IHttpActionResult ChangeEmployeePassword(int id, [FromBody]PasswordModel.Format_Change dataModel)
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
                EmployeeModel model = new EmployeeModel();
                model.ChangePassword(id, dataModel);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success());
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.Unauthorized, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
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
        [Route("{id}/ResetPassword")]
        public IHttpActionResult ResetEmployeePassword(int id, [FromBody]PasswordModel.Format_Reset dataModel)
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
                EmployeeModel model = new EmployeeModel();
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
        [Route("{id}/Permission")]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<PermissionCatalogModel.Format_Detail>))]
        public IHttpActionResult GetPermissionsByEmployeeId(int id)
        {
            RedisKey cacheKey = "employee_" + id + "_Permission";
            string cacheValue = null;
            if (Global._enableRedisCache)
                cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);
            if (cacheValue == null)
            {
                try
                {
                    PermissionCatalogModel model = new PermissionCatalogModel();
                    return Content(HttpStatusCode.OK, model.GetAllPermissionByEmployeeId(id));
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
                return Content(HttpStatusCode.OK, JsonConvert.DeserializeObject<List<Object>>(cacheValue));
            }
        }
        
        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}/Role")]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EmployeeInRoleModel.Format_Detail>))]
        public IHttpActionResult GetRolesByEmployeeId(int id)
        {
            RedisKey cacheKey = "employee_" + id + "_Role";
            string cacheValue = null;
            if (Global._enableRedisCache)
                cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);

            if (cacheValue == null)
            {
                try
                {
                    EmployeeInRoleModel model = new EmployeeInRoleModel();
                    model.GetAllByEmployeeId(id);
                    return Content(HttpStatusCode.OK, model.GetAllByEmployeeId(id));
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
                return Content(HttpStatusCode.OK, JsonConvert.DeserializeObject<List<Object>>(cacheValue));
            }
        }

        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpPost]
        [Route("{id}/Role")]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        public IHttpActionResult AddRolesByEmployeeId(int id, [FromBody] EmployeeInRoleModel.Format_Create dataModel)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(dataModel);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || dataModel == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }
            else
            {
                try
                {
                    EmployeeInRoleModel model = new EmployeeInRoleModel();
                    model.CreateManyByEmployeeId(id, dataModel.UserRoleIdList);
                    return Content(HttpStatusCode.OK, model.GetAllByEmployeeId(id));
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

        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpPut]
        [Route("{id}/Role")]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        public IHttpActionResult EditRolesByEmployeeId(int id, [FromBody] EmployeeInRoleModel.Format_Update dataModel)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(dataModel);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            try
            {
                EmployeeInRoleModel model = new EmployeeInRoleModel();
                model.DeleteAllByEmployeeId(id);

                if (dataModel != null && dataModel.UserRoleIdList.Count > 0)
                    model.CreateManyByEmployeeId(id, dataModel.UserRoleIdList);
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
