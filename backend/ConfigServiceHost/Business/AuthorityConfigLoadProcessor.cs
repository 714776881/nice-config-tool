
using ConfigService.Business;
using ConfigService.DataModel;
using Tool;
using XService;
using XViewer.Model.Data;

namespace ConfigServiceHost.Business
{
    class AuthorityConfigLoadProcessor : ConfigRequestProcessor
    {
        public AuthorityConfigLoadProcessor(XConnection connection)
            : base(connection)
        {
        }

        public override void Process(MemoryStream packet)
        {
            ETerminalType terminalType = GetTerminalType(packet);
            string account = DecodeHelper.DecodeStringTag(packet, Params.PARAMETER_ACCOUNT);
            bool loaddefault = IsNeedLoadDefaultConfig(packet);

            //构造响应包
            MemoryStream response = null;
            string config = ConfigCenter.Instance.GetConfig(EConfigType.E_AuthorityConfig, terminalType, "", account, "", loaddefault);
            if (!string.IsNullOrEmpty(config))
            {
                response = CreateValidResponse(Cmds.LoadAuthorityConfigResponse, config);
            }
            else
            {
                response = EncodeHelper.CreateInvalidResponse(Cmds.LoadAuthorityConfigResponse, "No authority Configuration!");
            }

            //发送响应
            m_Connection.Response(response);
        }
    }
}