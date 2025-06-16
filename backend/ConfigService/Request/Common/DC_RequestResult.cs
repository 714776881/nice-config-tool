using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ConfigService.Request
{
    [DataContract]
    public class DC_RequestResult
    {
        [DataMember]
        public bool IsSuccessful { get; set; }
        [DataMember]
        public bool IsExceedTime { get; set; }
        [DataMember]
        public bool IsNotAllowed { get; set; }
        [DataMember]
        public string ErrorInfo { get; set; }
        [DataMember]
        public Object ResultObject { get; set; }
        public DC_RequestResult(Object retObj, bool bSuccess = true, bool bExceedTime = false, bool bNotAllowed = false, string errorInfo = "")
        {
            ResultObject = retObj;
            IsSuccessful = retObj != null ? bSuccess : false;
            IsExceedTime = bExceedTime;
            IsNotAllowed = bNotAllowed;
            ErrorInfo = errorInfo;
        }

    }
}
