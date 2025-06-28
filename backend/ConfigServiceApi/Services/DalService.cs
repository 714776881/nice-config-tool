using ConfigServiceApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigServiceApi.Utils;
using System.Runtime.InteropServices;
using System.Data;
using System.Text.RegularExpressions;
using Dapper;

namespace ConfigServiceApi.Services
{
    public class DalService
    {
        private OrmHelper ORM = new OrmHelper();

        public List<Dictionary<string,string>> Select(string sql, dynamic values = null)
        {
            Logger.LogInfo("[Select]：" + sql);
            string message = string.Empty;

            var dt = ORM.Query<dynamic>(sql);
            var result = new List<Dictionary<string, string>>();
            foreach (var row in dt)
            {
                var dic = new Dictionary<string, string>();
                var rowDic = (IDictionary<string, object>)row;
                foreach (var key in rowDic.Keys)
                {
                    dic[key.ToUpper()] = rowDic[key]?.ToString() ?? string.Empty;
                }
                result.Add(dic);
            }

            if (result == null)
            {
                Logger.LogError("[ERROR]：" + message);
                return null;
            }

            //result.Skip((1 - 1) * 50).Take(1).ToList();


            Logger.LogInfo("[RESULT]：" + result.Count + "个");
            return result;
        }

        public bool ExeSql(string sql, Dictionary<string, string> values = null)
        {
            Logger.LogInfo("[ExeSql]：" + sql);

            if (sql.Contains("[;]"))
            {
                var sqls = sql.Split("[;]").Where(item => !string.IsNullOrEmpty(item)).ToList();
                return ExeBatchSql(sqls);
            }

            return ExeFirstSql(sql,values);
        }

        public bool ExeFirstSql(string sql, Dictionary<string, string> values = null)
        {
            string message = string.Empty;
            bool result = false;

            if (values != null && values.Count > 0)
            {
                if (ORM.DBType == DBType.ODBC)
                {
                    // 1. 匹配所有形如 :param 的字段，按出现顺序提取
                    var paramNames = new List<string>();
                    var paramRegex = new Regex(@":(\w+)");
                    sql = paramRegex.Replace(sql, m =>
                    {
                        var name = m.Groups[1].Value;
                        paramNames.Add(name);
                        return "?";
                    });

                    // 2. 根据顺序准备参数列表
                    var paramList = paramNames.ToDictionary(name => name, name => values.TryGetValue(name, out var val) ? val : null).ToList();

                    var parameters = new DynamicParameters();
                    foreach (var kv in paramList)
                    {
                        parameters.Add("", kv.Value);
                    }

                    result = ORM.Execute(sql, parameters) >= 0;
                }
                else
                {
                    // 非 ODBC，直接使用命名参数
                    result = ORM.Execute(sql, values) >= 0;
                }
            }
            else
            {
                result = ORM.Execute(sql) >= 0;
            }
            Logger.LogInfo("[RESULT]：" + result);
            return result;
        }

        public bool ExeBatchSql(List<string> sqls)
        {   
            Logger.LogInfo("[BatchExeSql]：" + string.Join(";",sqls));
            string message = string.Empty;
            var result = ORM.ExecuteMultipleSql(sqls);
            if (result == false)
            {
                Logger.LogError("[ERROR]：" + message);
                return false;
            }
            Logger.LogInfo("[RESULT]：" + result);
            return true;
        }
    }
}