
using System.Net.Sockets;
using Tool;
using XViewer.Model.Data;

namespace XService
{
    class XSession
    {
        public XSession(XConnection connection, Socket socket)
        {
            try
            {
                m_Connection = connection;
                m_Socket = socket;
                Buffer = new byte[BufferSize];
            }
            catch (Exception e)
            {
                Log.Instance.Exception("create session error:" + e.Message);
            }
        }

        public void Process()
        {
            Log.Instance.Debug("XSession:Process:begin");
            try
            {
                m_Socket.BeginReceive(Buffer, 0, BufferSize, 0, new AsyncCallback(ReceiveCallback), this);
            }
            catch (Exception e)
            {
                Log.Instance.Exception("XSession:Process m_Socket.BeginReceive failed:" + e.Message);
            }
        }

        void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Log.Instance.Debug("XSession:ReceiveCallback:beign");
                XSession session = (XSession)ar.AsyncState;
                int bytesread = m_Socket.EndReceive(ar);
                if (0 < bytesread)
                {
                    Data.Write(Buffer, 0, bytesread);
                    //取最后四个字节判断是否为包尾，如果是则说明已经收到一个完整的数据包了
                    if (session.IsIntegrity())
                    {
                        m_Connection.Parse(session.Data);
                        session = null;
                        m_Connection.Receive();
                    }
                    else
                    {
                        m_Socket.BeginReceive(Buffer, 0, BufferSize, 0, new AsyncCallback(ReceiveCallback), session);
                    }
                }
                else
                {
                    if (session.IsIntegrity())
                    {
                        m_Connection.Parse(session.Data);
                        session = null;
                        m_Connection.Receive();
                    }
                    else
                    {
                        m_Connection.Close();
                        Log.Instance.Debug("receive data 0 length, close the connection");
                    }
                }
            }
            catch (Exception e)
            {
                //出现异常，关闭连接
                m_Connection.Close();
                Log.Instance.Exception("receive data error:" + e.Message);
            }
        }

        bool IsIntegrity()
        {
            bool ret = false;
            if (8 < Data.Length)
            {
                byte[] temp = new byte[4];
                Data.Seek(-4, SeekOrigin.End);
                Data.Read(temp, 0, 4);
                if ((temp[0] == PacketDelimiter.PacketEnd[0]) && (temp[1] == PacketDelimiter.PacketEnd[1]) && (temp[2] == PacketDelimiter.PacketEnd[2]) && (temp[3] == PacketDelimiter.PacketEnd[3]))
                {
                    ret = true;
                }
            }
            return ret;
        }

        Socket m_Socket = null;
        int BufferSize = 128;
        byte[] Buffer = null;
        XConnection m_Connection = null;
        MemoryStream Data = new MemoryStream();
    }

    class XConnection
    {
        public XConnection(Socket socket, string clientip)
        {
            m_Socket = socket;
            m_ClientIP = clientip;
            m_ClientUID = "";
        }

        public virtual bool IsAsync() { return false; }
        public virtual void Execute() { }
        public virtual void Process(MemoryStream packet) { }

        public void Parse(MemoryStream packet)
        {
            Log.Instance.Debug("XConnection:Parse:beign");
            packet.Seek(4, SeekOrigin.Begin);

            int len = DecodeHelper.DecodeIntTag(packet, Params.PARAMETER_PACKAGE_LENGTH);

            m_ClientUID = DecodeHelper.DecodeStringTag(packet, Params.PARAMETER_TERMINALID);

            Process(packet);
        }

        public void Receive()
        {
            //一次请求抽象成一次会话，一次连接可能会包含多个会话
            XSession session = new XSession(this, m_Socket);
            session.Process();
        }

        public void Response(MemoryStream packet)
        {
            try
            {
                Log.Instance.Debug("start to send response to client:" + m_ClientIP);

                byte[] data = packet.ToArray();
                m_Socket.SendBufferSize = 0;
                int last = data.Length;
                int sent = 0;
                while (0 < last)
                {
                    sent += m_Socket.Send(data, sent, last, SocketFlags.None);
                    last = data.Length - sent;
                }
                Log.Instance.Debug("send response to client:" + m_ClientIP + " success!");
            }
            catch (Exception e)
            {
                Log.Instance.Exception("send response to client error:" + e.Message);
            }
        }

        //终端关闭连接时服务器才关闭，以支持长连接
        public virtual void Close()
        {
            try
            {
                if (null != m_Socket)
                {
                    m_Socket.Shutdown(SocketShutdown.Both);
                    m_Socket.Close();
                    m_Socket = null;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Exception("close connection error:" + e.Message);
            }
        }

        public string ClientIP() { return m_ClientIP; }
        public string ClientUID() { return m_ClientUID; }

        protected string m_ClientIP = null;
        protected string m_ClientUID = null;
        protected Socket m_Socket = null;
    }
}
