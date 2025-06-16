using ConfigServiceApi.Models;
using Microsoft.AspNetCore.Mvc;
using ConfigServiceApi.Services;

namespace ConfigServiceHost.ApiControllers
{
    [ApiController]
    public class ReptCatelogController : ControllerBase
    {
        public ReptCatelogService _service;
        public ReptCatelogController()
        {
            _service = new ReptCatelogService();
        }

        [HttpPost]
        [Route("Api/ReptCatelog/GetCatelog")]
        public ApiResponse GetCatelog(string modality, [FromBody]ReptLibState state)
        {
            var res = new ApiResponse();
            try
            {
                if (string.IsNullOrEmpty(modality) || state == null)
                {
                    res.code = ApiResponse.Error;
                    res.message = ApiResponse.ParamsErrorMsg;
                }

                var reptCatelogNodes = _service.GetReptCatelog(modality, state);

                var reptCatelog = new ReptCatelogVO();
                reptCatelog.modality = modality;
                reptCatelog.nodes = reptCatelogNodes;

                res.data = reptCatelog;
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
        [Route("Api/ReptCatelog/AddCatelogNode")]
        public ApiResponse AddCatelogNode([FromBody]AddCatologForm form )
        {
            var res = new ApiResponse();
            try
            {
                if (form.State == null || form.ParentNode == null )
                {
                    res.code = ApiResponse.Error;
                    res.message = ApiResponse.ParamsErrorMsg;
                }

                var addNodeId = _service.AddCatelogNode(form.AddNode,form.ParentNode , form.State);

                res.code = ApiResponse.Success;
                res.data = addNodeId;
            }
            catch(Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        [Route("Api/ReptCatelog/UpdateCatelogNode")]
        public ApiResponse UpdateCatelogNode([FromBody] CatelogNodeForm form)
        {
            var res = new ApiResponse();
            try
            {
                if (form.Node == null || string.IsNullOrEmpty(form.Node.key))
                {
                    res.code = ApiResponse.Error;
                    res.message = ApiResponse.ParamsErrorMsg;
                }

                var addNodeId = _service.UpdateCatelogNode(form.Node);

                res.code = ApiResponse.Success;
                res.data = addNodeId;
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        [Route("Api/ReptCatelog/DeleteCatelogNode")]
        public ApiResponse DeleteCatelogNode([FromBody] CatelogNodeForm form)
        {
            var res = new ApiResponse();
            try
            {
                if (form.Node == null || string.IsNullOrEmpty(form.Node.key))
                {
                    res.code = ApiResponse.Error;
                    res.message = ApiResponse.ParamsErrorMsg;
                }

                var addNodeId = _service.DeleteCatelogNode(form.Node);

                res.code = ApiResponse.Success;
                res.data = addNodeId;
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        [Route("Api/ReptCatelog/CopyCatelogNode")]
        public ApiResponse CopyCatelogNode([FromBody] CopyMoveCatologForm form)
        {

            var res = new ApiResponse();
            try
            {
                if (form.Node == null)
                {
                    res.code = ApiResponse.Error;
                    res.message = ApiResponse.ParamsErrorMsg;
                }

                var nodeId = _service.CopyCatelogNode(form.Node,form.TargetNode,form.State);
                if(form.Node.type == ENodeType.Category)
                {
                    var node = _service.GetCategoryNode(nodeId, form.State);
                    res.data = node;
                }
                else
                {
                    res.data = nodeId;
                }
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
        [Route("Api/ReptCatelog/MoveCatelogNode")]
        public ApiResponse MoveCatelogNode([FromBody] CopyMoveCatologForm form)
        {
            var res = new ApiResponse();
            try
            {
                if (form.Node == null)
                {
                    res.code = ApiResponse.Error;
                    res.message = ApiResponse.ParamsErrorMsg;
                }

                var result = _service.MoveCatelogNode(form.Node, form.TargetNode, form.State);

                res.code = ApiResponse.Success;
                res.data = result;
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