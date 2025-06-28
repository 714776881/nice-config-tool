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
using Tool;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConfigServiceHost.ApiControllers
{
    public class CrudReqVO
    {
        public string sql {get;set;}
        public Dictionary<string,string> values { get; set; }
    }


    [ApiController]
    public class CrudController : ControllerBase
    {

        private DalService _DalService;
        public CrudController()
        {
            _DalService = new DalService();
        }

        [HttpPost]
        [Route("Api/Crud/Select")]
        public ApiResponse Select([FromForm] CrudReqVO req)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var result = new DbDataModel();

                result.data = _DalService.Select(req.sql, req.values);

                res.code = ApiResponse.Success;
                res.data = result;
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
                Logger.LogError("[Select]" + ex.Message);
            }
            return res;
        }

        [HttpPost]
        [Route("Api/Crud/ExeSql")]
        public ApiResponse ExeSql([FromForm] CrudReqVO req)
        { 
            ApiResponse res = new ApiResponse();
            try
            {
                if(string.IsNullOrEmpty(req.sql))
                {
                    res.code = ApiResponse.Error;
                    res.message = "SQL为空！";
                }

                res.data = _DalService.ExeSql(req.sql, req.values);
                res.code = ApiResponse.Success;
            }
            catch (Exception ex)
            {
                res.code = ApiResponse.Error;
                res.message = ex.Message;
                Logger.LogError("[ExeSql]" + ex.Message);
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
                Logger.LogError("[ExeBatchSql]" + ex.Message);
            }
            return res;
        }
    }
}