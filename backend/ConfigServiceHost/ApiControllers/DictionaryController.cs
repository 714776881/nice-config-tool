using ConfigService.DataModel;
using ConfigService.Request;
using ConfigServiceApi;
using Microsoft.AspNetCore.Mvc;
using ServiceManager.Tool;
using System.Reflection;
using System.Text.RegularExpressions;

using ConfigServiceApi.Models;
using ConfigServiceApi.Utils;
using ConfigServiceApi.Services;

namespace ConfigServiceHost.ApiControllers
{
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly DictionaryService _service;

        public DictionaryController()
        {
            _service = new DictionaryService();
        }

        #region 获取模态列表
        [HttpGet]
        [Route("Api/Dictionary/GetModality")]
        public ApiResponse GetModality()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var result = _service.GetModality().Select(x => new DictionaryVO()
                {
                    value = x.Modality.Trim(),
                    label = x.Modality.Trim()
                }).ToList();

                response.code = ApiResponse.Success;
                response.data = result;
            }
            catch (Exception ex)
            {
                response.code = ApiResponse.Error;
                response.message = ex.Message;
            }
            return response;
        }
        #endregion

        [HttpGet]
        [Route("Api/Dictionary/GetExamItems")]
        public ApiResponse GetExamItems(string bodypartId)
        {
            var res = new ApiResponse();
            try
            {
                var result = _service.GetExamItems(bodypartId).Select(x => new DictionaryVO()
                {
                    value = x.ExamItemId,
                    label = x.ExamItem
                }).ToList();

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
    }
}