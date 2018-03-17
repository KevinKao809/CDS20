using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;
using sfShareLib;
using sfAPIService.Models;
using System.Threading.Tasks;
using sfAPIService.Filter;
using CDSShareLib.Helper;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "device")]
    [RoutePrefix("device-api")]
    public class DeviceApiController : ApiController
    {
        /// <summary>
        /// Roles : device
        /// </summary>
        [HttpGet]
        [Route("Device/{id}")]
        public async Task<IHttpActionResult> GetDeviceById(int id)
        {
            string logAPI = "[Get] " + Request.RequestUri.ToString();
            
            try
            {
                DeviceModels deviceModel = new DeviceModels();
                return Ok(await deviceModel.GetIoTDeviceByDeviceId(id));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "404":
                        return NotFound();
                }

                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Roles : device
        /// </summary>
        [HttpPost]
        [Route("Device/{id}/Log")]
        public async Task<HttpResponseMessage> UploadPhotoFile(int id)
        {
            try
            {
                DeviceUtility deviceHelper = new DeviceUtility();

                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

                DeviceModels deviceModel = new DeviceModels();
                DeviceModels.Detail iotDevice = await deviceModel.GetIoTDeviceByDeviceId(id);

                //FileHelper fileHelper = new FileHelper();
                BlobStorageHelper storageHelper = new BlobStorageHelper(Global._systemStorageName, Global._systemStorageKey, Global._imageStorageContainer);
                string root = Path.GetTempPath();
                var provider = new MultipartFormDataStreamProvider(root);

                // Read the form data.
                string fileAbsoluteUri = "";
                await Request.Content.ReadAsMultipartAsync(provider);
                long logStartTimestamp = 0;

                //FormData
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        if (key.ToLower().ToString() == "startts")
                            logStartTimestamp = long.Parse(val);
                    }
                }

                //FileData
                foreach (MultipartFileData fileData in provider.FileData)
                {
                    string formColumnName = fileData.Headers.ContentDisposition.Name.ToLower();
                    string fileExtenionName = Path.GetExtension(fileData.Headers.ContentDisposition.FileName.Replace("\"", "").ToLower());
                    if (formColumnName.Equals("filename"))
                    {
                        DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
                        IoTDevice dbIoTDevice = dbhelp.GetByid(id);
                        DateTime logStartDT = deviceHelper.GetSpecifyTimeZoneDateTimeByTimeStamp(logStartTimestamp, dbIoTDevice.Factory.TimeZone);

                        string uploadFilePath = logStartDT.ToString("yyyy/MM/dd") + "/" + dbIoTDevice.IoTHubDeviceID + "/" + logStartTimestamp + fileExtenionName;
                        fileAbsoluteUri = storageHelper.SaveFiletoStorage(fileData.LocalFileName, uploadFilePath);
                    }
                }

                if (fileAbsoluteUri.Equals(""))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File is empty");

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception ex)
            {
                string logAPI = "[Post] " + Request.RequestUri.ToString();

                switch (ex.Message)
                {
                    case "404":
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                Global._appLogger.Error(logAPI + logMessage);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }                        
        }

        /// <summary>
        /// Roles : device
        /// </summary>
        //[HttpGet]
        //[Route("Device/{id}/MessageSchema")]
        //public IHttpActionResult GetAllMessageSchemaByDeviceId(int id)
        //{
        //    IoTDeviceMessageCatalogModels iotDMCModels = new IoTDeviceMessageCatalogModels();
        //    MessageCatalogModels msgCatalogModels = new MessageCatalogModels();

        //    List<object> objectList = new List<object>();
        //    var msgCatalogs = iotDMCModels.GetAllMessageCatalogByIoTDeviceId(id);
        //    foreach (var msgCatalog in msgCatalogs)
        //    {
        //        objectList.Add(msgCatalogModels.GetMessageCatalogElementSchema(msgCatalog.MessageCatalogId));
        //    }
        //    return Ok(objectList);
        //}

    }
}
