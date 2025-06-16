using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ConfigServiceApi.Models;
using ConfigServiceApi.Repository;

namespace ConfigServiceApi.Services
{
    public class CategoryService
    {
        private readonly ExamItemRepository _examItemRepository;
        private readonly BodyPartRepository _bodyPartRepository;
        private readonly CategoryRepository _categoryRepository;

        public CategoryService()
        {
            _examItemRepository = new ExamItemRepository();
            _bodyPartRepository = new BodyPartRepository();
            _categoryRepository = new CategoryRepository();
        }

        public List<TRepttempCategoryEntity> GetExamItemCategory(string modality, ReptLibState state, bool isPublic)
        {
            var categorys = new List<TRepttempCategoryEntity>();
            // 检查部位
            var bodyparts = _bodyPartRepository.GetBodyparts(modality, state);
            foreach (var bodypart in bodyparts)
            {
                // 检查项目
                categorys.AddRange(GetExamItemCategoryByBodypartId(bodypart.BodypartId, state, isPublic));
            }
            return categorys;
        }
        public List<TRepttempCategoryEntity> GetExamItemCategoryByBodypartId(string bodypartId, ReptLibState state, bool isPublic)
        {
            var categorys = new List<TRepttempCategoryEntity>();
            var examItems = _examItemRepository.GetExamItems(bodypartId, state);
            var examItemCategorys = _categoryRepository.GetExamItemCagetory(bodypartId, state);

            foreach (var examitem in examItems)
            {
                var category = examItemCategorys.Find(x =>
                {
                    return x.ExamitemId == examitem.ExamItemId;
                });

                if (category == null) continue;

                bool isOwnerValid = isPublic ? string.IsNullOrEmpty(category.Ownerid) : !string.IsNullOrEmpty(category.Ownerid);
                if (isOwnerValid)
                {
                    categorys.Add(category);
                } 
            }
            return categorys;
        }

        public bool AddExamItemCategory(string bodypartId,List<string> examItemIds,ReptLibState state,bool isPublic)
        {
            _categoryRepository.Remove(bodypartId);
            
            foreach(var examItemId in examItemIds)
            {
                TRepttempCategoryEntity entity = new TRepttempCategoryEntity();
                entity.CategoryId = Guid.NewGuid().ToString();

                entity.ParentCategoryId = examItemId;
                entity.ExamitemId = examItemId;

                entity.Category = "检查项目类型标识";

                if (isPublic == false)
                {
                    entity.Ownerid = state.UserId;
                }
                entity.CategoryType = ((int)ECategoryType.ExamItem).ToString();

                entity.BodypartId = bodypartId;
                entity.RegionId = state.RegionId;
                entity.HospitalId = state.HospitalId;
                entity.DepartmentId = state.DepartmentId;
                entity.Sequence = _categoryRepository.GetMaxCount().ToString();
                entity.Deleted = "0";
                _categoryRepository.Add(entity);
            }
            return true;
        }
    }
}
