using ConfigServiceApi.Models;
using ConfigServiceApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Repository
{
    internal class UserRepository : RepositoryBase
    {
        public List<TUserEntity> GetUsersByUserCode(string userCode)
        {
            var sql = $"select * from t_user where userCode = '{userCode}'";
            return Orm.Query<TUserEntity>(sql).ToList();
        }

        public TUserEntity GetUserById(string userId)
        {
            var sql = $"select * from t_user where userid = '{userId}'";
            return Orm.QueryFirst<TUserEntity>(sql);
        }
    }
}
