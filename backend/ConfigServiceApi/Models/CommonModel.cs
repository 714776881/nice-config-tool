using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using ConfigServiceApi.Utils;

namespace ConfigServiceApi.Models
{

    public class KeyValue
    {
        public KeyValue(string key, string value, string description)
        {
            this.Key = key;
            this.Value = value;
            this.Description = description;
        }
        public string Key
        {
            get; set;
        }
        public string Value
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        override
        public string ToString()
        {
            return string.Format("{0}={1},", Key, Value);
        }
    }

    public class ApiResponse
    {
        public static string Success = "200";
        public static string Error = "500";

        public static string ParamsErrorMsg = "参数错误";

        public ApiResponse(object data, string code = "0", string message = "")
        {
            this.data = data;
            this.code = code;
            this.message = message;
        }

        public ApiResponse() { }

        public string code { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
        public override string ToString()
        {
            return JsonTool.SerializeObject(this);
        }
    }
}
