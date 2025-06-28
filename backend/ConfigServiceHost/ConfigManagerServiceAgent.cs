using ConfigService.Business;
using ConfigService.Request;
using ConfigServiceHost.Business;
using Model.Config;
using Model.Resource;
using ServiceManager.Tool;
using Tool;
using XService;

namespace ConfigServiceHost
{
    public class ConfigManagerServiceAgent
    {
        public ConfigManagerServiceAgent() { }

        private static IRequestFactory _REQUESTFACTORY = new RequestFactory();

        private static string m_HostIP = string.Empty;
        private static Int32 m_Port = 0;
        private static XServiceWorker m_Service = null;

        public bool Initialize()
        {
            try
            {
                RequestManager.GetInstance().RequestFactory = new RequestFactory();             //设置请求创建工厂
                StartConfigService();
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(typeof(ConfigManagerServiceAgent), E_LogType.E_LogError, "", "配置服务初始化异常");
                LogManager.WriteLog(typeof(ConfigManagerServiceAgent), E_LogType.E_LogError, "", ex.StackTrace);
                return false;
            }
        }

        private static bool LoadConfig()
        {
            bool ret = true;
            try
            {
                m_HostIP = ConfigHelper.GetSetting("hostip");
                m_HostIP = NetHelper.CorrectIp(m_HostIP);
                string sPort = ConfigHelper.GetSetting("listenport");
                if (!string.IsNullOrEmpty(sPort))
                {
                    m_Port = int.Parse(sPort);
                }
                if (string.IsNullOrEmpty(m_HostIP) || m_Port <= 0)
                {
                    ret = false;
                }
            }
            catch (Exception e)
            {
                ret = false;
                LogManager.WriteLog("", E_LogType.E_LogError, "Service", string.Format("加载配置时发生异常：{0}", e.Message));
            }

            return ret;
        }

        private static void SettingLanguage()
        {
            string language = System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag;
            Resource.Culture = new global::System.Globalization.CultureInfo(language);
        }

        private static void StartConfigService()
        {
            //设置语言，根据操作系统默认语言来设置
            SettingLanguage();

            LogAdapter.LogInfo("启动配置服务，开始初始化.");

            Config.Instance.UID = "ConfigService";

            if (InitSocketService())
            {
                if (StartSocketService())
                {
                    Log.Instance.Normal("start service success");
                    LogManager.WriteLog("", E_LogType.E_LogInfo, "Service", "Socket接口服务启动成功.");
                }
                else
                {
                    Log.Instance.Normal("start service failed");
                    LogManager.WriteLog("", E_LogType.E_LogError, "Service", "Socket接口服务启动失败，自动退出程序!");
                    Console.WriteLine("configservice start failed, will to exit!");
                    Environment.Exit(0);
                }
            }
            else
            {
                LogManager.WriteLog("", E_LogType.E_LogError, "Service", "Socket接口服务初始化失败，不启动Socket接口服务.");
            }

        }

        private static bool StartSocketService()
        {
            bool ret = false;

            try
            {
                XConnectionCreator connectioncreator = new ConfigConnectionCreator();
                Console.WriteLine($"IP:{m_HostIP}  Port:{m_Port}");
                m_Service = new XServiceWorker(connectioncreator, m_HostIP, m_Port);
                if (null != m_Service)
                {
                    ret = m_Service.Start();
                }
            }
            catch (Exception e)
            {
                LogManager.WriteLog("", E_LogType.E_LogError, "Service", string.Format("启动Socket接口服务时发生异常：{0}", e.Message));
            }
            return ret;
        }

        private static bool InitSocketService()
        {
            bool bRet = false;
            if (LoadConfig())
            {
                Log.Instance.SetSaveMode();
                Log.Instance.Normal("I am config service");
                Log.Instance.Normal("start to load config files...");
                ConfigCenter.Instance.Load(AppContext.BaseDirectory);
                Log.Instance.Normal("config load success, start service...");

                bRet = true;
            }

            return bRet;
        }

    }
}
