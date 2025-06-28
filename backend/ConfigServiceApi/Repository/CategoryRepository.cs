using ConfigServiceApi.Models;
using ConfigServiceApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Repository
{
    internal class CategoryRepository : RepositoryBase
    {
        public new string TableName = "t_repttempcategory";
        public new string IdName = "categoryid";
        public TRepttempCategoryEntity GetCategoryById(string categoryId)
        {
            var sql = $"select * from t_repttempcategory where categoryId = '{categoryId}'";
            return Orm.QueryFirst<TRepttempCategoryEntity>(sql);
        }

        public List<TRepttempCategoryEntity> GetCategoryByPatientId(string patientId)
        {
            var sql = $"select * from t_repttempcategory where parentcategoryId = '{patientId}' and deleted = '0'";
            return Orm.Query<TRepttempCategoryEntity>(sql).ToList();
        }

        public List<TRepttempCategoryEntity> GetCagetoryByBodypartId(string bodypartId, ReptLibState state)
        {
            var sql = $"select * from t_repttempcategory where bodypartId = '{bodypartId}'  and regionId = '{state.RegionId}' and hospitalID = '{state.HospitalId}' and departmentid = '{state.DepartmentId}' and deleted = '0' ";
            sql += $"and (ownerid = '{state.UserId}' or ownerid is null)";
            sql += $" and categoryType = '{((int?)state.CategoryType)}'";
            return Orm.Query<TRepttempCategoryEntity>(sql).ToList();
        }

        public List<TRepttempCategoryEntity> GetCagetoryByExamItemId(string examItemId, ReptLibState state)
        {
            var sql = $"select * from t_repttempcategory where examItemId = '{examItemId}' and regionId = '{state.RegionId}' and hospitalID = '{state.HospitalId}' and departmentid = '{state.DepartmentId}' and deleted = '0' ";
            sql += $" and (ownerid = '{state.UserId}' or ownerid is null)";
            sql += $" and categoryType = '{((int?)state.CategoryType)}'";
            return Orm.Query<TRepttempCategoryEntity>(sql).ToList();
        }

        public List<TRepttempCategoryEntity> GetExamItemCagetory(string bodypartId, ReptLibState state)
        {
            var sql = $"select * from t_repttempcategory where bodypartId = '{bodypartId}' and regionId = '{state.RegionId}' and hospitalID = '{state.HospitalId}' and departmentid = '{state.DepartmentId}' and deleted = '0' ";
            sql += $" and (ownerid = '{state.UserId}' or ownerid is null) and categoryType = '{((int)ECategoryType.ExamItem)}'";
            return Orm.Query<TRepttempCategoryEntity>(sql).ToList();
        }

        public bool Remove(string bodypartId)
        {
            var sql = $"delete from t_repttempcategory where bodypartId = '{bodypartId}' and categoryType = '{((int)ECategoryType.ExamItem)}'";
            return Orm.Execute(sql) > 0;
        }


        public bool Add(TRepttempCategoryEntity model)
        {
            return Orm.Insert(model, TableName) >= 0;
        }

        public bool Update(TRepttempCategoryEntity model)
        {
            
            return Orm.Update(model, TableName, IdName) >= 0;
        }

        public bool Delete(TRepttempCategoryEntity model)
        {
            var sql = $"update t_repttempcategory set deleted = '1' where categoryId = '{model.CategoryId}'";
            return Orm.Execute(sql) > 0;
        }

        public int GetMaxCount()
        {
            var sql = $"select count(*) from t_repttempcategory";
            return Orm.QueryFirst<int>(sql);
        }

    }
}