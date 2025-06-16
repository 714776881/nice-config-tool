using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace ConfigServiceApi.Utils
{
    public class ConfigHelper
    {
        static IConfiguration Configuration { get; set; }

        static string file = "appsettings.json";
        public ConfigHelper(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static string GetSetting(string key, string dfValue = "")
        {
            string val = string.Empty;
            try
            {
                if (Configuration == null)
                {
                    Configuration = new ConfigurationBuilder()
                        .Add(new JsonConfigurationSource
                        {
                            Path = "appsettings.json",
                            Optional = false,
                            ReloadOnChange = true
                        }).Build();
                }
                val = Configuration[key];
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
            }
            if (string.IsNullOrEmpty(val) && (string.IsNullOrEmpty(dfValue) == false))
            {
                val = dfValue;
            }
            return val;
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns></returns>
        public static string GetSetting(params string[] sections)
        {
            try
            {
                if (Configuration == null)
                {
                    Configuration = new ConfigurationBuilder()
                        .Add(new JsonConfigurationSource
                        {
                            Path = "appsettings.json",
                            Optional = false,
                            ReloadOnChange = true
                        }).Build();
                }
                if (sections.Any())
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception)
            {

            }
            return "";
        }

        /// <summary>
        /// 递归获取配置信息数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<T> GetSetting<T>(params string[] sections)
        {
            try
            {
                if (Configuration == null)
                {
                    Configuration = new ConfigurationBuilder()
                        .Add(new JsonConfigurationSource
                        {
                            Path = "appsettings.json",
                            Optional = false,
                            ReloadOnChange = true
                        }).Build();
                }
                List<T> list = new List<T>();
                Configuration.Bind(string.Join(":", sections), list);
                return list;
            }
            catch (Exception)
            {

            }
            return null;
        }

    }
}

