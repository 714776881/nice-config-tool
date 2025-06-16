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
    public class HospitalController : ControllerBase
    {
        #region 修改涉及到医院的配置

        [HttpPost]
        [Route("Api/EditHospital")]
        public bool EditHospitalByPost(HospitalRequestModel model)
        {
            try
            {
                HospitalService.Load(AppContext.BaseDirectory);
                var hospitalControl = new HospitalService();
                hospitalControl.PostEms(model);
                hospitalControl.PostDataSoure(model);
                hospitalControl.PostDataSoureNew(model);
                hospitalControl.PostRisConfigDepartment(model);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return false;
            }
            return true;
        }
        #endregion
    }
}