using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConfigService.Business
{
    public class XmlAccessor
    {
        /// <summary>
        /// 合并两个XML文档(合并根节点)
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="targetXmlDoc">目标XML</param>
        /// <param name="defaultXmlDoc">默认的xml</param>
        public static string MergeXmlDocument(string xpath, Dictionary<string, string> filepath)
        {
            int iPos = xpath.LastIndexOf('/');
            if (iPos > 0)
            {
                xpath = xpath.Substring(0, iPos);
            }
            XmlDocument docment = new XmlDocument();
            XmlNode rootElement = null;
            string generalConfig = string.Empty;
            foreach (string keyPath in filepath.Keys)
            {
                try
                {
                    generalConfig = string.Empty;
                    ReadConfig(filepath[keyPath], ref generalConfig);
                    if (rootElement == null)
                    {
                        if (generalConfig.IndexOf('<') > 0)
                        {
                            generalConfig = generalConfig.Substring(generalConfig.IndexOf('<'));
                        }
                        docment.LoadXml(generalConfig.Trim());
                        rootElement = docment.SelectSingleNode(xpath);
                        continue;
                    }
                    XmlDocument xml = new XmlDocument();
                    if (generalConfig.IndexOf('<') > 0)
                    {
                        generalConfig = generalConfig.Substring(generalConfig.IndexOf('<'));
                    }
                    xml.LoadXml(generalConfig.Trim());
                    XmlNode xn = xml.SelectSingleNode(xpath);
                    foreach (XmlNode child in xn.ChildNodes)
                    {
                        XmlNode nodeClone = docment.ImportNode(child, true);
                        rootElement.AppendChild(nodeClone);
                    }
                }
                catch (Exception ex)
                {
                    //Log.Instance.Error("Exception in MergeXmlDocument. FilePath: " + keyPath);
                    //Log.Instance.Error("Exception in MergeXmlDocument" + ex.Message);
                    //Log.Instance.Error("Exception in MergeXmlDocument" + ex.StackTrace);
                    throw;
                }
            }
            return docment.InnerXml;
        }

        private static void ReadConfig(string filepath, ref string content)
        {
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
    }
}
