using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Models
{
    public class DalForm
    {
        public string configFileName { get; set; }
        public string sqlPath { get; set; }
        public Dictionary<string, string> data { get; set; }
    }

    public class DalBatchForm
    {
        public string configFileName { get; set; }
        public string sqlPath { get; set; }
        public List<Dictionary<string, string>> data { get; set; }
    }
}
