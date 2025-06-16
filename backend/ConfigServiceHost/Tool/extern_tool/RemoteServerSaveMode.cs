using System.Net.Sockets;
using System.Net;
using XViewer.Model.Data;
using Model.Config;

namespace Tool
{
    public class RemoteServerSaveMode : LogSaveMode
    {
        public RemoteServerSaveMode(string ip, int port)
        {
            m_ServerPort = port;
            m_ServerIP = ip;
        }

        public void Save(string str_log)
        {
            try
            {
                MemoryStream packet = CreatePacket(str_log);
                //准备网络资源
                Prepare();
                //发送
                packet.WriteTo(m_NetStream);
                m_NetStream.Flush();
                packet.Dispose();
            }
            catch (Exception)
            {
                ReleaseConnection();
                //发送日志时除了异常，就不再记录日志了，否则在极端情况下可能会造成大量日志挤压，耗光内存
            }
        }

        MemoryStream CreatePacket(string strlog)
        {
            MemoryStream packet = new();

            EncodeHelper.EncodeTag(packet, Params.PARAMETER_TERMINALID);
            byte[] UID = System.Text.Encoding.UTF8.GetBytes(Config.Instance.UID);
            packet.WriteByte((byte)UID.Length);
            packet.Write(UID, 0, UID.Length);
            EncodeHelper.EncodeTag(packet, Params.PARAMETER_COMMANDID);
            packet.WriteByte(0x01);
            packet.WriteByte((byte)Cmds.SaveLog);

            EncodeHelper.EncodeByteTag(packet, Params.PARAMETER_LOGINFO_LEVEL, 0x01);

            //填内容                
            EncodeHelper.EncodeTag(packet, Params.PARAMETER_LOG);
            byte[] content = System.Text.Encoding.UTF8.GetBytes(strlog);
            ushort contentlen = (ushort)content.Length;
            byte[] contentlength = BitConverter.GetBytes(contentlen);
            packet.Write(contentlength, 0, contentlength.Length);
            packet.Write(content, 0, content.Length);

            return EncodeHelper.FormatPacket(packet);
        }

        void Prepare()
        {
            if ((null == m_Client) || (null == m_NetStream) || (false == m_Client.Connected))
            {
                ReleaseConnection();

                IPAddress address = IPAddress.Parse(m_ServerIP);
                IPEndPoint? server = new(address, m_ServerPort);

                m_Client = new TcpClient();
                m_Client.Connect(server);
                m_NetStream = m_Client.GetStream();
                server = null;
            }
        }

        void ReleaseConnection()
        {
            try
            {
                if (null != m_NetStream)
                {
                    m_NetStream.Close();
                    m_NetStream = null;
                }
                if (null != m_Client)
                {
                    m_Client.Close();
                    m_Client = null;
                }
            }
            catch (Exception)
            {

            }
        }

        int m_ServerPort = 0;
        string? m_ServerIP = null;
        TcpClient? m_Client = null;
        NetworkStream? m_NetStream = null;
    }
}
