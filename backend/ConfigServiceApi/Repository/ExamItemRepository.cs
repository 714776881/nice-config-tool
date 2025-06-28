using ConfigServiceApi.Models;
using ConfigServiceApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Repository
{
    internal class ExamItemRepository : RepositoryBase
    {
        public TExamItemEntity GetExamItemById(string examItemId)
        {
            var sql = $"select * from t_examitem where examitemid = '{examItemId}'";
            return Orm.Query<TExamItemEntity>(sql).FirstOrDefault();
        }
        public List<TExamItemEntity> GetExamItems(string bodypartId, ReptLibState state)
        {
            var sql = $"select * from t_examitem where bodypartId = '{bodypartId}' and regionId = '{state.RegionId}' and hospitalcode = '{state.HospitalId}' and departmentid = '{state.DepartmentId}' and deleted = '0'  order by sequence";
            return Orm.Query<TExamItemEntity>(sql).ToList();
        }

        public List<TExamItemEntity> GetExamItems(string bodypartId)
        {
            var sql = $"select * from t_examitem where bodypartId = '{bodypartId}' and deleted = '0'  order by sequence";
            return Orm.Query<TExamItemEntity>(sql).ToList();
        }

    }
}
