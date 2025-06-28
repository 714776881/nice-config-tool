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


        public static string GetFullFilePath(string filePath)
        {
            var fullFilePath = string.Empty;
            if (File.Exists(filePath))
            {
                fullFilePath = filePath;
            }
            else
            {
                fullFilePath = GetRisConfigRootPath() + filePath;
            }
            return fullFilePath;
        }


        private static string GetRisConfig(string fullFilePath)
        {

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
            var fullFilePath = GetFullFilePath(filePath);
            var fileContent = GetRisConfig(fullFilePath);
            // 获取文件中的节点
            var nodeValue = XmlTool.GetOutNode(fileContent, nodePath);
            nodeValue = XmlTool.ToFormatXmlString(nodeValue);
            nodeValue = nodeValue.Replace("&#xA;", "\n").Replace("&#xD;", "\r");
            return nodeValue;
        }

        public static bool PostFileNodeValue(string  filePath,string nodePath,string nodeValue)
        {
            var fullFilePath = GetFullFilePath(filePath);
            var fileContent = GetRisConfig(fullFilePath);
            fileContent = XmlTool.ReplaceNode(fileContent, nodePath, nodeValue);
            return FileTool.SaveToFile(fullFilePath, XmlTool.ToFormatXmlString(fileContent));
        }


        // 从配置文件中获取数据库连接信息
        public static string GetDbConnection(out DBType dbType)
        {
            var dbTypeStr = ConfigHelper.GetSetting("DBType", "");
            if (dbTypeStr == "ORACLE") {
                dbType = DBType.ORACLE;
            }
            else if(dbTypeStr == "ODBC")
            {
                dbType = DBType.ODBC;
            }
            else if(dbTypeStr == "NPGSQL")
            {
                dbType = DBType.NPGSQL;
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