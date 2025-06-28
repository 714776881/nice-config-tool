using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Models
{
    public class ReptTempForm
    {
        // 模板ID
        public string? ReptTempId { get; set; }
        // 模板名称
        public string? ReptTemp { get; set; }
        // 检查所见
        public string? StudyResult { get; set; }
        public string? CStudyResult { get; set; }
        // 检查诊断
        public string? DiagResult { get; set; }
        public string? CDiagResult { get; set; }

        // 阴阳性
        public int? IsAbnormal { get; set; }
        public bool IsPublic { get; set; }
        // 区域ID
        public string? RegionId { get; set; }
        // 院区ID
        public string? HospitalId { get; set; }
        // 部门ID
        public string? DepartmentId { get; set; }
    }

    public class ReptTempVO
    {
        // 模板ID
        public string? reptTempId { get; set; }
        // 模板名称
        public string? reptTemp { get; set; }
        public string? studyResult { get; set; }
        public string? diagResult { get; set; }

        // 阴阳性
        public int? isAbnormal { get; set; }
        // 是否公有
        public bool isPublic { get; set; }
        // 更新时间
        public string? modifyDT { get; set; }
    }

    public class TReptTempEntity
    {
        // 模板ID
        public string? ReptTempId { get; set; }
        // 分类ID
        public string CategoryId { get; set; }
        // 模板名称
        public string? ReptTemp { get; set; }
        // 检查所见
        public string? StudyResult { get; set; }
        // 检查诊断
        public string? DiagResult { get; set; }
        public string? CStudyResult { get; set; }
        public string? CDiagResult { get; set; }

        // 阴阳性
        public int? IsAbnormal { get; set; }

        // 用户ID，空时表示公有
        public string? OwnerId { get; set; }
        // 删除表示
        public string? Deleted { get; set; }
        // 排序
        public string? Sequence { get; set; }
        // 区域ID
        public string? RegionId { get; set; }
        // 院区ID
        public string? HospitalId { get; set; }
        // 科室ID
        public string? DepartmentId { get; set; }
        // 最后一次修改时间
        public string? ModifyDT { get; set; }
    }
}
