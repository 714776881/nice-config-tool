using ConfigService.DataModel;
using ServiceManager.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigService.Business
{
    public class ConfigCenter
    {
        public static ConfigCenter Instance
        {
            get
            {
                return m_Instance;
            }
        }

        //加载配置文件到内存中来
        public void Load(string rootPath)
        {
            m_RootPath = Path.Combine(rootPath, "Config");
            Console.WriteLine($"m_RootPath={m_RootPath}");
            LoadGeneralConfig();
            LoadDepartmentConfig();
            LoadTerminalConfig();
            LoadPersonalConfig();
            LoadAuthorityConfig();
            LoadTechstationConfig();
        }

        public string GetConfig(EConfigType cfgType, ETerminalType terminalType, string department, string userInfo, string clientIp, bool bDefault = false)
        {
            string strCfg = "";
            switch (cfgType)
            {
                case EConfigType.E_GeneralConfig:
                    LogAdapter.LogInfo("E_GeneralConfig.");
                    strCfg = GetGeneralConfig(terminalType);
                    break;
                case EConfigType.E_DataSourceConfig:
                    LogAdapter.LogInfo("E_DataSourceConfig.");
                    strCfg = GetDataSourceConfig();
                    break;
                case EConfigType.E_DataSourceConfigRIS:
                    LogAdapter.LogInfo("E_DataSourceConfigRIS.");
                    strCfg = GetRISDataSourceConfig();
                    break;
                case EConfigType.E_DepartmentConfig:
                    //strCfg = GetDepartmentConfig(department, bDefault, terminalType);
                    break;
                case EConfigType.E_TerminalConfig:
                    LogAdapter.LogInfo("E_TerminalConfig.");
                    strCfg = GetTerminalConfig(clientIp, bDefault, terminalType);
                    break;
                case EConfigType.E_PersionalConfig:
                    LogAdapter.LogInfo("E_PersionalConfig.");
                    strCfg = GetPersonalConfig(userInfo, bDefault, terminalType);
                    break;
                case EConfigType.E_AuthorityConfig:
                    LogAdapter.LogInfo("E_AuthorityConfig.");
                    strCfg = GetAuthorityConfig(userInfo, bDefault, terminalType);
                    break;
            }
            return strCfg;
        }

        public bool SaveConfig(EConfigType cfgType, ETerminalType terminalType, string department, string userInfo, string clientIp, string configStr)
        {
            bool bRet = false;
            switch (cfgType)
            {
                case EConfigType.E_GeneralConfig:
                    break;
                case EConfigType.E_DataSourceConfig:
                    break;
                case EConfigType.E_DepartmentConfig:
                    SaveDepartmentConfig(department, configStr, terminalType);
                    break;
                case EConfigType.E_TerminalConfig:
                    SaveTerminalConfig(clientIp, configStr, terminalType);
                    break;
                case EConfigType.E_PersionalConfig:
                    SavePersonalConfig(userInfo, configStr, terminalType);
                    break;
                case EConfigType.E_AuthorityConfig:
                    ConfigCenter.Instance.SaveAuthorityConfig(userInfo, configStr, terminalType);
                    break;
            }

            return bRet;
        }

        private string GetGeneralConfig(ETerminalType terminalType)
        {
            if (terminalType == ETerminalType.E_Techstation)
            {
                string techstationConfig = string.Empty;
                ReadRisConfig(m_TechstationConfig, ref techstationConfig);
                return techstationConfig;
            }

            if (terminalType != ETerminalType.E_Ris)
            {
                string generalConfig = string.Empty;
                ReadConfig(m_GeneralConfigFile, ref generalConfig);
                return generalConfig;
            }
            else
            {
                string generalConfig = string.Empty;
                ReadRisConfig(m_RisGeneralConfig, ref generalConfig);
                return generalConfig;
            }
        }

        private string GetDataSourceConfig()
        {
            string dsConfig = string.Empty;
            string cfgFile = Path.Combine(GetConfigPath(), "DataSourceConfiguration_NEW.xml");
            ReadConfig(cfgFile, ref dsConfig);

            return dsConfig;
        }

        private string GetRISDataSourceConfig()
        {
            string dsConfig = string.Empty;
            string cfgFile = Path.Combine(GetConfigPath(), GetRisSubPath(), "General", "DataSourceConfiguration.xml");
            ReadConfig(cfgFile, ref dsConfig);
            return dsConfig;
        }

        //private string GetDepartmentConfig(string department, bool loaddefault, ETerminalType terminalType)
        //{
        //    string config = "";
        //    if (terminalType == ETerminalType.E_Ris)
        //    {
        //        string backup = "RisDepartmentGeneral_Config.xml";
        //        GetConfig(department, m_RisDepartmentConfig, ref config, backup, loaddefault);
        //    }

        //    return config;
        //}

        public string GetDepartmentConfig(ETerminalType clientType, Dictionary<string, string> pathdic)
        {
            string config = "";
            if (clientType == ETerminalType.E_Ris)
            {
                try
                {
                    config = XmlAccessor.MergeXmlDocument("config", pathdic);
                }
                catch (System.Exception ex)
                {
                    //Log.Instance.Error("Exception in GetDepartmentConfig" + ex.Message);
                    //Log.Instance.Error("Exception in GetDepartmentConfig" + ex.StackTrace);
                    config = string.Empty;
                }
            }
            return config;
        }

        public bool HasDepartmentConfig(string department, ETerminalType clientType, ref Dictionary<string, string> departDic)
        {
            bool ret = false;
            if (clientType == ETerminalType.E_Ris)
            {
                ret = HasConfigDir(department, m_RisDepartmentConfig, ref departDic);
            }
            return ret;
        }

        bool HasConfigDir(string client, Dictionary<string, Dictionary<string, string>> source, ref Dictionary<string, string> fileDic)
        {
            bool ret = false;
            Monitor.Enter(source);
            string key = client;
            if (source.ContainsKey(key))
            {
                fileDic = source[key];
                ret = true;
            }
            else
            {
                fileDic = null;
                ret = false;
            }
            Monitor.Pulse(source);
            Monitor.Exit(source);
            return ret;
        }

        //添加客户端类型参数，用于未来扩展
        private string GetTerminalConfig(string clientip, bool loaddefault, ETerminalType terminalType)
        {
            string config = "";
            if (terminalType != ETerminalType.E_Ris)
            {
                GetConfig(clientip, m_TerminalConfig, ref config, "TerminalGeneral_Config.xml", loaddefault);
            }
            else
            {
                GetRisConfig(clientip, m_RisTerminalConfig, ref config, "RisTerminalGeneral_Config.xml", loaddefault);
            }
            return config;
        }

        private string GetPersonalConfig(string clientip, bool loaddefault, ETerminalType terminalType)
        {
            string config = "";
            if (terminalType != ETerminalType.E_Ris)
            {
                GetConfig(clientip, m_PersonalConfig, ref config, "PersonalGeneral_Config.xml", loaddefault);
            }
            else
            {
                if (loaddefault)    //默认
                {
                    GetRisDefaultConfig(clientip, m_RisPersonalConfigDefault, ref config, "RisPersonalGeneral_Config.xml");
                }
                else
                {
                    GetRisConfig(clientip, m_RisPersonalConfig, ref config, "RisPersonalGeneral_Config.xml", loaddefault);
                }
            }
            return config;
        }

        private string GetAuthorityConfig(string clientip, bool loaddefault, ETerminalType terminalType)
        {
            string config = "";
            string backup = "AuthorityGeneral_Config.xml";
            GetConfig(clientip, m_AuthorityConfig, ref config, backup, loaddefault);

            return config;
        }

        private void SaveDepartmentConfig(string key, string config, ETerminalType terminalType)
        {
            //目前只有RIS支持Department
            if (terminalType == ETerminalType.E_Ris)
            {
                //SaveConfig(GetRisSubPath() + "\\Department\\", key, config, m_RisDepartmentConfig);
            }
        }

        private void SaveTerminalConfig(string key, string config, ETerminalType terminalType)
        {
            if (terminalType != ETerminalType.E_Ris)
            {
                SaveConfig("Terminal", key, config, m_TerminalConfig);
            }
            else
            {
                SaveConfig(Path.Combine(GetRisSubPath(), "Terminal"), key, config, m_RisTerminalConfig);
            }
        }

        private void SavePersonalConfig(string key, string config, ETerminalType terminalType)
        {
            if (terminalType != ETerminalType.E_Ris)
            {
                SaveConfig("Personal", key, config, m_PersonalConfig);
            }
            else
            {
                SaveConfig(Path.Combine(GetRisSubPath(), "Personal"), key, config, m_RisPersonalConfig);
            }
        }

        private void SaveAuthorityConfig(string key, string config, ETerminalType terminalType)
        {
            string subdir = "Authority";
            SaveConfig(subdir, key, config, m_AuthorityConfig);
        }

        private void LoadGeneralConfig()
        {
            m_GeneralConfigFile = Path.Combine(GetConfigPath(), "ViewerGeneral_Config.xml");
            LoadConfig(m_RisGeneralConfig, Path.Combine(GetRisSubPath(), "General"));
        }

        private void LoadDepartmentConfig()
        {
            LoadConfig(m_RisDepartmentConfig, Path.Combine(GetRisSubPath(), "Department"));
        }

        private void LoadTerminalConfig()
        {
            LoadConfig(m_TerminalConfig, "Terminal");
            LoadConfig(m_RisTerminalConfig, Path.Combine(GetRisSubPath(), "Terminal"));
        }

        private void LoadPersonalConfig()
        {
            LoadConfig(m_PersonalConfig, "Personal");
            LoadConfig(m_RisPersonalConfig, Path.Combine(GetRisSubPath(), "Personal"));
            LoadConfig(m_RisPersonalConfigDefault, Path.Combine(GetRisSubPath(), "Personal", "Default"));
        }

        private void LoadAuthorityConfig()
        {
            LoadConfig(m_AuthorityConfig, "Authority");
        }
        private void LoadTechstationConfig()
        {
            LoadConfig(m_TechstationConfig, "Techstation");
        }
        private void LoadConfig(Dictionary<string, string> target, string subdir)
        {
            string strdir = Path.Combine(GetConfigPath(), subdir);
            if (Directory.Exists(strdir))
            {
                string[] files = Directory.GetFiles(strdir);
                for (int i = 0; i < files.Length; ++i)
                {
                    string filename = Path.GetFileName(files[i]);
                    string extension = Path.GetExtension(filename);
                    if (extension.Trim().ToLower() == ".xml")
                    {
                        target.Add(filename, files[i]);
                    }
                }
            }
        }

        void LoadConfig(Dictionary<string, Dictionary<string, string>> target, string subdir)
        {
            string strdir = Path.Combine(GetConfigPath(), subdir);
            if (Directory.Exists(strdir))
            {
                string[] dirArray = Directory.GetDirectories(strdir);
                for (int i = 0; i < dirArray.Length; ++i)
                {
                    string[] files = Directory.GetFiles(dirArray[i]);
                    Dictionary<string, string> filelist = new Dictionary<string, string>();
                    for (int j = 0; j < files.Length; ++j)
                    {
                        string filename = Path.GetFileName(files[j]);
                        string extension = Path.GetExtension(filename);
                        if (extension.Trim().ToLower() == ".xml")
                        {
                            filelist.Add(filename, files[j]);
                        }
                    }
                    string deptname = Path.GetFileName(dirArray[i]);
                    target.Add(deptname, filelist);
                }
            }
        }


        private void ReadConfig(string filepath, ref string content)
        {
            Console.WriteLine(filepath);
            LogAdapter.LogInfo(string.Format("读取配置文件:{0}", filepath));
            if (File.Exists(filepath) == false)
            {
                content = string.Empty;
                return;
            }
            FileStream fs = File.Open(filepath, FileMode.Open);
            if (null != fs)
            {
                int filelen = (int)fs.Length;
                byte[] body = new byte[filelen];
                fs.Read(body, 0, body.Length);
                content = System.Text.Encoding.UTF8.GetString(body, 0, body.Length);
                fs.Close();
                fs = null;
            }
        }

        private void ReadRisConfig(Dictionary<string, string> sourceConfig, ref string content)
        {
            try
            {
                content = XmlAccessor.MergeXmlDocument("config", sourceConfig);
            }
            catch (System.Exception ex)
            {
                //Log.Instance.Error("Exception in ReadRisConfig" + ex.Message);
                //Log.Instance.Error("Exception in ReadRisConfig" + ex.StackTrace);
                content = string.Empty;
            }
        }

        private bool HasConfig(string client, Dictionary<string, string> source, string backup)
        {
            bool ret = false;

            Monitor.Enter(source);
            string key = client + ".xml";
            if (source.ContainsKey(key))
            {
                ret = true;
            }
            else
            {
                if (source.ContainsKey(backup))
                {
                    ret = true;
                }
            }
            Monitor.Pulse(source);
            Monitor.Exit(source);

            return ret;
        }

        private void GetRisConfig(string client, Dictionary<string, string> source, ref string config, string backup, bool loaddefault)
        {
            Monitor.Enter(source);
            string filename = null;
            if (loaddefault)
            {
                if (source.ContainsKey(backup))
                {
                    filename = source[backup];
                }
            }
            else
            {
                string key = client + ".xml";
                if (source.ContainsKey(key))
                {
                    filename = source[key];
                }
            }
            if (null != filename)
            {
                ReadConfig(filename, ref config);
            }
            Monitor.Pulse(source);
            Monitor.Exit(source);
        }

        private void GetRisDefaultConfig(string client, Dictionary<string, string> sourceConfig, ref string config, string backup)
        {
            try
            {
                config = XmlAccessor.MergeXmlDocument("config", sourceConfig);
            }
            catch (System.Exception ex)
            {
                //Log.Instance.Error("Exception in GetRisDefaultConfig" + ex.Message);
                //Log.Instance.Error("Exception in GetRisDefaultConfig" + ex.StackTrace);
                config = string.Empty;
            }
        }

        private void GetConfig(string client, Dictionary<string, string> source, ref string config, string backup, bool loaddefault)
        {
            Monitor.Enter(source);
            bool found = false;

            string filename = null;
            if (!loaddefault)
            {
                string key = client + ".xml";
                if (source.ContainsKey(key))
                {
                    filename = source[key];
                    found = true;
                }
            }
            if (!found)
            {
                if (source.ContainsKey(backup))
                {
                    filename = source[backup];
                }
            }

            if (null != filename)
            {
                ReadConfig(filename, ref config);
            }
            Monitor.Pulse(source);
            Monitor.Exit(source);
        }

        private void SaveConfig(string subdir, string key, string config, Dictionary<string, string> target)
        {
            try
            {
                Monitor.Enter(target);
                string dir = Path.Combine(GetConfigPath(), subdir);
                if (Directory.Exists(dir))
                {
                    string strkey = key + ".xml";
                    string filepath = Path.Combine(dir, strkey);
                    FileStream fs = File.Open(filepath, FileMode.Create);
                    if (null != fs)
                    {
                        byte[] body = System.Text.Encoding.UTF8.GetBytes(config);
                        fs.Write(body, 0, body.Length);
                        fs.Flush();
                        fs.Close();
                        fs = null;
                        if (!target.ContainsKey(strkey))
                        {
                            target[strkey] = filepath;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Log.Instance.Exception("saveconfig error:" + e.Message);
            }
            finally
            {
                Monitor.Pulse(target);
                Monitor.Exit(target);
            }
        }

        private string GetConfigPath()
        {
            return m_RootPath;
        }

        private string GetRisSubPath()
        {
            return "RisConfig";
        }

        string m_RootPath = string.Empty;
        string m_GeneralConfigFile = string.Empty;
        Dictionary<string, string> m_TerminalConfig = new Dictionary<string, string>(0);
        Dictionary<string, string> m_PersonalConfig = new Dictionary<string, string>(0);
        Dictionary<string, string> m_AuthorityConfig = new Dictionary<string, string>(0);
        Dictionary<string, string> m_TechstationConfig = new Dictionary<string, string>(0);
        //Ris 配置
        //string m_RisGeneralConfigFile = string.Empty;
        Dictionary<string, string> m_RisGeneralConfig = new Dictionary<string, string>(0);
        Dictionary<string, Dictionary<string, string>> m_RisDepartmentConfig = new Dictionary<string, Dictionary<string, string>>(0);
        Dictionary<string, string> m_RisPersonalConfig = new Dictionary<string, string>(0);
        Dictionary<string, string> m_RisPersonalConfigDefault = new Dictionary<string, string>(0);
        Dictionary<string, string> m_RisTerminalConfig = new Dictionary<string, string>(0);

        private static ConfigCenter m_Instance = new ConfigCenter();
    }
}