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
    [RoutePrefix("admin-api/Factory")]
    public class FactoryController : ApiController
    {
        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<FactoryModel.Format_Detail>))]
        public IHttpActionResult GetAllFactoryByCompanyId()
        {
            FactoryModel factoryModel = new FactoryModel();
            try
            {
                int companyId = Global.GetCompanyIdFromToken();
                List<FactoryModel.Format_Detail> factoryList = factoryModel.GetAllByCompanyId(companyId);
                return Content(HttpStatusCode.OK, factoryList);
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FactoryModel.Format_Detail))]
        public IHttpActionResult GetFactoryById(int id)
        {
            FactoryModel factoryModel = new FactoryModel();
            try
            {
                FactoryModel.Format_Detail factory = factoryModel.GetById(id);
                return Content(HttpStatusCode.OK, factory);
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
        public IHttpActionResult CreateFactoryFormData([FromBody] FactoryModel.Format_Create factory)
        {
            int companyId = Global.GetCompanyIdFromToken();

            string logForm = "Form : " + JsonConvert.SerializeObject(factory);
            string logAPI = "[Post] " + Request.RequestUri.ToString();
            
            if (!ModelState.IsValid || factory == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            FactoryModel factoryModel = new FactoryModel();
            try
            {
                int id = factoryModel.Create(companyId, factory);
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
        public IHttpActionResult UpdateFactory(int id, [FromBody] FactoryModel.Format_Update factory)
        {
            string logForm = "Form : " + JsonConvert.SerializeObject(factory);
            string logAPI = "[Patch] " + Request.RequestUri.ToString();
            if (!ModelState.IsValid || factory == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return Content(HttpStatusCode.BadRequest, HttpResponseFormat.InvaildData());
            }

            FactoryModel factoryModel = new FactoryModel();
            try
            {
                factoryModel.Update(id, factory);
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
        [Route("{id}/Image")]
        public async Task<IHttpActionResult> UploadFactoryPhotoFile(int id)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return Content(HttpStatusCode.UnsupportedMediaType, HttpResponseFormat.UnsupportedMediaType());

            try
            {
                // Check factory is existing
                FactoryModel factoryModel = new FactoryModel();
                var existingFactory = factoryModel.GetByIdForInternal(id);

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
                    string formColumnName = fileData.Headers.ContentDisposition.Name.ToLower().Trim(trimChar);
                    string fileExtenionName = fileData.Headers.ContentDisposition.FileName.Split('.')[1].ToLower().Trim(trimChar);
                    if (formColumnName.Equals("image"))
                    {
                        //string formColumnName = fileData.Headers.ContentDisposition.Name;
                        //string fileExtenionName = fileData.Headers.ContentDisposition.FileName.Replace("\"", "").Split('.')[1];
                        string fileExtension = Path.GetExtension(fileData.Headers.ContentDisposition.FileName.Replace("\"", "").ToLower());
                        if (fileExtension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase)
                            || fileExtension.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase)
                            || fileExtension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ImageHelper imageHelper = new ImageHelper();
                            string uploadFilePath = String.Format("company-{0}/Factory/{1}", existingFactory.CompanyId, id);
                            fileAbsoluteUri = imageHelper.PublishImage(fileData.LocalFileName, storageHelper, uploadFilePath, Global._factoryPhotoWidthHeight, Global._imageBgColor, Global._imageFormat);

                            //string uploadFilePath = String.Format("company-{0}/Factory/{1}-default-{2}{3}", existingFactory.CompanyId, id, (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, fileExtension);
                            //fileAbsoluteUri = storageHelper.SaveFiletoStorage(fileData.LocalFileName, uploadFilePath);
                        }
                        else
                            return Content(HttpStatusCode.BadRequest, HttpResponseFormat.Error("Unsupport File Type."));
                    }

                }

                if (fileAbsoluteUri.Equals(""))
                    return Content(HttpStatusCode.BadRequest, HttpResponseFormat.Error("File is empty"));

                //Edit factory logo path
                factoryModel.UpdatePhotoURL(id, fileAbsoluteUri);
                
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
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            FactoryModel factoryModel = new FactoryModel();
            try
            {
                factoryModel.Delete(id);
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
        /// Roles : admin, superadmin
        /// </summary>
        [Route("{factoryId}/Message")]
        public IHttpActionResult GetMessageByFactoryId(int factoryId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                FactoryModel factoryModel = new FactoryModel();
                int companyId = factoryModel.GetByIdForInternal(factoryId).CompanyId;
                CompanyModel companyModel = new CompanyModel();
                CompanyModel.Format_Detail company = companyModel.GetById(companyId);

                var companySubscription = companyModel.GetValidSubscriptionPlanByCompanyId(companyId);
                if (companySubscription == null)
                    throw new Exception("can't find valid subscription plan.");

                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, companySubscription.CosmosDBConnectionString);
                return Ok(docDBHelpler.GetMessageByFactoryId(factoryId, top, hours, order));
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
