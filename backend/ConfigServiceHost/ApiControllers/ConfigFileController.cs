using ConfigService.DataModel;
using ConfigService.Request;
using ConfigServiceApi;
using Microsoft.AspNetCore.Mvc;
using ServiceManager.Tool;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Drawing;

using ConfigServiceApi.Utils;
using ConfigServiceApi.Models;
using ConfigServiceApi.Services;
using Tool;

namespace ConfigServiceHost.ApiControllers
{
    [ApiController]
    public class ConfigFileController : ControllerBase
    {

        [HttpGet]
        [Route("Api/ConfigFile")]
        public ApiResponse Get(string fileName)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var fileContent = ConfigFileService.GetConfig(fileName);

                if(string.IsNullOrEmpty(fileContent))
                {
                    throw new Exception("文件不存在");
                }

                res.code = ApiResponse.Success;
                res.data = fileContent;
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        [Route("Api/ConfigFile")]
        public ApiResponse Post(string fileName, [FromBody] string fileContent)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var result = ConfigFileService.PostConfig(fileName, fileContent);

                res.code = ApiResponse.Success;
                res.data = result;
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }
    }
}
