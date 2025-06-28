using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Models
{
    public class AddExamItemCategoryForm
    {
        public ReptLibState state { get; set; }
        public List<string> examItemIds { get; set; }
    }

    public class TRepttempCategoryEntity
    {
        // 分类ID 
        public string CategoryId { get; set; }
        // 部位ID
        public string? BodypartId { get; set; }
        // 检查项目ID
        public string? ExamitemId { get; set; }
        // 类型名称
        public string Category { get; set; }
        // 模板类型
        public string CategoryType { get; set; }
        // 用户ID，空时表示公有
        public string? Ownerid { get; set; }
        // 删除表示
        public string? Deleted { get; set; }
        // 排序
        public int? Sequence { get; set; }
        // 区域ID
        public string? RegionId { get; set; }
        // 院区ID
        public string? HospitalId { get; set; }
        // 科室ID
        public string? DepartmentId { get; set; }
        // 父类ID。表示多级分类
        public string? ParentCategoryId { get; set; }
    }
}

