using System.Data.Common;
using System.Net.Sockets;

namespace XService
{
    interface XConnectionCreator
    {
        XConnection Create(Socket socket, string clientip);
    }
}
