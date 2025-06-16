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
using ConfigServiceApi.Utils;
using ConfigServiceApi.Services;


namespace ConfigServiceHost.ApiControllers
{
    [ApiController]
    public class XmlFileController : ControllerBase   
    {
        [HttpGet]
        [Route("Api/XmlFile")]
        public ApiResponse Get(string filePath, string nodePath)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                if(string.IsNullOrEmpty(filePath))
                {
                    res.code = ApiResponse.Error;
                    res.message = "值为空！";
                    return res;
                }

                var result = RisConfigService.GetFileNodeValue(filePath, nodePath);

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
        [HttpPost]
        [Route("Api/XmlFile")]
        public ApiResponse Post(string filePath, string nodePath, [FromBody] NodeValueModel nodeValueModel)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    res.code = ApiResponse.Error;
                    res.message = "值为空！";
                    return res;
                }

                var result = RisConfigService.PostFileNodeValue(filePath.Trim(), nodePath.Trim(), nodeValueModel.NodeValue);

                res.code = ApiResponse.Success;
                res.data = result;
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.ToString();
            }
            return res;
        }
    }
}
