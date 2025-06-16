using ConfigService.DataModel;
using ConfigService.Request;
using Microsoft.AspNetCore.Mvc;
using ServiceManager.Tool;
using System.Text.RegularExpressions;

namespace ConfigServiceHost.Controllers
{
    [ApiController]
    public class ConfigController : ControllerBase
    {
        #region 获取配置信息

        [HttpPost]
        [Route("WSConfigService/XmlConfig")]
        public DC_RequestResult ConfigSettingLoadByPost(DC_RequestParam param)
        {
            LogAdapter.LogDebug(string.Format("Receive Request ConfigSettingLoadByPost. RequestID:{0}  ", param.RequestID));
            DC_RequestResult retObj = RequestManager.GetInstance().ExecuteRequest(param,
                (int)ERequestType.R_GetXmlConfig);
            LogAdapter.LogDebug(string.Format("End Request ConfigSettingLoadByPost. RequestID:{0}  ", param.RequestID));
            return retObj;
        }
        #endregion

    }
}