using ConfigServiceApi.Models;
using ConfigServiceApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Repository
{
    internal class ModalityRepository : RepositoryBase
    {
        public List<TModalityEntity> GetModality()
        {
            var sql = $"select m.* from t_modality_config m";
            return Orm.Query<TModalityEntity>(sql).ToList();
        }
    }
}
