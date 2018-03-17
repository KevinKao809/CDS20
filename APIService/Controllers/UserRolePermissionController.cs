using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

using System.Text;
using sfShareLib;
using sfAPIService.Models;
namespace sfAPIService.Controllers
{
    /*
    [Authorize]
    [RoutePrefix("admin-api/UserRolePermission")]
    public class UserRolePermissionController : ApiController
    {
        [HttpGet]
        [Route("UserRole/{userRoleId}")]
        public IHttpActionResult GetAllUserRolePermissionsByUserRoleId(int userRoleId)
        {
            UserRolePermissionModels userRolePermissionModel = new Models.UserRolePermissionModels();
            return Ok(userRolePermissionModel.GetAllUserRolePermissionByUserRoleId(userRoleId));
        }

        
        [HttpGet]
        public IHttpActionResult GetUserRolePermissionById(int id)
        {
            try
            {
                UserRolePermissionModels userRolePermissionModel = new UserRolePermissionModels();
                return Ok(userRolePermissionModel.getUserRolePermissionById(id));
            }
            catch
            {
                return NotFound();
            }
        }
        

        [HttpPost]
        public IHttpActionResult AddFormData([FromBody]UserRolePermissionModels.Edit userRolePermission)
        {
            string logForm = "Form : " + Global._jsSerializer.Serialize(userRolePermission);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || userRolePermission == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                UserRolePermissionModels userRolePermissionModel = new UserRolePermissionModels();
                userRolePermissionModel.addUserRolePermission(userRolePermission);
                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPut]
        public IHttpActionResult EditFormData(int id, [FromBody] UserRolePermissionModels.Edit userRolePermission)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(userRolePermission);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || userRolePermission == null)
            {
                Global._appLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                UserRolePermissionModels userRolePermissionModel = new UserRolePermissionModels();
                userRolePermissionModel.updateUserRolePermission(id, userRolePermission);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Global._appLogger.Error(logAPI + logMessage);

                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                UserRolePermissionModels userRolePermissionModel = new UserRolePermissionModels();
                userRolePermissionModel.deleteUserRolePermission(id);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                string logAPI = "[Delete] " + Request.RequestUri.ToString();
                StringBuilder logMessage = LogHelper.BuildExceptionMessage(ex);
                Global._appLogger.Error(logAPI + logMessage);
                return InternalServerError();
            }
        }
    }
    */
}
