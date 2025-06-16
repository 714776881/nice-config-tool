using System;
using System.Collections.Generic;
using ServiceManager.Tool;
using System.Collections;

namespace ConfigService.Request
{
    public class RequestManager
    {
        public static RequestManager GetInstance()
        {
            if (null == Instance)
            {
                Instance = new RequestManager();
            }
            return Instance;
        }

        public DC_RequestResult ExecuteRequest(DC_RequestParam reqParam, int reqType)
        {
            DC_RequestResult retObj = null;
            Request request = null;
            RegisterRequest(reqParam, reqType, out request);
            if (null != request)
            {
                retObj = request.Execute();
                UnRegisterRequest(request);
            }
            return retObj;
        }
        private void RegisterRequest(DC_RequestParam reqParam, int reqType, out Request request)
        {
            request = null;
            if (null != reqParam)
            {
                lock (TerminalRequestDict)
                {
                    try
                    {
                        Dictionary<string, Request> terminalReqs = null;
                        if (true == TerminalRequestDict.TryGetValue(reqParam.TerminalID, out terminalReqs))
                        {
                            if (false == terminalReqs.TryGetValue(reqParam.RequestID, out request))
                            {
                                request = GenerateRequest(reqParam, reqType);
                                terminalReqs.Add(request.RequestID, request);
                            }
                            else
                            {
                                request.RegisterAppendedParameter(reqParam);
                            }
                        }
                        else
                        {
                            request = GenerateRequest(reqParam, reqType);
                            terminalReqs = new Dictionary<string, Request>();
                            terminalReqs.Add(request.RequestID, request);
                            TerminalRequestDict.Add(request.TerminalID, terminalReqs);
                        }
                    }
                    catch (System.Exception ex)
                    {
                    	
                    }
                }
            }
        }

        private void UnRegisterRequest(Request request)
        {
            if (null != request && true == request.IsCompleteFinished)
            {
                lock (TerminalRequestDict)
                {
                    Dictionary<string, Request> terReq = null;
                    if (true == TerminalRequestDict.TryGetValue(request.TerminalID, out terReq))
                    {
                        terReq.Remove(request.RequestID);
                    }
                    else
                    {
                        // ErrorInfo
                    }
                }
            }
        }

        private Request GenerateRequest(DC_RequestParam reqParam, int reqType)
        {
            Request request = null;
            if (null != RequestFactory)
            {
                request = RequestFactory.Create(reqType, reqParam);
            }

            return request;
        }

        private RequestManager()
        {
            TerminalRequestDict = new Dictionary<string, Dictionary<string, Request>>();
        }

        public IRequestFactory RequestFactory { get; set; }
        private Dictionary<string, Dictionary<string, Request>> TerminalRequestDict { get; set; }
        private static RequestManager Instance { get; set; }
    }
}
