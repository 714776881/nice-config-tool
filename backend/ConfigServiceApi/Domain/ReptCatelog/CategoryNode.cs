using ConfigServiceApi.Models;
using ConfigServiceApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigServiceApi.Services;

namespace ConfigServiceApi.Domain
{
    /**
     * 
    public class CategoryNode : Node
    {
        private CategoryRepository _categoryRepository;
        public CategoryNode()
        {
            _categoryRepository = new CategoryRepository();
        }
        public List<Node> GetCategoryNodesByBoydpartId(string bodypartId, ReptLibStateForm state)
        {
            var nodes = new List<Node>();
            // 分组节点
            var bodypartCategorys = _categoryRepository.GetCagetoryByBodypartId(bodypartId, state).OrderBy((x) =>
            {
                return x.Ownerid;
            });
            foreach (var category in bodypartCategorys)
            {
                nodes.Add(LoadCategoryNode(category, state));
            }
            return nodes;
        }

        public List<Node> GetCategoryNodesByExamItemId(string examItemId, ReptLibStateForm state)
        {
            var nodes = new List<Node>();
            // 分组节点
            var categorys = _categoryRepository.GetCagetoryByExamItemId(examItemId, state);
            foreach (var category in categorys)
            {
                nodes.Add(LoadCategoryNode(category, state));
            }
            // 模板节点
            var reptTemps = _repttempRepository.GetReptTempByCategoryId(examItemId, state.UserId);
            foreach (var reptTemp in reptTemps)
            {
                nodes.Add(CreateReptTempNode(reptTemp));
            }
            return nodes;
        }
    }
    */
}
