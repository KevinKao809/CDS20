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
using Swashbuckle.Swagger.Annotations;
using Newtonsoft.Json;
using CDSShareLib.Helper;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
    [RoutePrefix("admin-api/Dashboard")]
    public class DashboardController : ApiController
    {
        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        /// <param name="type">factory, equipmentclass, Default factory</param>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DashboardModel.Format_DetailForFactory>))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DashboardModel.Format_DetailForEquipmentClassBoard>))]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin")]
        public IHttpActionResult GetAllDashboard([FromUri]string type = "factory")
        {
            int companyId = Global.GetCompanyIdFromToken();
            DashboardModel model = new DashboardModel();
            switch (type)
            {
                case "factory":
                    return Content(HttpStatusCode.OK, model.GetAllFactoryByCompanyId(companyId));
                case "equipmentclass":
                    return Content(HttpStatusCode.OK, model.GetAllEquipmentClassBoardByCompanyId(companyId));
            }

            return Content(HttpStatusCode.OK, "");
        }

        /// <summary>
        /// Client_Id : Admin - OK
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DashboardModel.Format_Detail))]
        public IHttpActionResult GetDashboardById(int id)
        {
            try
            {
                DashboardModel model = new DashboardModel();
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
        public IHttpActionResult CreateDashboard([FromBody]DashboardModel.Format_Create dataModel)
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
                DashboardModel model = new DashboardModel();
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
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteDashboard(int id)
        {
            try
            {
                DashboardModel model = new DashboardModel();
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
