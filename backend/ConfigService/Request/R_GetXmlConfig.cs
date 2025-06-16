using ConfigService.Business;
using ConfigService.DataModel;
using Microsoft.AspNetCore.Http;
using ServiceManager.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigService.Request
{
    class R_GetXmlConfig : SynRequest
    {
        private ConfigParams cfgParam { get; set; }
        public R_GetXmlConfig(DC_RequestParam reqParam)
          : base(reqParam, (int)ERequestType.R_GetXmlConfig) //
        {
            IsCompleteFinished = false;
        }

        override protected bool ParseParameters()
        {
            try
            {
                bool bRet = false;
                if (null != ObjectRequestParam && !string.IsNullOrEmpty(ObjectRequestParam.RequestParam))
                {
                    cfgParam = JsonTool.Parse<ConfigParams>(ObjectRequestParam.RequestParam);
                    bRet = true;
                }
                return bRet;
            }
            catch (Exception ex)
            {
                LogAdapter.LogError(string.Format("[EXCEPTION]请求参数异常:{0}", ex.Message), ex);
                return false;
            }
        }

        override protected DC_RequestResult DoWork()
        {
            string errInfo = "";
            //EConfigType cfgType = (EConfigType)cfgParam.ECfgType;
            //ETerminalType terType = (ETerminalType)cfgParam.ETerminalType;
            string strConfig = ConfigCenter.Instance.GetConfig(cfgParam.ECfgType, cfgParam.ETerminalType, cfgParam.DepartmentName, cfgParam.UserInfo, cfgParam.TerminalIP, cfgParam.BDefaultConfig);
            return new DC_RequestResult(strConfig, !string.IsNullOrEmpty(strConfig), false, false, errInfo);
        }
    }
}