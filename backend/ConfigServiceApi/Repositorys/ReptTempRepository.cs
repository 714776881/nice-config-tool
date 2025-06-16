using ConfigServiceApi.Models;
using ConfigServiceApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Repository
{
    internal class ReptTempRepository : RepositoryBase
    {
        public static new string TableName = "t_repttemp";
        public static new string IdName = "repttempid";
        public TReptTempEntity GetReptTempById(string reptTempId)
        {
            var sql = $"select * from t_repttemp where repttempid = '{reptTempId}'";
            return Orm.QueryFirst<TReptTempEntity>(sql);
        }
        public List<TReptTempEntity> GetReptTempByCategoryId(string categoryId,string ownerId)
        {
            var sql = $"select * from t_repttemp where categoryId = '{categoryId}' ";
            sql += $"and （ownerid = '{ownerId}' or ownerid is null) ";
            sql += $"and deleted = '0'";
            return Orm.Query<TReptTempEntity>(sql).ToList();
        }

        // 目录中使用，不加载模板数据
        public List<TReptTempEntity> GetReptTempNodeByCategoryId(string categoryId, string ownerId)
        {
            var sql = $"select r.repttempid,r.repttemp,r.temptype,r.ownerid,r.categoryid,r.sequence from t_repttemp r where categoryId = '{categoryId}' ";
            sql += $"and （ownerid = '{ownerId}' or ownerid is null) ";
            sql += $"and deleted = '0'";
            return Orm.Query<TReptTempEntity>(sql).ToList();
        }

        public bool Add(TReptTempEntity reptTemp)
        {
            return Orm.Insert(reptTemp, TableName) > 0;
        }

        public bool Update(TReptTempEntity reptTempModel)
        {
            return Orm.Update(reptTempModel, TableName, IdName) >= 0;
        }

        public bool Delete(TReptTempEntity reptTempModel)
        {
            var sql = $"update t_repttemp set deleted = '1' where repttempid = '{reptTempModel.ReptTempId}'";
            return Orm.Execute(sql) > 0;
        }

        public int GetMaxCount()
        {
            var sql = $"select count(*) from t_repttemp";
            return Orm.QueryFirst<int>(sql);
        }
    }
}
