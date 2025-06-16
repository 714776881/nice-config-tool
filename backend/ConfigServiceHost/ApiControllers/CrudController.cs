using ConfigService.DataModel;
using ConfigService.Request;
using ConfigServiceApi;
using Microsoft.AspNetCore.Mvc;
using ServiceManager.Tool;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Drawing;

using ConfigServiceApi.Utils;
using ConfigServiceApi.Models;
using ConfigServiceApi.Services;

namespace ConfigServiceHost.ApiControllers
{
    [ApiController]
    public class CrudController : ControllerBase
    {

        private DalService _DalService;
        public CrudController()
        {
            _DalService = new DalService();
        }

        [HttpGet]
        [Route("Api/Crud")]
        public ApiResponse Select(string sql)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var result = new DbDataModel();
                result.data = _DalService.Select(sql);

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

        [HttpPost]
        [Route("Api/Crud/ExeSql")]
        public ApiResponse ExeSql(string sql)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var result = _DalService.ExeSql(sql);

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

        [HttpPost]
        [Route("Api/Crud/ExeBatchSql")]
        public ApiResponse ExeBatchSql([FromBody] BatchSqlModel model)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                string message = string.Empty;
                var result = _DalService.ExeBatchSql(model.sqls);

                if (result == false)
                {
                    res.code = ApiResponse.Error;
                    res.message = message;
                    return res;
                }
                else
                {
                    res.code = ApiResponse.Success;
                    res.data = result;
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