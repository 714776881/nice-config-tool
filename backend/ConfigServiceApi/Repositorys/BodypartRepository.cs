using ConfigServiceApi.Models;
using ConfigServiceApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Repository
{
    internal class BodyPartRepository : RepositoryBase
    {
        public List<TBodypartEntity> GetBodyparts(string modality, ReptLibState state)
        {
            var sql = $"select * from t_bodypart where modality = '{modality}' and regionId = '{state.RegionId}' and hospitalid = '{state.HospitalId}' and deleted = '0'  order by sequence";
            return Orm.Query<TBodypartEntity>(sql).ToList();
        }

        public TBodypartEntity GetBodyPartById(string bodypartId)
        {
            var sql = $"select * from t_bodypart where bodypartid = '{bodypartId}' and deleted = '0'";
            return Orm.Query<TBodypartEntity>(sql).FirstOrDefault();
        }

    }
}
