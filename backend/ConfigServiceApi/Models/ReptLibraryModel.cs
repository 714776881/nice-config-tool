namespace ConfigServiceApi.Models
{
    public class ReptLibState
    {
        public string? UserId { get; set; }
        public string? UserCode { get; set; }
        public string? RegionId { get; set; }
        public string? HospitalId { get; set; }
        public string? DepartmentId { get; set; }
        public ECategoryType? CategoryType { get; set; }
        public bool IsLoadExamItem { get; set; }
    }

    public class ReptCatelogVO
    {
        public string? modality { get; set; }
        public List<Node> nodes { get; set; }

    }

    public class AddCatologForm
    {
        public ReptLibState State { get; set; }
        public Node ParentNode { get; set; }
        public Node AddNode { get; set; }
    }
    public class CopyMoveCatologForm
    {
        public ReptLibState State { get; set; }
        public Node Node { get; set; }
        public Node TargetNode { get; set; }
    }


    public class CatelogNodeForm
    {
        public Node Node { get; set; }
    }

    public class Node
    {
        public string key { get; set; }
        public string? title { get; set; }
        public ENodeType? type { get; set; }
        public bool isPublic { get; set; } // 区分公有知识库和私有知识库
        public bool isShare { get; set; } // 当既需要区分公有知识库和私有知识库，则需要区分是否两者都有
        public List<Node>? children { get; set; }
    }

    public enum ECategoryType
    {
        ReptTemp = 1002,
        SpecDes = 1003,
        GMMacr = 1004,
        ExamItem = 1005,
    }

    public enum ENodeType
    {
        BodyPart,
        ExamItem,
        Category,
        ReportTemplate,
    }

    public class TModalityEntity
    {
        // 检查类型
        public string? Modality { get; set; }
        public string? ModalityExpand { get; set; }
        // 描述
        public string? Remark { get; set; }
        public string? ModalityFormat { get; set; }
        public string? ModalityName { get; set; }
        public string? MaxidName { get; set; }
        public string? PatientidFormat { get; set; }
        public string? AutomergeMode { get; set; }
        public string? DeptCode { get; set; }
        public string? DeptName { get; set; }
    }
    public class TBodypartEntity
    {
        // 部位ID
        public string? BodypartId { get; set; }
        // 部位编码
        public string? BodypartCode { get; set; }
        // 部位名称
        public string? Bodypart { get; set; }
        // 部位英文名称
        public string? BodypartEng { get; set; }
        // 删除标识
        public string? Deleted { get; set; }
        // 排序
        public string? Sequence { get; set; }
        // 检查类型
        public string? Modality { get; set; }
    }

    public class TExamItemEntity
    {
        // 检查项目ID
        public string? ExamItemId { get; set; }
        // 检查项目编码
        public string? ExamItemCode { get; set; }
        // 检查项目
        public string? ExamItem { get; set; }
        // 检查项目英文名称
        public string? ExamItemEng { get; set; }
        // 上属部位ID
        public string? BodyPartId { get; set; }
        // 删除标识
        public string? Deleted { get; set; }
        // 排序
        public string? Sequence { get; set; }
    }
}
