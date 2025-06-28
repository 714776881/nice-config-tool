using ConfigService.DataModel;
using ConfigService.Request;
using ConfigServiceApi;
using Microsoft.AspNetCore.Mvc;
using ServiceManager.Tool;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

using ConfigServiceApi.Models;
using ConfigServiceApi.Utils;
using ConfigServiceApi.Services;

namespace ConfigServiceHost.ApiControllers
{
    public class ScopeController : ControllerBase
    {

        [HttpGet]
        [Route("Api/Scope")]
        public ApiResponse GetAll()
        {
            var response = new ApiResponse();
            try
            {
                var service = new ScopeService();

                var userId = HttpContext.Session.GetString("UserId");
                var regionId = HttpContext.Session.GetString("RegionId");
                var hospitalId = HttpContext.Session.GetString("HospitalId");
                var departmentId = HttpContext.Session.GetString("DepartmentId");
                
                if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(regionId) || string.IsNullOrEmpty(hospitalId) || string.IsNullOrEmpty(departmentId))
                {
                    response.code = ApiResponse.Error;
                    response.message = "Session为空！";
                    return response;
                }
                
                response.data = service.GetOptions(userId, regionId, hospitalId, departmentId);
                response.code = ApiResponse.Success;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                response.message = ex.Message;
                response.code = ApiResponse.Error;
            }
            return response;
        }
    }
}
