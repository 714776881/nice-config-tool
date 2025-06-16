
using System.Net.Sockets;
using System.Net;
using Tool;

namespace XService
{
    class XServiceWorker : IServiceHost
    {
        //threadcount:指日志服务线程池里的线程个数
        public XServiceWorker(XConnectionCreator connectioncreator, string ip, int port)
            : base(port)
        {
            m_ConnectionCreator = connectioncreator;
            m_ListenIP = ip;
        }

        //启动服务
        public override bool Start()
        {
            bool ret = true;

            try
            {
                m_Running = true;
                m_Thread = new Thread(this.Listen);
                m_Thread.Name = "monitor thread";
                m_Thread.IsBackground = true;
                m_Thread.Start();
            }
            catch (Exception e)
            {
                ret = false;
                Log.Instance.Exception("start service error:" + e.Message);
            }

            return ret;
        }
        //停止服务
        public override void Stop()
        {
            try
            {
                m_Running = false;
                //模拟一个连接请求，优雅退出线程
                MemoryStream exitcmd = new MemoryStream();
                byte[] bexitcmd = new byte[1];
                exitcmd.Write(bexitcmd, 0, 1);
                IPAddress address = IPAddress.Parse("127.0.0.1");
                IPEndPoint server = new IPEndPoint(address, m_ListenPort);

                TcpClient client = new TcpClient();
                client.Connect(server);
                NetworkStream ns = client.GetStream();
                exitcmd.WriteTo(ns);
                ns.Flush();
                //结束日志线程
                Log.Instance.Stop();
            }
            catch (Exception e)
            {
                Log.Instance.Exception("stop monitor error:" + e.Message);
            }
        }

        void Listen()
        {
            TcpListener listener = null;
            try
            {
                IPEndPoint point = new IPEndPoint(IPAddress.Parse(m_ListenIP), m_ListenPort);
                listener = new TcpListener(point);
                listener.Start();

                while (m_Running)
                {
                    Socket socket = listener.AcceptSocket(); 

                    char[] seps = { ':' };
                    string temp = socket.LocalEndPoint.ToString();
                    string[] splits = temp.Split(seps);
                    string localip = splits[0];

                    temp = socket.RemoteEndPoint.ToString();
                    splits = temp.Split(seps);
                    string clientip = splits[0];

                    if ((clientip == localip) && (1 == socket.Available))
                    {
                        Log.Instance.Normal("exit friendly");
                    }
                    else
                    {
                        Log.Instance.Debug("receive:" + socket.RemoteEndPoint.ToString() + "'s request");

                        XConnectionManager.Instance.Add(m_ConnectionCreator.Create(socket, clientip));
                    }
                }
                listener.Stop();
            }
            catch (Exception e)
            {
                if (null != listener)
                {
                    listener.Stop();
                }
                Log.Instance.Exception("listen failed, service will exit:" + e.Message);
            }
        }

        string m_ListenIP = null;
        bool m_Running = false;
        Thread m_Thread = null;
        XConnectionCreator m_ConnectionCreator = null;
    }
}
