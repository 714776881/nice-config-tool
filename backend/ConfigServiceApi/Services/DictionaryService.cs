using ConfigServiceApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConfigServiceApi.Models;

namespace ConfigServiceApi.Services
{
    public class DictionaryService
    {
        private readonly ModalityRepository _dicRepository;
        private readonly ExamItemRepository _examItemRepository;

        public DictionaryService() {
            _dicRepository = new ModalityRepository();
            _examItemRepository = new ExamItemRepository();
        }

        public List<TModalityEntity> GetModality()
        {
            var modalitys = _dicRepository.GetModality();

            // 去重
            modalitys = modalitys.GroupBy(x => x.Modality).Select(x => x.First()).ToList();

            return modalitys;
        }

        public List<TExamItemEntity> GetExamItems(string bodypartId)
        {
            var examItemNodes = _examItemRepository.GetExamItems(bodypartId);

            return examItemNodes;
        }
    }
}
