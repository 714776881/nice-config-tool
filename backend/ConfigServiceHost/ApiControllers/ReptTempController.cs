using ConfigServiceApi.Models;
using ConfigServiceApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text;

namespace ConfigServiceHost.ApiControllers
{
    [ApiController]
    public class ReptTempController : ControllerBase
    {
        private ReptTempService _service;

        public ReptTempController()
        {
            _service = new ReptTempService();
        }

        [HttpPost]
        [Route("Api/ReptTemp/Get")]
        public ApiResponse GetReptTemp(string reptTempId)
        {
            var res = new ApiResponse();
            try
            {
                if(string.IsNullOrEmpty(reptTempId)) {
                    res.code = ApiResponse.Error;
                    res.message = "参数错误";
                    return res;
                }

                var entity = _service.GetReportTemplate(reptTempId);
                if(entity == null)
                {
                    res.code = ApiResponse.Error;
                    res.message = "参数错误";
                    return res;
                }

                var result = new ReptTempVO()
                {
                    reptTempId = entity.ReptTempId,
                    reptTemp = entity.ReptTemp,
                    studyResult = entity.CStudyResult == null ? entity.StudyResult : entity.CStudyResult,
                    diagResult = entity.CDiagResult == null ? entity.DiagResult : entity.CDiagResult,
                    isPublic = string.IsNullOrEmpty(entity.OwnerId) ? true : false,
                    modifyDT = entity.ModifyDT
                };

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
        [Route("Api/ReptTemp/Add")]
        public ApiResponse AddReptTemp([FromBody]TReptTempEntity reptTempModel)
        {
            var res = new ApiResponse();
            try
            {
                var result = _service.Add(reptTempModel);

                if(result)
                {
                    res.code = ApiResponse.Success;
                    res.message = "添加成功！";
                }
                else
                {
                    res.code = ApiResponse.Error;
                    res.message = "添加失败";
                }
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        [Route("Api/ReptTemp/Update")]
        public ApiResponse UpdateReptTemp([FromBody] ReptTempForm form)
        {
           var res = new ApiResponse();
            try
            {
                if(string.IsNullOrEmpty(form.ReptTempId))
                {
                    res.code = ApiResponse.Error;
                    res.message = "参数错误！";
                }

                var userId = HttpContext.Session.GetString("UserId");
                var result = _service.Update(form, userId);

                if (result)
                {
                    res.code = ApiResponse.Success;
                    res.message = "上传成功！";
                }
                else
                {
                    res.code = ApiResponse.Error;
                    res.message = "上传成功";
                }
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