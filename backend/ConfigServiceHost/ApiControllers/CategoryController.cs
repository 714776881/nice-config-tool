using ConfigServiceApi.Models;
using Microsoft.AspNetCore.Mvc;
using ConfigServiceApi.Services;

namespace ConfigServiceHost.ApiControllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public CategoryService _service;
        public CategoryController()
        {
            _service = new CategoryService();
        }

        [HttpPost]
        [Route("Api/Category/GetExamItemCategory")]
        public ApiResponse GetExamItemCategory(string modality,bool isPublic, [FromBody] ReptLibState state)
        {
            var res = new ApiResponse();
            try
            {
                if (string.IsNullOrEmpty(modality) || state == null)
                {
                    res.code = ApiResponse.Error;
                    res.message = ApiResponse.ParamsErrorMsg;
                }

                var examItemCategory = _service.GetExamItemCategory(modality, state, isPublic);

                var result = examItemCategory.Select(x => x.ExamitemId).ToList();

                res.data = result;
                res.code = ApiResponse.Success;
            } 
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        [Route("Api/Category/GetExamItemCategoryByBodypartId")]
        public ApiResponse GetExamItemCategoryByBodypartId(string bodyPartId, bool isPublic, [FromBody] ReptLibState state)
        {
            var res = new ApiResponse(); 
            try
            {
                if (string.IsNullOrEmpty(bodyPartId) || state == null)
                {
                    res.code = ApiResponse.Error;
                    res.message = ApiResponse.ParamsErrorMsg;
                }

                var examItemCategory = _service.GetExamItemCategoryByBodypartId(bodyPartId, state, isPublic);
                var result = examItemCategory.Select(x => x.ExamitemId).ToList();

                res.data = result;
                res.code = ApiResponse.Success;
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        [Route("Api/Category/AddExamItemCategory")]
        public ApiResponse AddExamItemCategory(string bodyPartId,bool isPublic,[FromBody]AddExamItemCategoryForm form )
        {
            var res = new ApiResponse();
            try
            {
                if (string.IsNullOrEmpty(bodyPartId) || form == null)
                {
                    res.code = ApiResponse.Error;
                    res.message = ApiResponse.ParamsErrorMsg;
                }

                var result = _service.AddExamItemCategory(bodyPartId, form.examItemIds, form.state, isPublic);

                res.code = ApiResponse.Success;
                res.data = result;
            }
            catch(Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }
    }
}
