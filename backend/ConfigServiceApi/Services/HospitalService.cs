using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ConfigServiceApi.Utils;
using ConfigServiceApi.Models;

namespace ConfigServiceApi
{
    public class HospitalService
    {
        private static string ConfigServiePath = "";
        private static string EmsFileFullPath = "";
        public HospitalService()
        {
            EmsFileFullPath = ConfigHelper.GetSetting("EmsConfigFilePath", "");
        }

        public static void Load(string configServicePath)
        {
            ConfigServiePath = configServicePath;
        }

        // 修改EMS文件
        public bool PostEms(HospitalRequestModel hospital)
        {
            var fileFullPath = EmsFileFullPath;
            var fileContent = FileTool.ReadFromFile(fileFullPath);

            var ris_name = XmlTool.GetNode(fileContent, "//add[@type='ris']/@name");
            var cloud_name = XmlTool.GetNode(fileContent, "//add[@type='CLOUDPLATFORM']/@name");
            var db_name = string.Empty;
            if(hospital.hospitalType == "0" )
            {
                db_name = ris_name; // 提交医院
            }
            else
            {
                db_name = cloud_name; // 会诊医院
            }

            var dataSourceNode = $"<add name=\"{hospital.hospitalName}\" dbName = \"{db_name}\" hospitalIds=\"{hospital.hospitalCode}\" departmentNames=\"{hospital.departments}\"/>";

            var nodePath = $"//add[@hospitalIds='{hospital.oldHospitalCode}']";
            var node = XmlTool.GetOutNode(fileContent, nodePath);
            if(node == "")
            {
                var newContent = XmlTool.AddNode(fileContent, "//dataSources", dataSourceNode);
                fileContent = XmlTool.ToFormatXmlString(newContent);
            }
            else
            {
                var newContent = XmlTool.ReplaceNode(fileContent, nodePath, dataSourceNode);
                fileContent = XmlTool.ToFormatXmlString(newContent);
            }

            return FileTool.SaveToFile(fileFullPath, fileContent);
        }

        public bool PostDataSoure(HospitalRequestModel hospital)
        {
            var fileFullPath = ConfigServiePath + "\\Config\\RisConfig\\General\\DataSourceConfiguration.xml";
            var fileContent = FileTool.ReadFromFile(fileFullPath);

            var ris_name = XmlTool.GetNode(fileContent, "//item[@value='ris' and @name='type']/../@name");

            var dataSourceNode = $"<item name=\"{hospital.hospitalName}\"><item name=\"HospitalID\" value=\"{hospital.hospitalCode}\" /><item name=\"HospitalName\" value=\"{hospital.hospitalName}\" /><item name=\"dbName\" value=\"{ris_name}\" /></item>";

            var nodePath = $"//item[@name='HospitalID' and @value='{hospital.oldHospitalCode}']/..";
            var node = XmlTool.GetNode(fileContent, nodePath);
            if(node == "")
            { 
                var newContent = XmlTool.AddNode(fileContent, "//item[@name='DataSources']", dataSourceNode);
                fileContent = XmlTool.ToFormatXmlString(newContent);
            }
            else
            {
                var newContent = XmlTool.ReplaceNode(fileContent, nodePath, dataSourceNode);
                fileContent = XmlTool.ToFormatXmlString(newContent);
            }

            return FileTool.SaveToFile(fileFullPath, fileContent);
        }

        public bool PostDataSoureNew(HospitalRequestModel hospital)
        {
            var fileFullPath = ConfigServiePath + "\\Config\\DataSourceConfiguration_NEW.xml";
            var fileContent = FileTool.ReadFromFile(fileFullPath);

            var ris_name = XmlTool.GetNode(fileContent, "//item[@value='ris' and @name='type']/../@name");

            var dataSourceNode = $"<item name=\"{hospital.hospitalName}\"><item name=\"HospitalID\" value=\"{hospital.hospitalCode}\" /><item name=\"HospitalName\" value=\"{hospital.hospitalName}\" /><item name=\"dbName\" value=\"{ris_name}\" /></item>";

            var nodePath = $"//item[@name='HospitalID' and @value='{hospital.oldHospitalCode}']/..";
            var node = XmlTool.GetNode(fileContent, nodePath);
            if (node == "")
            {
                var newContent = XmlTool.AddNode(fileContent, "//item[@name='DataSources']", dataSourceNode);
                fileContent = XmlTool.ToFormatXmlString(newContent);
            }
            else
            {
                var newContent = XmlTool.ReplaceNode(fileContent, nodePath, dataSourceNode);
                fileContent = XmlTool.ToFormatXmlString(newContent);
            }
            return FileTool.SaveToFile(fileFullPath, fileContent);
        }

        public bool PostRisConfigDepartment(HospitalRequestModel hospital)
        {
            var dirPath = ConfigServiePath + "\\Config\\RisConfig\\Department";
            try
            {
                var hospitalName = hospital.hospitalName + "_" + hospital.hospitalCode;

                DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                // 发现是否已存在该文件夹
                foreach (DirectoryInfo directory in directories)
                {
                    if (directory.Name.Equals(hospitalName))
                    {
                        return true;
                    }
                }

                if(string.IsNullOrEmpty(hospital.templateType))
                {
                    return true;
                }

                // 根据模板类型创建模板文件
                string tempDirName = string.Empty;
                switch (hospital.templateType)
                {
                    case "1":
                        tempDirName = "超声会诊医院_hospitalid";
                        break;
                    case "2":
                        tempDirName = "超声科_itemcode";
                        break;
                    case "3":
                        tempDirName = "超声社区医院_hospitalid";
                        break;
                    case "4":
                        tempDirName = "放射会诊医院_hospitalid";
                        break;
                    case "5":
                        tempDirName = "放射科_itemcode";
                        break;
                    case "6":
                        tempDirName = "放射社区医院_hospitalid";
                        break;
                }
                var newDirectory = directoryInfo.CreateSubdirectory(hospital.hospitalName + "_" + hospital.hospitalCode);
                CopyDirectory(dirPath + "\\" + tempDirName, newDirectory.FullName);
            }
            catch (Exception ex)
            {
                // 捕获并处理任何异常
                Console.WriteLine("PostRisConfigDepartment：" + ex.Message);
            }
            return true;
        }
        static void CopyDirectory(string sourceDir, string destDir)
        {
            // 获取源文件夹中的所有文件和子文件夹
            var files = Directory.GetFiles(sourceDir);
            var dirs = Directory.GetDirectories(sourceDir);

            // 创建目标文件夹
            Directory.CreateDirectory(destDir);

            // 复制文件
            foreach (var file in files)
            {
                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true); // true 表示覆盖现有文件
            }

            // 递归复制子文件夹
            foreach (var dir in dirs)
            {
                var destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
        }
    }
}