
using ConfigService.Business;
using ConfigService.DataModel;
using Tool;
using XService;
using XViewer.Model.Data;

namespace ConfigServiceHost.Business
{
    class DepartmentConfigSaveProcessor : ConfigRequestProcessor
    {
        public DepartmentConfigSaveProcessor(XConnection connection)
            : base(connection)
        {
        }

        public override void Process(MemoryStream packet)
        {
            ETerminalType terminalType = GetTerminalType(packet);
            string department = DecodeHelper.DecodeStringTag(packet, Params.PARAMETER_Department);
            string config = ParseConfig(packet);

            MemoryStream response = null;
            bool bRet = ConfigCenter.Instance.SaveConfig(EConfigType.E_DepartmentConfig, terminalType, department, "", "", config);
            if (true == bRet)
            {
                response = CreateValidResponse(Cmds.SaveConfigRsp, "success");
            }
            else
            {
                response = EncodeHelper.CreateInvalidResponse(Cmds.SaveConfigRsp, "save personal Configuration failed!");
            }

            //发送响应
            m_Connection.Response(response);
        }
    }
}