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
        public List<TUserEntity> GetUsers(string userId)
        {
            var sql = $"select * from t_user where userId = '{userId}'";
            return Orm.Query<TUserEntity>(sql).ToList();
        }
    }
}
