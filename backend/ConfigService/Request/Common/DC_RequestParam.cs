using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigService.Request
{
    public class DC_RequestParam
    {
        public DC_RequestParam()
        {
            IsAppendedRequestParam = false;
            WaitTime = 2000;
        }
        public string RequestID { get; set; }
        public string TerminalID { get; set; }      //终端名称
        public string RequestParam { get; set; }    //请求参数       
        public int WaitTime { get; set; }
        public int MaxResultNum { get; set; }           // 结果集最大条数
        public bool IsAppendedRequestParam { get; set; }
        public string UserID { get; set; }
    }

    public class RequestParamBase
    {
        public RequestParamBase()
        {

        }
        public string TerminalID { get; set; }      //终端名称
        public string TerminalType { get; set; }    //终端类型(带版本)
        public string TerminalIP { get; set; }      //终端IP
        public string TerminalMac { get; set; }     //终端MAC
        public string UserSessionID { get; set; }    //用户SessionID（当前登录有效）
        public string UserID { get; set; }          //操作者ID
        public string UserName { get; set; }         //操作者名字
        public string UserRole { get; set; }         //操作者权限
        public string DepartmentID { get; set; }     //部门ID
        public string DepartmentName { get; set; }     //部门名字
        public string UserHospitalID { get; set; }      //用户的医院ID
        public string UserHospitalName { get; set; }    //
        public string UserRegionID { get; set; }      //用户的区域ID
        public string UserRegionName { get; set; }      //用户的区域
        public string DepartmentCategory { get; set; }  // //科室类型 
        public string Language { get; set; }       //客户端的语言
        public List<string> DataSourceList { get; set; }   //数据源列表

    }

}
