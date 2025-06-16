
using ConfigService.Business;
using ConfigService.DataModel;
using Tool;
using XService;
using XViewer.Model.Data;

namespace ConfigServiceHost.Business
{
    class GeneralConfigLoadProcessor : ConfigRequestProcessor
    {
        public GeneralConfigLoadProcessor(XConnection connection)
            : base(connection)
        {
        }

        public override void Process(MemoryStream packet)
        {
            ETerminalType terminalType = GetTerminalType(packet);
            Console.WriteLine($"terminalType={terminalType}");

            //构造响应包
            MemoryStream response = null;
            string config = ConfigCenter.Instance.GetConfig(EConfigType.E_GeneralConfig, terminalType, "", "", "");
            if (!string.IsNullOrEmpty(config))
            {
                response = CreateValidResponse(Cmds.LoadGeneralConfigResponse, config);
            }
            else
            {
                response = EncodeHelper.CreateInvalidResponse(Cmds.LoadGeneralConfigResponse, "No Configuration!");
            }

            //发送响应
            m_Connection.Response(response);
        }
    }
}