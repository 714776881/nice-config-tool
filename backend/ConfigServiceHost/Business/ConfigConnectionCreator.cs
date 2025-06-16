
using System.Data.Common;
using System.Net.Sockets;
using XService;

namespace ConfigServiceHost.Business
{
    class ConfigConnectionCreator : XConnectionCreator
    {
        public XConnection Create(Socket socket, string clientip)
        {
            XConnection con = new ConfigConnection(socket, clientip);
            return con;
        }
    }
}