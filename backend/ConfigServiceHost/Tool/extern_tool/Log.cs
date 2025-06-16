
namespace Tool
{
    class Log
    {
        //日志等级
        public enum Level
        {
            NoLog = 0,
            Audit,//审计，为法规注册提供信息，比如说提交到区域平台中的日志
            Normal, //常规级别，记录系统中发生的关键事件和时间        
            Debug,//调试级别，尽可能详细的记录，若程序运行出了问题，根据这个级别的日志应能定位出问题原因        
            TimeCheck,//性能测试，当需要测试性能时使用此档位
        }

        public static Log Instance
        {
            get
            {
                return m_Instance;
            }
        }

        //设置日志保存策略，保存在本地还是日志服务器
        public void SetSaveMode(string logserverip = "", int logserverport = 0)
        {
            if ((null != logserverip) && (0 < logserverip.Length) && (0 < logserverport))
            {
                m_SaveMode = new RemoteServerSaveMode(logserverip, logserverport);//写到远程日志服务上
            }
            else
            {
                m_SaveMode = new LocalFileSaveMode(Keeps);//默认写到本地文件里
            }
        }

        public void Normal(string strlog)
        {
            Write(Level.Normal, strlog);
        }

        public void Error(string log)
        {
            string strlog = "Error:" + log;
            Write(Level.Normal, strlog);
        }

        public void Exception(string strlog)
        {
            string log = "Exception:" + strlog;
            Normal(log);
        }

        public void Debug(string strlog)
        {
            Write(Level.Debug, strlog);
        }

        public void Write(Level level, string strlog)
        {
            Console.WriteLine(strlog);
            if (m_Level < level)
            {
                return;
            }
            if ((null == strlog) || (0 == strlog.Length))
            {
                return;
            }
            //加时间戳和线程ID
            string log = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff") + ":" + "TID=" + Environment.CurrentManagedThreadId.ToString() + "," + strlog + "\r\n";
            AddLog(log);
        }

        public int Keeps { get; set; }
        public void SetLevel(int level)
        {
            m_Level = (Level)level;
        }

        public void Stop()
        {
            Monitor.Enter(m_Contents);
            m_Contents.Add("exit");
            m_Running = false;
            Monitor.Pulse(m_Contents);
            Monitor.Exit(m_Contents);
        }

        void AddLog(string log)
        {
            if (true == m_Running)
            {
                Monitor.Enter(m_Contents);
                m_Contents.Add(log);
                Monitor.Pulse(m_Contents);
                Monitor.Exit(m_Contents);
            }
        }

        private bool GetRequest(ref string str_log)
        {
            bool b_ret = false;

            Monitor.Enter(m_Contents);
            if (0 == m_Contents.Count)
            {
                Monitor.Wait(m_Contents);
            }
            if (0 < m_Contents.Count)
            {
                str_log = m_Contents[0];
                m_Contents.RemoveAt(0);
                b_ret = true;
            }
            Monitor.Pulse(m_Contents);
            Monitor.Exit(m_Contents);

            return b_ret;
        }

        Log()
        {
            try
            {
                m_Running = true;
                Keeps = 7;
                m_Thread = new Thread(this.Do)
                {
                    Name = "log service thread",
                    IsBackground = true
                };
                m_Thread.Start();
            }
            catch (Exception)
            {

            }
        }

        void Do()
        {
            string str_log = "";
            while (true)
            {
                if (GetRequest(ref str_log))
                {
                    if ("exit" == str_log)
                    {
                        break;
                    }
                    else
                    {
                        WriteLog(str_log);
                    }
                }
            }
            //保存未保存的日志
            for (int i = 0; i < m_Contents.Count; ++i)
            {
                WriteLog(m_Contents[i]);
            }
            m_Contents.Clear();
        }

        void WriteLog(string str_log)
        {
            if (null != m_SaveMode)
            {
                m_SaveMode.Save(str_log);
            }
        }

        private bool m_Running = false;
        private Level m_Level = Level.Normal;
        private LogSaveMode? m_SaveMode = null;
        private List<string> m_Contents = new(1);
        private Thread? m_Thread = null;
        private static Log m_Instance = new();
    }
}
