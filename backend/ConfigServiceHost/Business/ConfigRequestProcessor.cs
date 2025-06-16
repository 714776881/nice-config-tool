
using ConfigService.DataModel;
using Tool;
using XService;
using XViewer.Model.Data;

namespace ConfigServiceHost.Business
{
    class ConfigRequestProcessor : XRequestProcessor
    {
        public ConfigRequestProcessor(XConnection connection)
            : base(connection)
        {
        }

        protected bool IsNeedLoadDefaultConfig(MemoryStream packet)
        {
            bool ret = false;
            string configtype = DecodeHelper.DecodeStringTag(packet, Params.PARAMETER_LOAD_CONFIG_TYPE);
            if ("default" == configtype)
            {
                ret = true;
            }

            return ret;
        }

        protected string ParseConfig(MemoryStream packet)
        {
            int buflen = 2;
            byte[] buffer = new byte[buflen];
            packet.Read(buffer, 0, buflen);

            string config = "";
            Params tag = DecodeHelper.DecodeTag(buffer);
            if (Params.PARAMETER_CONFIG == tag)
            {
                byte[] btaglen = new byte[4];
                packet.Read(btaglen, 0, 4);
                int taglen = BitConverter.ToInt32(btaglen, 0);
                byte[] tagvalue = new byte[taglen];
                packet.Read(tagvalue, 0, taglen);
                config = System.Text.Encoding.UTF8.GetString(tagvalue, 0, taglen);
            }

            return config;
        }

        protected MemoryStream CreateValidResponse(Cmds command, string config)
        {
            MemoryStream data = EncodeHelper.CreateResponseHeader(command, 1);

            byte[] bconfig = System.Text.Encoding.UTF8.GetBytes(config);
            EncodeHelper.EncodeTag(data, Params.PARAMETER_CONFIG);
            int configlen = bconfig.Length;
            // [TODO] 此处写入的长度字段值没有转成大端，因多个终端此处也是按小端处理，暂不修改
            byte[] bconfiglen = BitConverter.GetBytes(configlen);
            data.Write(bconfiglen, 0, bconfiglen.Length);
            data.Write(bconfig, 0, bconfig.Length);

            return EncodeHelper.FormatPacket(data);
        }

        protected ETerminalType GetTerminalType(MemoryStream packet)
        {
            byte bType = DecodeHelper.DecodeByteTag(packet, Params.PARAMETER_ClientType);
            return (ETerminalType)bType;
        }
    }
}