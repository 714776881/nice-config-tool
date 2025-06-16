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
            var ConfigFilePath = ConfigServiceApi.Utils.ConfigHelper.GetSetting("ConfigFilePath", "");

            if (string.IsNullOrWhiteSpace(ConfigFilePath))
            {
                ConfigFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "/MetaConfig/";
            }

            var separatorChar = ConfigFilePath.EndsWith("/") ? "" : "/";

            return ConfigFilePath + separatorChar;
        }


        public static string GetConfig(string fileName)
        {
            var fileContent = FileTool.ReadFromFile(GetConfigFilePath() + fileName);
            return fileContent;
        }

        public static bool PostConfig(string fileName,string fileContent)
        {
            var result = FileTool.SaveToFile(GetConfigFilePath() + fileName, fileContent);
            return result;
        }
    }
}
