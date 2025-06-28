using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ConfigServiceApi.Utils
{
    public class JsonTool
    {
        // 深度克隆对象
        public static T Clone<T>(object obj)
        {
            string str = SerializeObject(obj);
            return DeserializeObject<T>(str);
        }
        // 根据节点名称获取第一个匹配到的节点路径
        public static string GetJsonPath(string jsonStr, string nodeName, int index)
        {
            var result = new List<string>();
            JToken token = JToken.Parse(jsonStr);
            var nodes = token.SelectTokens(".." + nodeName);

            if (index < nodes.Count())
            {
                var node = nodes.ToList()[index];

                if (node is JObject) return string.Empty;

                return nodes.ToList()[index].Path;
            }
            return string.Empty;
        }
        // 格式化
        public static string ToFormatJsonString(string jsonStr)
        {
            try
            {
                JToken obj = JToken.Parse(jsonStr);
                return obj.ToString();
            }
            catch (Exception ex)
            {
                return jsonStr;
            }
        }
        // 序列化
        public static string SerializeObject(Object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        // 反序列化
        public static T DeserializeObject<T>(string info)
        {
            return (T)JsonConvert.DeserializeObject(info, typeof(T));
        }
        // 融合JSON
        public static string MergeJson(string json1, string json2)
        {
            if (string.IsNullOrEmpty(json1))
            {
                return json2;
            }
            if (string.IsNullOrEmpty(json2))
            {
                return json1;
            }

            JObject obj1 = JObject.Parse(json1);
            JObject obj2 = JObject.Parse(json2);
            // 合并两个JObject
            obj1.Merge(obj2, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union // 处理数组合并
            });
            // 得到融合后的JSON消息
            return obj1.ToString();
        }
    }
}
