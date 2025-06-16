using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Text.RegularExpressions;

namespace ConfigServiceApi.Utils
{
    public class XmlTool
    {
        public static bool IsXml(string str)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string GetXmlPath(string xmlStr, string nodeName, int index)
        {
            try
            {
                XElement root = XElement.Parse(xmlStr);
                var nodes = root.Descendants(nodeName);

                if (index < nodes.Count())
                {
                    var node = nodes.ElementAt(index);
                    return GetPath(node);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                // 可以根据需要处理异常
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return string.Empty;
            }
        }

        private static string GetPath(XElement element)
        {
            List<string> pathSegments = new List<string>();
            while (element != null)
            {
                pathSegments.Insert(0, element.Name.LocalName);
                element = element.Parent;
            }
            return string.Join("/", pathSegments);
        }

        public static string GetListFirstNode(string data, string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            XmlNode parent = doc.SelectSingleNode(path);
            if (parent == null || parent.ChildNodes == null || parent.ChildNodes.Count == 0)
            {
                return data;
            }
            XmlNode node = parent.ChildNodes[0];
            return node.OuterXml;
        }

        public static void GetAllNamespaces(XmlDocument xmlDoc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            foreach (XmlAttribute attribute in xmlDoc.DocumentElement.Attributes)
            {
                if (attribute.Name.StartsWith("xmlns"))
                {
                    string prefix = attribute.Name.Contains(":") ? attribute.Name.Split(':')[1] : string.Empty;
                    nsmgr.AddNamespace(prefix, attribute.Value);
                }
            }

            foreach (XmlNode node in xmlDoc.ChildNodes)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Name.StartsWith("xmlns"))
                    {
                        string prefix = attribute.Name.Contains(":") ? attribute.Name.Split(':')[1] : string.Empty;
                        nsmgr.AddNamespace(prefix, attribute.Value);
                    }
                }
            }

            foreach (var prefix in nsmgr)
            {
                Console.WriteLine("Prefix: {0}, Namespace: {1}", prefix, nsmgr.LookupNamespace(prefix.ToString()));
            }
        }
        public static string AddListNode(string data, string path, string value)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            XmlNode parent = doc.SelectSingleNode(path);
            if(parent == null ||parent.ChildNodes == null || parent.ChildNodes.Count == 0)
            {
                return data;
            }
            parent.InnerXml = value;
            return doc.OuterXml;
        }

        public static string AddNode(string data, string path, string value)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            XmlNode parent = doc.SelectSingleNode(path);
            if (parent == null || parent.ChildNodes == null || parent.ChildNodes.Count == 0)
            {
                return data;
            }

            XmlDocumentFragment fragment = doc.CreateDocumentFragment();
            fragment.InnerXml = value;

            parent.AppendChild(fragment);

            return doc.OuterXml;
        }

        // 默认间隔两个空格，可能不太美观，需要改成制表位或者4个空格
        public static string ToFormatXmlString(string xmlString)
        {
            try
            {
                XDocument x_doc = XDocument.Parse(xmlString);
                if(x_doc.Declaration == null)
                {
                    return x_doc.ToString();
                }
                return x_doc.Declaration.ToString() + "\n" +  x_doc.ToString();
            }
            catch
            {
                return xmlString;
            }
        }
        public static string GetNode(string data, string path)
        {
            if(string.IsNullOrEmpty(data) || string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            XPathDocument doc = new XPathDocument(new StringReader(data));
            XPathNavigator navigator = doc.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select(path);
            var value = string.Empty;
            while (iterator.MoveNext())
            {
                value = iterator.Current.InnerXml;
            }
            return value;
        }

        public static string GetOutNode(string data, string path)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            XPathDocument doc = new XPathDocument(new StringReader(data));
            XPathNavigator navigator = doc.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select(path);
            var value = string.Empty;
            while (iterator.MoveNext())
            {
                // 获取 XML 文档的字符串表示，并手动替换 &#xA; 为 \n
                // 注意：这通常不是推荐的做法，因为它可能会破坏 XML 的有效性
                // 如果需要，也可以处理其他字符实体引用，比如 &#xD; (回车)

                value = iterator.Current.OuterXml;
            }
            return value;
        }



        public static readonly string NameSpaceTag = "error=\"I am a placeholder for a namespace, you have found me\"";
        public static string RemoveNameSapce(string xmlMessage,ref string ns)
        {
            string pattern = "xmlns=\"[^\"]*\"";
            var item =  Regex.Match(xmlMessage, pattern);
            if(item != null)
            {
                ns = item.Value;
            }
            return Regex.Replace(xmlMessage, pattern, NameSpaceTag);
        }

        public static string AddNameSpace(string xmlMessage,string ns)
        {
            return xmlMessage.Replace(NameSpaceTag, ns);
        }

        public static List<string> GetChildrenNodes(string data, string path)
        {
            var value = new List<string>();
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(path))
            {
                return value;
            }
            XPathDocument doc = new XPathDocument(new StringReader(data));
            XPathNavigator navigator = doc.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select(path);
            while (iterator.MoveNext())
            {
                XPathNodeIterator childNodes = iterator.Current.SelectChildren(XPathNodeType.Element);
                while (childNodes.MoveNext())
                {
                    value.Add(childNodes.Current.OuterXml);
                }
            }
            return value;
        }

        public static string SetNode(string data, string path, string value)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(path))
            {
                return data;
            }

            value = ReplaceXmlSpecialCharacters(value);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            var nodes =  doc.SelectNodes(path);
            foreach(XmlNode node in nodes)
            {
                node.InnerXml = value;
            }
            return doc.OuterXml;
        }
        public static string ReplaceNode(string data, string path, string newNodeString)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(path))
            {
                return data;
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            var nodes = doc.SelectNodes(path);
            foreach (XmlNode node in nodes)
            {
                XmlDocumentFragment fragment = doc.CreateDocumentFragment();
                fragment.InnerXml = newNodeString;

                node.ParentNode.ReplaceChild(fragment, node);
            }
            return doc.OuterXml;
        }
        public static string EscapeForXml(string input)
        {
            if(IsXml(input) == false || string.IsNullOrEmpty(input))
            {
                return input;
            }

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    xmlWriter.WriteString(input);
                }
                return stringWriter.ToString();
            }
        }
        public static string ReplaceXmlSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Replace special XML characters with their corresponding entities
            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}