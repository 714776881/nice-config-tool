using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Models
{
    public class ScopeVO
    {
        public List<ScopeEntity>? Options;
    }


    public class ScopeEntity
    {
        public string? RegionCode { get; set; }
        public string? RegionName { get; set; }
        public string? HospitalCode { get; set; }
        public string? HospitalName { get; set; }
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
    }
}
