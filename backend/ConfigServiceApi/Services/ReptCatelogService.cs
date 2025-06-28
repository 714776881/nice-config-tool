using ConfigServiceApi.Models;
using ConfigServiceApi.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace ConfigServiceApi.Services
{
    public class ReptCatelogService
    {
        private readonly ExamItemRepository _examItemRepository;
        private readonly BodyPartRepository _bodyPartRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly ReptTempRepository _repttempRepository;

        public ReptCatelogService()
        {
            _examItemRepository = new ExamItemRepository();
            _bodyPartRepository = new BodyPartRepository();
            _categoryRepository = new CategoryRepository();
            _repttempRepository = new ReptTempRepository();
        }
        /*
         *  获取个人和公有的模板目录
         *  -检查部位
         *      -检查项目
         *          -分组节点
         *          -模板节点
         *      -分组节点
         *          -子分组节点
         *          -模板节点
         */
        public List<Node> GetReptCatelog(string modality,ReptLibState state)
        {
            // 检查部位
            var bodypartNodes = GetBodypartNodes(modality, state);
            foreach(var bodypartNode in bodypartNodes)
            {
                if (state.IsLoadExamItem)
                {
                    // 检查项目
                    var examItemNodes = GetExamItemNodes(bodypartNode.key, state);
                    foreach (var examItemNode in examItemNodes)
                    {
                        examItemNode.children.AddRange(GetCategoryNodesByExamItemId(examItemNode.key, state));
                    }
                    bodypartNode.children.AddRange(examItemNodes);
                }
                // 分组节点
                bodypartNode.children.AddRange(GetCategoryNodesByBoydpartId(bodypartNode.key, state));
            }
            return bodypartNodes;
        }
        public List<Node> GetCategoryNodesByBoydpartId(string bodypartId, ReptLibState state)
        {
            var nodes = new List<Node>();
            // 分组节点
            var bodypartCategorys = _categoryRepository.GetCagetoryByBodypartId(bodypartId, state).OrderBy((x) =>
            {
                return x.Ownerid;
            });
            foreach (var category in bodypartCategorys)
            {
                nodes.Add(GetCategoryNode(category.CategoryId, state));
            }
            return nodes;
        }

        private List<Node> GetCategoryNodesByExamItemId(string examItemId,ReptLibState state)
        {
            var nodes = new List<Node>();
            // 分组节点
            var categorys = _categoryRepository.GetCagetoryByExamItemId(examItemId, state);
            foreach (var category in categorys)
            {
                nodes.Add(GetCategoryNode(category.CategoryId, state));
            }
            // 模板节点
            var reptTemps = _repttempRepository.GetReptTempNodeByCategoryId(examItemId, state.UserId);
            foreach (var reptTemp in reptTemps)
            {
                nodes.Add(CreateReptTempNode(reptTemp));
            }
            return nodes;
        }

        private List<Node> GetBodypartNodes(string modality,ReptLibState state)
        {
            var bodyparts = _bodyPartRepository.GetBodyparts(modality, state);
            var nodes = new List<Node>();
            foreach (var bodypart in bodyparts)
            {
                nodes.Add(CreateBodyPartNode(bodypart));
            }
            return nodes;
        }

        public Node GetCategoryNode(string categoryId, ReptLibState state)
        {
            var node = CreateCategoryNode(_categoryRepository.GetCategoryById(categoryId));

            // 子分组节点
            var children = _categoryRepository.GetCategoryByPatientId(categoryId);
            foreach (var child in children)
            {
                node.children.Add(GetCategoryNode(child.CategoryId, state));
            }
            // 模板节点
            var reptTemps = _repttempRepository.GetReptTempNodeByCategoryId(categoryId, state.UserId);
            foreach (var reptTemp in reptTemps)
            {
                node.children.Add(CreateReptTempNode(reptTemp));
            }
            return node;
        }

        private List<Node> GetExamItemNodes(string bodypartId, ReptLibState state)
        {
            var nodes = new List<Node>();
            var examItems = _examItemRepository.GetExamItems(bodypartId, state);
            foreach (var examitem in examItems)
            {
                nodes.Add(CreateExamItemNode(examitem, true));
            }
            return nodes;
        }

        private Node CreateBodyPartNode(TBodypartEntity bodypart)
        {
            return new Node()
            {
                key = bodypart.BodypartId,
                title = bodypart.Bodypart,
                type = ENodeType.BodyPart,
                isPublic = true,
                children = new List<Node>()
            };
        }

        private Node CreateExamItemNode(TExamItemEntity examItem,bool IsPublic)
        {
            return new Node()
            {
                key = examItem.ExamItemId,
                title = examItem.ExamItem,
                type = ENodeType.ExamItem,
                isPublic = IsPublic,
                children = new List<Node>()
            };
        }

        private Node CreateCategoryNode(TRepttempCategoryEntity category)
        {
            return new Node()
            {
                key = category.CategoryId,
                title = category.Category,
                type = ENodeType.Category,
                isPublic = string.IsNullOrEmpty(category.Ownerid) ? true : false,
                children = new List<Node>()
            };
        }

        private Node CreateReptTempNode(TReptTempEntity reptTemp)
        {
            return new Node()
            {
                key = reptTemp.ReptTempId,
                title = reptTemp.ReptTemp,
                type = ENodeType.ReportTemplate,
                isPublic = string.IsNullOrEmpty(reptTemp.OwnerId) ? true : false,
                children = null
            };
        }

        public string AddCatelogNode(Node node, Node parentNode, ReptLibState state)
        {
            var nodeId = Guid.NewGuid().ToString();

            if (node.type == ENodeType.Category)
            {
                TRepttempCategoryEntity entity = new TRepttempCategoryEntity();
                entity.CategoryId = nodeId;

                if(parentNode.type == ENodeType.BodyPart)
                {
                    entity.BodypartId = parentNode.key;
                }

                if(parentNode.type == ENodeType.ExamItem)
                {
                    entity.ExamitemId = parentNode.key;
                }

                if(parentNode.type == ENodeType.Category)
                {
                    entity.ParentCategoryId = parentNode.key;
                }

                entity.Category = string.IsNullOrEmpty(node.title) ? "未命名" : node.title;
                entity.CategoryType = ((int)ECategoryType.ReptTemp).ToString();

                if (node.isPublic == false)
                {
                    entity.Ownerid = state.UserId;
                }
                entity.RegionId = state.RegionId;
                entity.HospitalId = state.HospitalId;
                entity.DepartmentId = state.DepartmentId;
                entity.Sequence = _categoryRepository.GetMaxCount();
                entity.Deleted = "0";
                _categoryRepository.Add(entity);
            }
            if(node.type == ENodeType.ReportTemplate)
            {
                TReptTempEntity entity = new TReptTempEntity();
                entity.ReptTempId = nodeId;

                if(parentNode.type == ENodeType.Category)
                {
                    entity.CategoryId = parentNode.key;
                }
                if(parentNode.type == ENodeType.ExamItem)
                {
                    entity.CategoryId = parentNode.key;
                }
                entity.ReptTemp = node.title;
                entity.StudyResult = "";
                entity.DiagResult = "";

                entity.RegionId = state.RegionId;
                entity.HospitalId = state.HospitalId;
                entity.DepartmentId = state.DepartmentId;

                if(node.isPublic == false)
                {
                    entity.OwnerId = state.UserId;
                }
                entity.Sequence = _repttempRepository.GetMaxCount().ToString();

                entity.Deleted = "0";

                _repttempRepository.Add(entity);
            }
            return nodeId;
        }
        public bool UpdateCatelogNode(Node node)
        {
            if(node.type == ENodeType.Category)
            {
                var entity = _categoryRepository.GetCategoryById(node.key);
                entity.Category = node.title;
                return _categoryRepository.Update(entity);
            }
            if(node.type == ENodeType.ReportTemplate)
            {
                var entity = _repttempRepository.GetReptTempById(node.key);
                entity.ReptTemp = node.title;
               return  _repttempRepository.Update(entity);
            }
            return false;
        }

        public bool DeleteCatelogNode(Node node)
        {
            if(node.type == ENodeType.Category)
            {
                var entity = _categoryRepository.GetCategoryById(node.key);
                return _categoryRepository.Delete(entity);
            }
            if(node.type == ENodeType.ReportTemplate)
            {
                var entity = _repttempRepository.GetReptTempById(node.key);
                return _repttempRepository.Delete(entity);
            }
            if(node.type == ENodeType.ExamItem)
            {
                var entity = _categoryRepository.GetCategoryById(node.key);
                return _categoryRepository.Delete(entity);
            }
            return false;
        }

        public string CopyCatelogNode(Node node,Node targetNode, ReptLibState state)
        {
            var nodeId = Guid.NewGuid().ToString();
            if(node.type == ENodeType.Category)
            {
                if(node == null || targetNode == null)
                {
                    throw new ArgumentNullException();
                }
                nodeId = CopyCategoryNode(node, targetNode, state);
            }
            if(node.type == ENodeType.ReportTemplate)
            {
                var entity = _repttempRepository.GetReptTempById(node.key);
                entity.ReptTempId = nodeId;
                entity.ReptTemp += " 副本";
                _repttempRepository.Add(entity);
            }
            return nodeId;
        }

        private string CopyCategoryNode(Node node, Node parentNode,ReptLibState state)
        {
            var nodeId = Guid.NewGuid().ToString();
            var category = _categoryRepository.GetCategoryById(node.key);
            if (category != null)
            {
                category.CategoryId = nodeId;
                category.Category = node.title;

                if (parentNode.type == ENodeType.BodyPart)
                {
                    category.BodypartId = parentNode.key;
                }
                else
                {
                    category.BodypartId = null;
                }
                if (parentNode.type == ENodeType.ExamItem)
                {
                    category.ExamitemId = parentNode.key;
                }
                else
                {
                    category.ExamitemId = null;
                }
                if (parentNode.type == ENodeType.Category)
                {
                    category.ParentCategoryId = parentNode.key;
                }
                else
                {
                    category.ParentCategoryId = null;
                }

                if (node.isPublic == false)
                {
                    category.Ownerid = state.UserId;
                }
                else
                {
                    category.Ownerid = null;
                }
                _categoryRepository.Add(category);
            }

            node.key = nodeId;
            foreach (var child in node.children)
            {
                if (child.type == ENodeType.Category)
                {
                    CopyCategoryNode(child, node, state);
                }
                else if (child.type == ENodeType.ReportTemplate)
                {
                    CopyReportTemplateNode(child, node, state);
                }
            }
            return nodeId;
        }

        private string CopyReportTemplateNode(Node node, Node parentNode,ReptLibState state)
        {
            var nodeId = Guid.NewGuid().ToString();
            var reptTemp = _repttempRepository.GetReptTempById(node.key);
            if (reptTemp != null)
            {
                reptTemp.ReptTempId = nodeId;
                reptTemp.CategoryId = parentNode.key;

                if(node.isPublic == false)
                {
                    reptTemp.OwnerId = state.UserId;
                }
                else
                {
                    reptTemp.OwnerId = null;
                }
                _repttempRepository.Add(reptTemp);
            }
            return nodeId;
        }

        public bool MoveCatelogNode(Node node,Node targetNode, ReptLibState state)
        {
            if(node.type == ENodeType.Category)
            {
                var entity = _categoryRepository.GetCategoryById(node.key);
                if (targetNode.type == ENodeType.Category)
                {   
                    var targetEntity = _categoryRepository.GetCategoryById(targetNode.key);
                    entity.ParentCategoryId = targetEntity.CategoryId;
                    entity.BodypartId = "";
                    entity.ExamitemId = "";
                    _categoryRepository.Update(entity);
                }
                if(targetNode.type == ENodeType.BodyPart)
                {
                    var targetEntity = _bodyPartRepository.GetBodyPartById(targetNode.key);
                    entity.BodypartId = targetEntity.BodypartId;
                    entity.ExamitemId = "";
                    entity.ParentCategoryId = "";
                    _categoryRepository.Update(entity);
                }
                if(targetNode.type == ENodeType.ExamItem)
                {
                    var targetEntity = _examItemRepository.GetExamItemById(targetNode.key);
                    entity.ExamitemId = targetEntity.ExamItemId;
                    entity.BodypartId = "";
                    entity.ParentCategoryId = "";
                    _categoryRepository.Update(entity);
                }
            }
            if(node.type == ENodeType.ReportTemplate)
            {
                var entity = _repttempRepository.GetReptTempById(node.key);
                if (targetNode.type == ENodeType.Category)
                {
                    var targetEntity = _categoryRepository.GetCategoryById(targetNode.key);
                    entity.CategoryId = targetEntity.CategoryId;
                    _repttempRepository.Update(entity);
                }
                if (targetNode.type == ENodeType.ExamItem)
                {
                    var targetEntity = _examItemRepository.GetExamItemById(targetNode.key);
                    entity.CategoryId = targetEntity.ExamItemId;
                    _repttempRepository.Update(entity);
                }
            }
            return true;
        }
    }
}