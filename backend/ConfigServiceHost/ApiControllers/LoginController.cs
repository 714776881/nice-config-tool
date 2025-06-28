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
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace ConfigServiceHost.ApiControllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpGet]
        [Route("Api/Login")]
        public ApiResponse Get(string userCode, string password)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var loginService = new LoginService();
                var result = loginService.VerifyUser(userCode, password);
                if (result)
                {
                    var userInfo = loginService.GetUserInfo(userCode);
                    userInfo.token = Guid.NewGuid().ToString();

                    SetSessin(userInfo);

                    res.data = userInfo;
                    res.code = ApiResponse.Success;
                }
                else
                {
                    res.code = ApiResponse.Error;
                    res.message = "登录失败，请重新输入！";
                }
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        [Route("Api/Login")]
        public ApiResponse Post([FromBody] LoginForm loginModel)
        {
            ApiResponse res = new ApiResponse();
            try
            {
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

        [HttpPost]
        [Route("Api/Lagout")]
        public ApiResponse Post()
        {
            ApiResponse res = new ApiResponse();
            try
            {
                ClearSession();

                res.code = ApiResponse.Success;
                res.data = true;
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
            HttpContext.Session.SetString("UserId", userInfo.userId);
            HttpContext.Session.SetString("HospitalId", userInfo.hospitalId);
            HttpContext.Session.SetString("RegionId", userInfo.regionId);
            HttpContext.Session.SetString("DepartmentId", userInfo.departmentId);

        }

        private void ClearSession()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("HospitalId");
            HttpContext.Session.Remove("RegionId");
            HttpContext.Session.Remove("DepartmentId");
        }
    }
}
