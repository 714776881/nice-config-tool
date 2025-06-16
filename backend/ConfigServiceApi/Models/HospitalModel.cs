using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace ConfigServiceApi.Models
{
    public class HospitalRequestModel
    {
        public string oldHospitalCode { get; set; }
        public string hospitalCode { get; set; }
        public string hospitalName { get; set; }
        public string templateType { get; set; }
        public string hospitalType { get; set; }
        public string departments { get; set; }
    }
}
