using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Models
{
    public class DbDataModel
    {
        public List<Dictionary<string, string>> data { get; set; }
    }

    public class BatchSqlModel
    {
        public List<string> sqls { get; set; }
    }
}
