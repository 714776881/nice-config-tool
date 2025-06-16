using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigService.Request
{
    public class RequestFactory : IRequestFactory
    {

        public RequestFactory()
        {
        }

        public Request Create(int reqType, DC_RequestParam reqParam)
        {
            Request request = null;

            ERequestType requestType = (ERequestType)reqType;
            switch (requestType)
            {
                case ERequestType.R_GetXmlConfig:
                    request = new R_GetXmlConfig(reqParam);
                    break;
                case ERequestType.R_SaveXmlConfig:
                    break;
            }

            return request;
        }
    }
}