using System;

namespace ConfigService.Request
{
    public class Request
    {
        public Request(DC_RequestParam reqParam, int reqType)
        {
            ObjectRequestParam = reqParam;
            RequestType = reqType;
            TerminalID = reqParam.TerminalID;
            RequestID = reqParam.RequestID;
            IsCompleteFinished = true;
        }

        public void RegisterAppendedParameter(DC_RequestParam reqParam)
        {
            //ObjectRequestParam.MaxResultNum = reqParam.MaxResultNum;
            //ObjectRequestParam.IsAppendedRequestParam = true;
        }

        public virtual DC_RequestResult Execute()
        {
            return ExcuteByStep();
        }

        protected DC_RequestResult ExcuteByStep()
        {
            DC_RequestResult resObj = null;
            if (true == ParseParameters())
            {
                resObj = DoWork();
            }
            Response();

            return resObj;
        }

        protected virtual bool ParseParameters()
        {
            return true;
        }

        protected virtual DC_RequestResult DoWork()
        {
            return null;
        }

        protected virtual void Response()
        {
            //if (WebOperationContext.Current != null && WebOperationContext.Current.OutgoingResponse != null)
            //{
            //    WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            //    WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
            //    WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Max-Age", "1000");
            //    WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Type", "application/json");
            //}
        }

        public string RequestID { get; set; }
        public string TerminalID { get; set; }
        public bool IsCompleteFinished { get; set; }
        public int RequestType { get; set; }
        protected DC_RequestParam ObjectRequestParam { get; set; }
    }
}
