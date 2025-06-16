
namespace Tool
{
    class NetHelper
    {
        /// <summary>
        /// 矫正IP地址，如果输入的IP不是在本机有效IP列表内，则给一个万能IP
        /// </summary>
        /// <param name="hostIp"></param>
        /// <returns></returns>
        public static string CorrectIp(string hostIp)
        {
            string correctedIp = string.Empty;

            if (string.IsNullOrEmpty(hostIp))
            {
                correctedIp = fullIp;
            }
            else
            {
                try
                {
                    string hostname = System.Net.Dns.GetHostName();
                    System.Net.IPAddress[] localips = System.Net.Dns.GetHostAddresses(hostname);
                    for (int i = 0; i < localips.Length; i++)
                    {
                        //取v4
                        if (System.Net.Sockets.AddressFamily.InterNetwork == localips[i].AddressFamily)
                        {
                            if (hostIp.Equals(localips[i].ToString()))
                            {
                                correctedIp = hostIp;
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            if (string.IsNullOrEmpty(correctedIp))
            {
                correctedIp = fullIp;//设置全能IP
            }

            return correctedIp;
        }

        /// <summary>
        /// 检查指定的IP是否包含在本机的有效IP列表中，同时会返回一个新的有效IP
        /// </summary>
        /// <param name="hostIp"></param>
        /// <returns></returns>
        public static bool IsHostIpValid(string hostIp, ref string newIp)
        {
            if (string.IsNullOrEmpty(hostIp))
            {
                newIp = fullIp;
                return false;
            }
            bool ret = false;
            try
            {
                string hostname = System.Net.Dns.GetHostName();
                System.Net.IPAddress[] localips = System.Net.Dns.GetHostAddresses(hostname);
                for (int i = 0; i < localips.Length; i++)
                {
                    //取v4
                    if (System.Net.Sockets.AddressFamily.InterNetwork == localips[i].AddressFamily)
                    {
                        if (hostIp.Equals(localips[i].ToString()))
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            if (!ret)
            {
                newIp = fullIp;//设置全能IP
            }

            return ret;
        }

        static readonly string fullIp = "0.0.0.0";
    }
}
