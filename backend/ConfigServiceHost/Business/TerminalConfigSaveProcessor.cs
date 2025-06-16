
using ConfigService.Business;
using ConfigService.DataModel;
using Tool;
using XService;
using XViewer.Model.Data;

namespace ConfigServiceHost.Business
{
    class TerminalConfigSaveProcessor : ConfigRequestProcessor
    {
        public TerminalConfigSaveProcessor(XConnection connection)
            : base(connection)
        {
        }

        public override void Process(MemoryStream packet)
        {
            ETerminalType terminalType = GetTerminalType(packet);
            string config = ParseConfig(packet);

            MemoryStream response = null;
            bool bRet = ConfigCenter.Instance.SaveConfig(EConfigType.E_TerminalConfig, terminalType, "", "", m_Connection.ClientIP(), config);
            if (true == bRet)
            {
                response = CreateValidResponse(Cmds.SaveConfigRsp, "success");
            }
            else
            {
                response = EncodeHelper.CreateInvalidResponse(Cmds.SaveConfigRsp, "save terminal Configuration failed!");
            }

            //发送响应
            m_Connection.Response(response);
        }
    }
}