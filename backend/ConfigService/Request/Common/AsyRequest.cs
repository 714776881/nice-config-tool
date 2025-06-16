using System;
using System.Threading;

namespace ConfigService.Request
{
    class AsyRequest : Request
    {
        public AsyRequest(DC_RequestParam reqParam, int reqType)
            : base(reqParam, reqType)
        {
            ResultObject = null;
        }

        /*
        override public Object Execute()
        {
            Thread t = new Thread(ThreadProcess);
            t.Start(this);
            Thread.Sleep(10000);
            return ResultObject;
        }
         */

        override protected bool ParseParameters()
        {
            return true;
        }

        override protected DC_RequestResult DoWork()
        {
            return null;
        }

        static void ThreadProcess(Object o)
        {
            AsyRequest asyReq = (AsyRequest)o;
            if (null != asyReq)
            {
                asyReq.ResultObject = asyReq.ExcuteByStep();
            }
        }

        private Object ResultObject { get; set; }
    }
}
