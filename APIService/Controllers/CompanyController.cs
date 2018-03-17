using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

using sfShareLib;
using sfAPIService.Models;
using StackExchange.Redis;
using sfAPIService.Filter;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using CDSShareLib.Helper;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin")]
    [RoutePrefix("admin-api/Company")]
    public class CompanyController : ApiController
    {
        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<CompanyModel.Format_Detail>))]
        public IHttpActionResult GetAllCompanies()
        {
            CompanyModel companyModel = new CompanyModel();
            try
            {
                return Content(HttpStatusCode.OK, companyModel.GetAll());
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
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CompanyModel.Format_Detail))]
        public IHttpActionResult GetCompanyById(int id)
        {
            RedisKey cacheKey = "company_" + id;
            string cacheValue = null;
            if (Global._enableRedisCache)
                cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);
            if (cacheValue == null)
            {
                CompanyModel companyModel = new CompanyModel();
                try
                {
                    CompanyModel.Format_Detail company = companyModel.GetById(id);
                    //RedisCacheHelper.SetKeyValue(cacheKey, JsonConvert.SerializeObject(company));
                    return Content(HttpStatusCode.OK, company);
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
                return Content(HttpStatusCode.OK, JsonConvert.DeserializeObject(cacheValue));
            }
        }       

        //[HttpGet]
        //[Route("{id}/AllSubscriptionPlan")]
        //public HttpResponseMessage GetAllSubscriptionPlanByCompanyId(int id)
        //{
        //    CompanyModel companyModel = new Models.CompanyModel();
        //    try
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, companyModel.GetSubscriptionPlanByCompanyId(id));
        //    }
        //    catch (CDSException cdsEx)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        //    }
        //}

        //[HttpGet]
        //[Route("{id}/ValidSubscriptionPlan")]
        //public HttpResponseMessage GetValidSubscriptionPlanByCompanyId(int id)
        //{
        //    CompanyModel companyModel = new Models.CompanyModel();
        //    try
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, companyModel.GetValidSubscriptionPlanByCompanyId(id));
        //    }
        //    catch (CDSException cdsEx)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        //    }            
        //}

        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpPost]
        public IHttpActionResult CreateCompany(CompanyModel.Format_Create company)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(company);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || company == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            try
            {
                CompanyModel companyModel = new CompanyModel();
                int id = companyModel.Create(company);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success(id));
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
        /// AllowAnonymous
        /// </summary>
        [AllowAnonymous]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "")]
        [HttpGet]
        [Route("{id}/AllowDomain")]
        public HttpResponseMessage GetAllowDomainByCompanyId(int id)
        {
            RedisKey cacheKey = "company_" + id + "_allowDomain";
            string cacheValue = null;
            if (Global._enableRedisCache)
                cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);

            if (cacheValue == null)
            {
                CompanyModel companyModel = new Models.CompanyModel();
                try
                {
                    CompanyModel.Format_Detail company = companyModel.GetById(id);
                    //RedisCacheHelper.SetKeyValue(cacheKey, new JavaScriptSerializer().Serialize(company));
                    return Request.CreateResponse(HttpStatusCode.OK, company.AllowDomain);
                }
                catch (CDSException cdsEx)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject<CompanyModel.Format_Detail>(cacheValue).AllowDomain);
            }
        }

        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpPut]
        [Route("{id}/Image")]
        public async Task<IHttpActionResult> UploadLogoFile(int id)
        {
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return Content(HttpStatusCode.UnsupportedMediaType, HttpResponseFormat.UnsupportedMediaType());

            CompanyModel companyModel = new CompanyModel();
            //FileHelper fileHelper = new FileHelper();
            BlobStorageHelper storageHelper = new BlobStorageHelper(Global._systemStorageName, Global._systemStorageKey, Global._imageStorageContainer);
            string root = Path.GetTempPath();
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                CompanyModel.Format_Detail company = companyModel.GetById(id);
            }
            catch (CDSException cdsEx)
            {
                return Content(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }

            try
            {
                // Read the form data.
                string LogoAbsoluteUri = "", IconAbsoluteUri = "";
                await Request.Content.ReadAsMultipartAsync(provider);

                //FileData
                foreach (MultipartFileData fileData in provider.FileData)
                {
                    string fileExtension = Path.GetExtension(fileData.Headers.ContentDisposition.FileName.Replace("\"", "").ToLower());
                    if (fileExtension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) 
                        || fileExtension.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase)
                        || fileExtension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        
                        ImageHelper imageHelper = new ImageHelper();
                        string uploadFilePath = String.Format("company-{0}", id);

                        LogoAbsoluteUri = imageHelper.PublishImage(fileData.LocalFileName, storageHelper, uploadFilePath, Global._companyLogoWidthHeight, Global._imageBgColor, Global._imageFormat);                        
                        IconAbsoluteUri = imageHelper.PublishImage(fileData.LocalFileName, storageHelper, uploadFilePath, Global._companyIconWidthHeight, Global._imageBgColor, Global._imageFormat);
                    }
                    else
                        return Content(HttpStatusCode.BadRequest, HttpResponseFormat.Error("Unsupport File Type."));
                }

                if (LogoAbsoluteUri.Equals(""))
                    return Content(HttpStatusCode.BadRequest, HttpResponseFormat.Error("File is empty or wrong extension name."));

                //Edit company logo path
                companyModel.UpdateLogoURL(id, LogoAbsoluteUri, IconAbsoluteUri);
                //RedisCacheHelper.DeleteCompanyCache(id);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success(LogoAbsoluteUri));
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
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult UpdateComapny(int id, [FromBody] CompanyModel.Format_Update company)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(company);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || company == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            try
            {
                CompanyModel companyModel = new CompanyModel();
                companyModel.UpdateById(id, company);
                //RedisCacheHelper.DeleteCompanyCache(id);
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
        public IHttpActionResult DeleteCompany(int id)
        {
            try
            {
                CompanyModel companyModel = new CompanyModel();
                companyModel.DeleteById(id);
                //RedisCacheHelper.DeleteCompanyCache(id);
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

        /***************************************** Employee *****************************************/
        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}/Employee")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EmployeeModel.Format_Detail>))]
        public IHttpActionResult GetAllEmployeeByCompanyId(int id)
        {
            EmployeeModel model = new EmployeeModel();
            return Content(HttpStatusCode.OK, model.GetAllByCompanyId(id));
        }

        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpPost]
        [Route("{id}/Employee")]
        public IHttpActionResult CreateEmployee(int id, [FromBody]EmployeeModel.Format_Create employee)
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
                EmployeeModel model = new EmployeeModel();
                int employeeId = model.Create(id, employee);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success(employeeId));
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

        /***************************************** IoT Hub *****************************************/
        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}/IoTHub")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<IoTHubModel.Format_Detail>))]
        public IHttpActionResult GetAllIoTHubByCompanyId(int id)
        {
            IoTHubModel model = new IoTHubModel();
            return Content(HttpStatusCode.OK, model.GetAllByCompanyId(id));
        }

        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpPost]
        [Route("{id}/IoTHub")]
        public IHttpActionResult CreateIoTHub(int id, [FromBody]IoTHubModel.Format_Create dataModel)
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
                IoTHubModel model = new IoTHubModel();
                int iothubId = model.Create(id, dataModel);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success(iothubId));
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

        /***************************************** CompanyInSubscription *****************************************/
        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}/Subscription")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<CompanyInSubscriptionPlanModel.Format_Detail>))]
        public IHttpActionResult GetAllSubscriptionByCompanyId(int id)
        {
            CompanyInSubscriptionPlanModel model = new CompanyInSubscriptionPlanModel();
            return Content(HttpStatusCode.OK, model.GetAllByCompanyId(id));
        }

        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}/Subscription/{subscriptionId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CompanyModel.Format_Detail))]
        public IHttpActionResult GetSubscriptionByCompanyId(int id, int subscriptionId)
        {
            try
            {
                CompanyInSubscriptionPlanModel model = new CompanyInSubscriptionPlanModel();
                return Content(HttpStatusCode.OK, model.Get(id, subscriptionId));
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
        [Route("{id}/Subscription")]
        public IHttpActionResult CreateSubscriptionByCompanyId(int id, [FromBody]CompanyInSubscriptionPlanModel.Format_Create dataModel)
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
                CompanyInSubscriptionPlanModel model = new CompanyInSubscriptionPlanModel();
                int subScriptionId = model.Create(id, dataModel);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success(subScriptionId));
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
        [Route("{id}/Subscription/{subscriptionId}")]
        public IHttpActionResult UpdateSubscription(int id, int subscriptionId, [FromBody] CompanyInSubscriptionPlanModel.Format_Update dataModel)
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
                CompanyInSubscriptionPlanModel model = new CompanyInSubscriptionPlanModel();
                model.Update(id, subscriptionId, dataModel);
                //RedisCacheHelper.DeleteCompanyCache(id);
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
        [Route("{id}/Subscription/{subscriptionId}")]
        public IHttpActionResult DeleteSubscription(int id, int subscriptionId)
        {
            try
            {
                CompanyInSubscriptionPlanModel model = new CompanyInSubscriptionPlanModel();
                model.Delete(id, subscriptionId);
                //RedisCacheHelper.DeleteCompanyCache(id);
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

        /***************************************** ExternalDashboard *****************************************/
        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}/ExternalDashboard")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ExternalDashboardModel.Format_Detail>))]
        public IHttpActionResult GetAllExternalDashboard([FromUri]int id)
        {
            ExternalDashboardModel model = new ExternalDashboardModel();
            return Content(HttpStatusCode.OK, model.GetAllByCompanyId(id));
        }

        /// <summary>
        /// Client_Id : SuperAdmin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}/ExternalDashboard/{externalDashboardId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ExternalDashboardModel.Format_Detail))]
        public IHttpActionResult GetExternalDashboardById(int id, int externalDashboardId)
        {
            try
            {
                ExternalDashboardModel model = new ExternalDashboardModel();
                return Content(HttpStatusCode.OK, model.GetById(externalDashboardId, id));
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
        [Route("{id}/ExternalDashboard")]
        public IHttpActionResult CreateExternalDashboard(int id, [FromBody]ExternalDashboardModel.Format_Create dataModel)
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
                ExternalDashboardModel model = new ExternalDashboardModel();
                int dataId = model.Create(id, dataModel);
                return Content(HttpStatusCode.OK, HttpResponseFormat.Success(dataId));
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
        [Route("{id}/ExternalDashboard/{externalDashboardId}")]
        public IHttpActionResult UpdateExternalDashboard(int id, int externalDashboardId, [FromBody]ExternalDashboardModel.Format_Update dataModel)
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
                ExternalDashboardModel model = new ExternalDashboardModel();
                model.Update(externalDashboardId, dataModel, id);

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
        [Route("{id}/ExternalDashboard/{externalDashboardId}")]
        public IHttpActionResult DeleteExternalDashboard(int id, int externalDashboardId)
        {
            try
            {
                ExternalDashboardModel model = new ExternalDashboardModel();
                model.DeleteById(externalDashboardId, id);
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
        //[HttpGet]
        //[Route("{id}/UsageLog")]
        //public HttpResponseMessage GetAllUsageLog(int id, [FromUri]int days = 1, [FromUri]string order = "asc")
        //{
        //    UsagLogModels usageLogModel = new UsagLogModels();
        //    try
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, usageLogModel.getAllByCompanyId(id, days, order));
        //    }
        //    catch (CDSException cdsEx)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        //    }
        //}

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        //[HttpGet]
        //[Route("{id}/UsageLog/Last")]
        //public HttpResponseMessage GetLastUsageLog(int id)
        //{
        //    try
        //    {
        //        UsagLogModels usageLogModel = new UsagLogModels();
        //        return Request.CreateResponse(HttpStatusCode.OK, usageLogModel.getLastByCompanyId(id));
        //    }
        //    catch (CDSException cdsEx)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, CDSException.GetCDSErrorMessageByCode(cdsEx.ErrorId));
        //    }
        //    catch (Exception ex)
        //    {
        //        string logAPI = "[Get] " + Request.RequestUri.ToString();
        //        StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
        //        Global._appLogger.Error(logAPI + logMessage);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        //    }            
        //}

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("{id}/IoTDeviceCustomizedConfiguration")]
        public IHttpActionResult GetAllIoTDeviceCustomizedConfiguration(int id)
        {
            IoTDeviceCustomizedConfigurationModels iotDeviceCCModels = new IoTDeviceCustomizedConfigurationModels();
            return Ok(iotDeviceCCModels.GetAllCustomizedConfigurationByCompanyId(id));
        }
        
        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [Route("{id}/Message")]
        public IHttpActionResult GetMessageByCompanyId(int id, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                CompanyModel companyModel = new CompanyModel();
                CompanyModel.Format_Detail company = companyModel.GetById(id);

                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(id);
                if (companySubscription == null)
                    throw new Exception("can't find valid subscription plan.");

                DocumentDBHelper docDBHelpler = new DocumentDBHelper(id, companySubscription.CosmosDBConnectionString);
                return Ok(docDBHelpler.GetMessageByCompanyId(id, top, hours, order));
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
