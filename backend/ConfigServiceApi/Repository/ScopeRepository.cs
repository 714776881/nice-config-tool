using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigServiceApi.Models;
using ConfigServiceApi.Repository;

namespace ConfigServiceApi.Repository
{
    public class ScopeRepository: RepositoryBase
    {
        public List<ScopeEntity> GetAll()
        {
            var sql = @"SELECT 
                        r.id AS regionCode,
                        r.regionname,
                        h.hospitalcode,
                        h.hospitalname,
                        d.itemcode AS departmentCode,
                        d.itemname AS departmentName
                    FROM 
                        t_regionconfig r
                    LEFT JOIN 
                        t_Hospitalconfig h ON r.id = h.regionid AND h.deleted = '0'
                    LEFT JOIN 
                        t_dictionary d ON d.hospitalid = h.hospitalcode AND d.diccategory = 1001 AND d.deleted = '0'
                    WHERE 
                        r.deleted = '0'";
            return Orm.Query<ScopeEntity>(sql).ToList();
        }
    }
}