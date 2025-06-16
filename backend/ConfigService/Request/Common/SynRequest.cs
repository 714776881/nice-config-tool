using System;

namespace ConfigService.Request
{
    public class SynRequest : Request
    {
        public SynRequest(DC_RequestParam reqParam, int reqType)
            : base(reqParam, reqType)
        { 
        }

        override protected bool ParseParameters()
        {
            return true;
        }

        override protected DC_RequestResult DoWork()
        {
            return null;
        }
    }
}
