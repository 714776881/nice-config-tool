using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConfigServiceApi.Repository;
using ConfigServiceApi.Models;

namespace ConfigServiceApi.Services
{
    public class ReptTempService
    {
        private readonly ReptTempRepository repttempRepository;

        public ReptTempService()
        {
            repttempRepository = new ReptTempRepository();
        }

        public TReptTempEntity GetReportTemplate(string reptTempId)
        {
            return repttempRepository.GetReptTempById(reptTempId);
        }
        
        public bool Add(TReptTempEntity reptTemp)
        {
            return repttempRepository.Add(reptTemp);
        }

        public bool Update(ReptTempForm form,string userId)
        {
            var entity = repttempRepository.GetReptTempById(form.ReptTempId);
            if(entity == null)
            {
                return false;
            }

            entity.ReptTempId = form.ReptTempId;
            entity.ReptTemp = form.ReptTemp;
            entity.StudyResult = form.StudyResult;
            entity.DiagResult = form.DiagResult;
            if (!string.IsNullOrEmpty(form.CStudyResult))
            {
                entity.CStudyResult = form.CStudyResult;
            }
            if (!string.IsNullOrEmpty(form.CDiagResult))
            {
                entity.CDiagResult = form.CDiagResult;
            }
            entity.IsAbnormal = form.IsAbnormal;
            entity.ModifyDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return repttempRepository.Update(entity);
        }

    }
}
