using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigServiceApi.Models;
using ConfigServiceApi.Utils;

namespace ConfigServiceApi.Services
{
    public class RisConfigService
    {
        private static string GetRisConfigRootPath()
        {
            var path =  ConfigHelper.GetSetting("ConfigServiceFilePath");
            var separatorChar = path.EndsWith("/") ? "" : "/";
            return path;
        }

        private static string GetRisConfig(string filePath)
        {
            var fullFilePath = GetRisConfigRootPath() + filePath;
            if (File.Exists(fullFilePath) == false)
            {
                throw new Exception("文件不存在！，" + fullFilePath);
            }
            var fileContent = File.ReadAllText(fullFilePath);
            if(string.IsNullOrEmpty(fileContent))
            {
                throw new Exception("文件内容为空！" + fullFilePath);
            }
            return fileContent;
        }

        public static string GetFileNodeValue(string filePath,string nodePath)
        {
            var fileContent = GetRisConfig(filePath);
            // 获取文件中的节点
            var nodeValue = XmlTool.GetOutNode(fileContent, nodePath);
            nodeValue = XmlTool.ToFormatXmlString(nodeValue);
            nodeValue = nodeValue.Replace("&#xA;", "\n").Replace("&#xD;", "\r");
            return nodeValue;
        }

        public static bool PostFileNodeValue(string  filePath,string nodePath,string nodeValue)
        {
            var fileContent = GetRisConfig(filePath);
            fileContent = XmlTool.ReplaceNode(fileContent, nodePath, nodeValue);
            return FileTool.SaveToFile(GetRisConfigRootPath() + filePath, XmlTool.ToFormatXmlString(fileContent));
        }


        // 从配置文件中获取数据库连接信息
        public static string GetDbConnection(out DBType dbType)
        {
            var dbTypeStr = ConfigHelper.GetSetting("DBType", "");
            if (dbTypeStr == "ORACLE") 
            {
                dbType = DBType.ORACLE;
            }
            else if(dbTypeStr == "ODBC")
            {
                dbType = DBType.ODBC;
            }
            else if(dbTypeStr == "MYSQL")
            {
                dbType = DBType.MYSQL;
            }
            else
            {
                dbType = DBType.ORACLE;
            }

            var filePath = GetRisConfigRootPath() + "General/DataSourceConfiguration.xml";


            return ConfigHelper.GetSetting("RisConnectString", "");
        }
    }
}