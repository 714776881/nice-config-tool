using ConfigServiceApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigServiceApi.Utils;
using System.Runtime.InteropServices;
using System.Data;

namespace ConfigServiceApi.Services
{
    public class DalService
    {
        private OrmHelper ORM = new OrmHelper();

        public List<Dictionary<string,string>> Select(string sql)
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
            Logger.LogInfo("[RESULT]：" + result.Count + "个");
            return result;
        }

        public bool ExeSql(string sql)
        {
            Logger.LogInfo("[ExeSql]：" + sql);
            string message = string.Empty;
            var result = ORM.Execute(sql);
            if (result == null)
            {
                Logger.LogError("[ERROR]：" + message);
                return false;
            }
            Logger.LogInfo("[RESULT]：" + result);
            return true;
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