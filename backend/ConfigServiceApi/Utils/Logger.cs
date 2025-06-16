using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ConfigServiceApi.Utils
{
    public class Logger : IDisposable
    {
        private string logPath = string.Empty;
        private FileStream fs = null;
        private StreamWriter sw = null;

        /// <summary>
        /// 日志级别0--Error;>1--infor;
        /// </summary>
        public static int LogLevel = 0;
        private static Logger instance;
        private static object o = new object();

        private Logger()
        {
            try
            {
                LogLevel = int.Parse(ConfigHelper.GetSetting("LogLevel", "1"));
                CreateLog();
            }
            catch (System.Exception es)
            {
                Console.WriteLine(es.Message);
            }
        }

        private void CreateLog()
        {
            string dicPath = ConfigHelper.GetSetting("LogPath","");
            if (string.IsNullOrEmpty(dicPath))
            {
                dicPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Log";
            }
            string fileName = string.Format(@"\Tool_{0}.log", DateTime.Now.ToString("yyyyMMddHH"));
            logPath = string.Format(@"{0}{1}", dicPath, fileName);
            if (!Directory.Exists(dicPath))
            {
                Directory.CreateDirectory(dicPath);
            }
            if (!File.Exists(logPath))
            {
                using (fs = new FileStream(logPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (sw = new StreamWriter(fs))
                    {
                        sw.WriteLine("创建日志:" + (DateTime.Now.ToString()));
                        sw.Flush();
                    }
                }
            }
        }

        private static Logger GetInstance()
        {
            if (instance == null)
            {
                lock (o)
                {
                    if (instance == null)
                    {
                        instance = new Logger();
                    }
                }
            }
            return instance;
        }
        public static void LogError(string msg, Exception e)
        {
            instance = GetInstance();
            if (instance != null)
            {
                instance.XLogError(msg + "\n" + e.Message + "\n" + e.StackTrace);
            }
        }

        public static void LogError(Exception e)
        {
            instance = GetInstance();
            if (instance != null)
            {
                instance.XLogError(e);
            }
        }

        public static void LogError(string errormsg)
        {
            instance = GetInstance();
            if (instance != null)
            {
                instance.XLogError(errormsg);
            }
        }

        public static void LogInfo(string infomsg)
        {
            instance = GetInstance();
            if (instance != null)
            {
                instance.XLogInfo(infomsg);
            }
        }

        private void XLogInfo(string message)
        {
            try
            {
                if (LogLevel < 1)
                {
                    return;
                }
                CreateLog();
                using (fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(String.Format("{0}  Info-{1}", DateTime.Now.ToString(), message));
                        sw.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void XLogError(Exception ex)
        {
            XLogError(ex.Message + "&&" + ex.StackTrace);
        }
        private void XLogError(string errormsg)
        {
            try
            {
                CreateLog();
                using (fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(String.Format("{0}  Error-{1}", DateTime.Now.ToString(), errormsg));
                        sw.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (fs != null && sw != null)
            {
                try
                {
                    sw.Close();
                    fs.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        #endregion
    }
}
