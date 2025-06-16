
using Tool;

namespace XService
{
    //一次连接可以包含多次会话，即多次请求，如何实现?也就是如何实现长连接?
    class XExecuter
    {
        public XExecuter(int id)
        {
            try
            {
                m_Thread = new Thread(this.Do);
                m_Thread.Name = "executer" + id.ToString();
                m_Thread.IsBackground = true;
                m_Thread.Start();
            }
            catch (Exception e)
            {
                Log.Instance.Exception("create executer error:" + e.Message);
            }
        }

        public void Accept(XConnection con)
        {
            Monitor.Enter(m_Connections);
            m_Connections.Add(con);
            Monitor.Pulse(m_Connections);
            Monitor.Exit(m_Connections);
        }

        bool Fetch(ref XConnection con)
        {
            bool b_ret = false;

            Monitor.Enter(m_Connections);
            if (0 == m_Connections.Count)
            {
                Monitor.Wait(m_Connections);
            }
            if (0 < m_Connections.Count)
            {
                con = m_Connections[0];
                m_Connections.RemoveAt(0);
                b_ret = true;
            }
            Monitor.Pulse(m_Connections);
            Monitor.Exit(m_Connections);

            return b_ret;
        }

        void Do()
        {
            XConnection con = null;
            while (true)
            {
                if (Fetch(ref con))
                {
                    con.Execute();
                    Log.Instance.Debug("executer exit after connection closed");
                    break;//连接关闭，执行器也就结束
                }
            }
        }

        private Thread m_Thread = null;
        List<XConnection> m_Connections = new List<XConnection>(0);
    }
}
