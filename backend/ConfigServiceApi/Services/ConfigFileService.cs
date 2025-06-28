using ConfigServiceApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace ConfigServiceApi.Services
{
    public class ConfigFileService
    {
        public static string GetConfigFilePath()
        {
            var configFilePath = ConfigServiceApi.Utils.ConfigHelper.GetSetting("ConfigFilePath", "");

            if (string.IsNullOrWhiteSpace(configFilePath))
            {
                configFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "/MetaConfig/";
            }

            if (!File.Exists(configFilePath))
            {
                Logger.LogError("未发现配置数据！请检查文件夹是否存在 " + configFilePath);
            }

            var separatorChar = configFilePath.EndsWith("/") ? "" : "/";

            return configFilePath + separatorChar;
        }


        public static string GetConfig(string fileName)
        {
            var filePath = string.Empty;
            // 若是指明了绝对路径
            if(File.Exists(fileName))
            {
                filePath = fileName;
            }
            else
            {
                filePath = GetConfigFilePath() + fileName;
            }
            var fileContent = FileTool.ReadFromFile(filePath);
            return fileContent;
        }

        public static bool PostConfig(string fileName,string fileContent)
        {
            var filePath = string.Empty;
            // 若是指明了绝对路径
            if (File.Exists(fileName))
            {
                filePath = fileName;
            }
            else
            {
                filePath = GetConfigFilePath() + fileName;
            }

            var result = FileTool.SaveToFile(filePath, fileContent);
            return result;
        }
    }
}
