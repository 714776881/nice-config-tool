using System;
using System.IO;
using System.Collections.Generic;
using log4net;
using log4net.Repository;
using System.Collections;
using static System.Net.WebRequestMethods;
using System.Diagnostics;

namespace ServiceManager.Tool
{

    public enum E_LogType
    {
        E_LogInfo,
        E_LogDebug,
        E_LogWarn,
        E_LogError,
        E_LogFatal
    }

    public class LogManager
    {
        public static void WriteLog(Type t, E_LogType eLogType, string requestId, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            WriteLog(log, eLogType, requestId, msg);
        }

        public static void WriteLog(string moduleName, E_LogType eLogType, string requestId, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(moduleName);
            WriteLog(log, eLogType, requestId, msg);
        }

        private static void WriteLog(log4net.ILog log, E_LogType eLogType, string requestId, string msg)
        {
            if (!String.IsNullOrEmpty(requestId))
            {
                msg = "[" + requestId + "]" + msg;
            }
            Console.WriteLine(msg);
            switch (eLogType)
            {
                case E_LogType.E_LogInfo:
                    log.Info(msg);
                    break;
                case E_LogType.E_LogDebug:
                    log.Debug(msg);
                    break;
                case E_LogType.E_LogWarn:
                    log.Warn(msg);
                    break;
                case E_LogType.E_LogError:
                    log.Error(msg);
                    break;
                case E_LogType.E_LogFatal:
                    log.Fatal(msg);
                    break;
            }
        }
    }

    public class LogAdapter
    {
        public static void LogError(string msg, string modulename = "")
        {
            Logger.Instance().LogError(msg, modulename);
        }
        public static void LogError(Exception ex, string modulename = "")
        {
            Logger.Instance().LogError(ex, modulename);
        }
        public static void LogError(string msg, Exception ex, string modulename = "")
        {
            Logger.Instance().LogError(msg, ex, modulename);
        }
        public static void LogFatal(string msg, string modulename = "")
        {
            Logger.Instance().LogFatal(msg, modulename);
        }
        public static void LogFatal(string msg, Exception ex, string modulename = "")
        {
            Logger.Instance().LogFatal(msg,ex, modulename);
        }
        public static void LogDebug(string msg, string modulename = "")
        {
            Logger.Instance().LogDebug(msg, modulename);
        }

        public static void LogDebug(string msg, Exception ex, string modulename = "")
        {
            Logger.Instance().LogDebug(msg, ex, modulename);
        }


        public static void LogInfo(string msg, string modulename = "")
        {
            Logger.Instance().LogInfo(msg, modulename);
        }
        public static void LogInfo(string msg, Exception ex, string modulename = "")
        {
            Logger.Instance().LogInfo(msg,ex, modulename);
        }

        public static void LogWarning(string msg, string modulename = "")
        {
            Logger.Instance().LogWarning(msg, modulename);
        }
        public static void LogWarning(string msg, Exception ex, string modulename = "")
        {
            Logger.Instance().LogWarning(msg , ex, modulename);
        }       	
   
    
    }


    class Logger
    {
        private Logger()
        {
            //ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
        }

        private static Logger m_Instance = null;
        
        public static Logger Instance()
        {
            if (m_Instance == null)
            {
                m_Instance = new Logger();
            }
            return m_Instance;
        }

        private Hashtable _LogHt = new Hashtable();
        private ILog GetLog(string modulename)
        {
            try
            {
                if (string.IsNullOrEmpty(modulename))
                {
                    return log4net.LogManager.GetLogger("Default");
                }
                if (_LogHt == null)
                {
                    _LogHt = new Hashtable();
                }
                lock (_LogHt)
                {
                    if (_LogHt.Contains(modulename) == false)
                    {
                        _LogHt.Add(modulename, log4net.LogManager.GetLogger(modulename));
                    }
                }
                return (ILog)_LogHt[modulename];
            }
            catch (Exception ex)
            {
                return log4net.LogManager.GetLogger("NETCoreRepository");
            }
        }

        public void LogError(string msg, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsErrorEnabled)
            {
                logger.Error(msg);
            }
        }
        public void LogError(Exception ex, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsErrorEnabled)
            {
                logger.Error(string.Format("{0}.{1}{2}{3}", ex.Source, ex.TargetSite == null ? "" : ex.TargetSite.Name, " 发生错误: ", ex.Message));
            }
        }
        public void LogError(string msg, Exception ex, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsErrorEnabled)
            {
                logger.Error(string.Format("{0} at {1}.{2}", msg, ex.Source, ex.TargetSite == null ? "" : ex.TargetSite.Name), ex);
            }
        }
        public void LogFatal(string msg, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsFatalEnabled)
            {
                logger.Fatal(msg);
            }
        }
        public  void LogFatal(string msg, Exception ex, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsFatalEnabled)
            {
                logger.Fatal(string.Format("{0} at {1}.{2}", msg, ex.Source, ex.TargetSite == null ? "" : ex.TargetSite.Name), ex);
            }
        }
        public  void LogDebug(string msg, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsDebugEnabled)
            {
                logger.Debug(msg);
            }
        }

        public  void LogDebug(string msg, Exception ex, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsDebugEnabled)
            {
                logger.Debug(string.Format("{0} at {1}.{2}", msg, ex.Source, ex.TargetSite == null ? "" : ex.TargetSite.Name), ex);
            }
        }


        public void LogInfo(string msg, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsInfoEnabled)
            {
                logger.Info(msg);
            }
        }
        public  void LogInfo(string msg, Exception ex, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsInfoEnabled)
            {
                logger.Info(string.Format("{0} at {1}.{2}", msg, ex.Source, ex.TargetSite == null ? "" : ex.TargetSite.Name), ex);
            }
        }

        public  void LogWarning(string msg, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsWarnEnabled)
            {
                logger.Warn(msg);
            }
        }
        public  void LogWarning(string msg, Exception ex, string modulename = "")
        {
            ILog logger = GetLog(modulename);
            if (logger.IsWarnEnabled)
            {
                logger.Warn(string.Format("{0} at {1}.{2}", msg, ex.Source, ex.TargetSite == null ? "" : ex.TargetSite.Name), ex);
            }
        }

    }

}
