using ConfigService.DataModel;
using ConfigService.Request;
using ConfigServiceApi;
using Microsoft.AspNetCore.Mvc;
using ServiceManager.Tool;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Drawing;

using ConfigServiceApi.Utils;
using ConfigServiceApi.Models;
using ConfigServiceApi.Services;
using log4net.Util;

namespace ConfigServiceHost.ApiControllers
{
    [ApiController]
    public class DalController : ControllerBase
    {
        private DalService _DalService;

        public DalController() {
            _DalService = new DalService();
        }

        [HttpGet]
        [Route("Api/Dal")]
        public ApiResponse Select(DalForm form)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var sql = GetSql(form);
                var result = _DalService.Select(sql);
                if(result == null)
                {
                    res.code = ApiResponse.Success;
                    res.message = "没有查询到相关数据！";
                }
                else
                {
                    res.code = ApiResponse.Error;
                    res.data = result;
                }
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        [Route("Api/Dal/ExeSql")]
        public ApiResponse ExeSql(DalForm form)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var sql = GetSql(form);
                var result = _DalService.ExeSql(sql);

                if (result == false)
                {
                    res.code = ApiResponse.Success;
                    res.message = "没有查询到相关数据！";
                }
                else
                {
                    res.code = ApiResponse.Error;
                    res.data = result;
                }
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        [Route("Api/Dal/ExeBatchSql")]
        public ApiResponse ExeBatchSql(DalBatchForm form)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var sqls = new List<string>();
                foreach (var item in form.data)
                {
                    var sql = ReplaceSql(GetSqlTemplate(form.configFileName, form.sqlPath), item);
                    sqls.Add(sql);
                }
                var result = _DalService.ExeBatchSql(sqls);

                if (result == false)
                {
                    res.code = ApiResponse.Success;
                    res.message = "没有查询到相关数据！";
                }
                else
                {
                    res.code = ApiResponse.Error;
                    res.data = result;
                }
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }
        // 根据参数生成完整SQL
        public static string GetSql(DalForm form)
        {
            var sql = ReplaceSql(GetSqlTemplate(form.configFileName, form.sqlPath), form.data);
            return sql;
        }

        // 从主题配置文件中，根据XPath路径中获取SQL模板
        public static string GetSqlTemplate(string configFileName, string sqlPath)
        {
            string fileContent = ConfigFileService.GetConfig(configFileName);

            return XmlTool.GetNode(fileContent, sqlPath); ;
        }

        // 使用字典数据注入SQL模板中，生成可以执行的SQL
        public static string ReplaceSql(string sqlTemplate, Dictionary<string, string> parameters)
        {
            string sql = string.Empty;
            foreach (var item in parameters)
            {
                sql = sqlTemplate.Replace(item.Key, item.Value);
            }
            return sql;
        }
    }
}