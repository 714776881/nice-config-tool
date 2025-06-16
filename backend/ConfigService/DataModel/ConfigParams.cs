using ConfigService.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigService.DataModel
{
    public enum EConfigType
    {
        E_GeneralConfig = 0,
        E_DataSourceConfig = 1,
        E_DepartmentConfig = 2,
        E_TerminalConfig = 3,
        E_PersionalConfig = 4,
        E_AuthorityConfig = 5,
        E_DataSourceConfigRIS = 6
    }

    public enum ETerminalType
    {
        E_Desktop = 0,    // 桌面版
        E_Web = 1,        // 网页版
        E_Mobile = 2,     // 移动端
        E_Ris = 3,        // Ris终端
        E_Techstation = 4,    // 技师工作站
    }

    public class ConfigParams : RequestParamBase
    {
        public ConfigParams()
        {
            DataSourceList = new List<string>();
            TerminalID = string.Empty;
            TerminalType = string.Empty;
            TerminalIP = string.Empty;
            TerminalMac = string.Empty;
        }
        public EConfigType ECfgType { get; set; }
        public ETerminalType ETerminalType { get; set; }
        public string UserInfo { get; set; }
        public bool BDefaultConfig { get; set; }
    }

}