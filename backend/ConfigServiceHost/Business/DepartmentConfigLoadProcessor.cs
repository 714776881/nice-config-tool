
using ConfigService.Business;
using ConfigService.DataModel;
using Tool;
using XService;
using XViewer.Model.Data;

namespace ConfigServiceHost.Business
{
    class DepartmentConfigLoadProcessor : ConfigRequestProcessor
    {
        public DepartmentConfigLoadProcessor(XConnection connection)
            : base(connection)
        {
        }

        public override void Process(MemoryStream packet)
        {
            //解析账号
            ETerminalType terminalType = GetTerminalType(packet);
            string department = DecodeHelper.DecodeStringTag(packet, Params.PARAMETER_Department);
            bool loaddefault = IsNeedLoadDefaultConfig(packet);

            //构造响应包
            MemoryStream response = null;
            Dictionary<string, string> departmentDic = new Dictionary<string, string>();
            if (true == ConfigCenter.Instance.HasDepartmentConfig(department, terminalType, ref departmentDic))
            {
                if (departmentDic == null || departmentDic.Count == 0)
                {
                    response = EncodeHelper.CreateInvalidResponse(Cmds.LoadDepartmentConfigResponse, "No department Configuration!");
                }
                else
                {
                    string config = ConfigCenter.Instance.GetDepartmentConfig(terminalType, departmentDic);
                    response = CreateValidResponse(Cmds.LoadDepartmentConfigResponse, config);
                }
            }
            else
            {
                response = EncodeHelper.CreateInvalidResponse(Cmds.LoadDepartmentConfigResponse, "No department Configuration!");
            }

            //发送响应
            m_Connection.Response(response);
        }
    }
}