
using ConfigService.Business;
using ConfigService.DataModel;
using Tool;
using XService;
using XViewer.Model.Data;

namespace ConfigServiceHost.Business
{
    class TerminalConfigLoadProcessor : ConfigRequestProcessor
    {
        public TerminalConfigLoadProcessor(XConnection connection)
            : base(connection)
        {
        }

        public override void Process(MemoryStream packet)
        {
            ETerminalType terminalType = GetTerminalType(packet);
            string account = DecodeHelper.DecodeStringTag(packet, Params.PARAMETER_ACCOUNT);
            bool loaddefault = IsNeedLoadDefaultConfig(packet);

            if (string.IsNullOrEmpty(account))
            {
                account = m_Connection.ClientIP();
            }

            //构造响应包
            MemoryStream response = null;
            string config = ConfigCenter.Instance.GetConfig(EConfigType.E_TerminalConfig, terminalType, "", "", account, loaddefault);
            if (!string.IsNullOrEmpty(config))
            {
                response = CreateValidResponse(Cmds.LoadTerminalConfigResponse, config);
            }
            else
            {
                response = EncodeHelper.CreateInvalidResponse(Cmds.LoadTerminalConfigResponse, "No Terminal Configuration!");
            }

            //发送响应
            m_Connection.Response(response);
        }
    }
}