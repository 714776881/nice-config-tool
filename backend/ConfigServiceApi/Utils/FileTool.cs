using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConfigServiceApi.Utils
{
    public class FileTool
    {
        public static bool SaveToFile(string filePath, string content)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    writer.Write(content);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("保存文件失败：" + ex.Message);
            }
            return false;
        }

        public static bool RemoveFile(string filePath)
        {
            try
            {
                if(File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                else
                {
                    Console.WriteLine("未发现需要删除的文件," + filePath);
                    return false;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("移除文件失败：" + ex.Message);
            }
            return false;
        }

        public static string ReadFromFile(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取文件失败：" + ex.Message);
                throw;
            }
        }
    }
}
