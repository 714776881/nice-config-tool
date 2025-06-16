using ConfigService.DataModel;
using ConfigService.Request;
using ConfigServiceApi;
using Microsoft.AspNetCore.Mvc;
using ServiceManager.Tool;
using System.Reflection;
using System.Text.RegularExpressions;
using Tool;
using System.Drawing;

using ConfigServiceApi.Models;
using Microsoft.AspNetCore.Identity;


namespace ConfigServiceHost.ApiControllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [Route("Api/Login")]
        public ApiResponse Post([FromBody] LoginForm loginModel)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                
                if (ConfigHelper.GetSetting("RunMode", "Release") == "Debug")
                {
                    var userInfo_debug = new LoginVO();
                    userInfo_debug.userCode = "9011";
                    userInfo_debug.userId = "9011";
                    userInfo_debug.userName = "李白";
                    userInfo_debug.userActor = "9011";
                    userInfo_debug.userRole = "9011";
                    userInfo_debug.regionId = "100";
                    userInfo_debug.hospitalId = "104";
                    userInfo_debug.departmentId = "102";
                    userInfo_debug.token = Guid.NewGuid().ToString();

                    SetSessin(userInfo_debug);
                    res.code = ApiResponse.Success;
                    res.data = userInfo_debug;
                    return res;
                }

                if (loginModel == null || loginModel.loginToken == null)
                {
                    res.code = ApiResponse.Error;
                    res.message = "验证码为空！";
                    return res;
                }

                var loginService = new LoginService();
                var userInfo = loginService.GetUserInfo(loginModel);
                userInfo.token = Guid.NewGuid().ToString();

                SetSessin(userInfo);
                res.code = ApiResponse.Success;
                res.data = userInfo;
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        private void SetSessin(LoginVO userInfo)
        {
            HttpContext.Session.SetString("Token", userInfo.token);
            HttpContext.Session.SetString("UserId", userInfo.userId);
            HttpContext.Session.SetString("HospitalId", userInfo.hospitalId);
            HttpContext.Session.SetString("RegionId", userInfo.regionId);
            HttpContext.Session.SetString("DepartmentId", userInfo.departmentId);
        }
    }
}
