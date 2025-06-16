
using System.Net.Sockets;
using Tool;
using XService;
using XViewer.Model.Data;

namespace ConfigServiceHost.Business
{
    class ConfigConnection : XConnection
    {
        public ConfigConnection(Socket socket, string clientip)
            : base(socket, clientip)
        {
        }

        public override bool IsAsync()
        {
            return true;
        }

        public override void Execute()
        {
            Receive();
        }

        public override void Process(MemoryStream packet)
        {
            try
            {
                //命令字
                int buflen = 3;
                byte[] buffer = new byte[buflen];
                packet.Read(buffer, 0, buflen);
                Params tag = DecodeHelper.DecodeTag(buffer);
                if (Params.PARAMETER_COMMANDID == tag)
                {
                    packet.Read(buffer, 0, 1);
                    ConfigRequestProcessor crp = null;
                    Cmds command = (Cmds)buffer[0];
                    switch (command)
                    {
                        case Cmds.LoadGeneralConfig:
                            {
                                crp = new GeneralConfigLoadProcessor(this);
                                break;
                            }
                        case Cmds.LoadDepartmentConfig:
                            {
                                crp = new DepartmentConfigLoadProcessor(this);
                                break;
                            }
                        case Cmds.LoadTerminalConfig:
                            {
                                crp = new TerminalConfigLoadProcessor(this);
                                break;
                            }
                        case Cmds.LoadPersonalConfig:
                            {
                                crp = new PersonalConfigLoadProcessor(this);
                                break;
                            }
                        case Cmds.LoadAuthorityConfig:
                            {
                                crp = new AuthorityConfigLoadProcessor(this);
                                break;
                            }
                        case Cmds.SaveDepartmentConfig:
                            {
                                crp = new DepartmentConfigSaveProcessor(this);
                                break;
                            }
                        case Cmds.SavePersonalConfig:
                            {
                                crp = new PersonalConfigSaveProcessor(this);
                                break;
                            }
                        case Cmds.SaveTerminalConfig:
                            {
                                crp = new TerminalConfigSaveProcessor(this);
                                break;
                            }
                        case Cmds.SaveAuthorityConfig:
                            {
                                crp = new AuthorityConfigSaveProcessor(this);
                                break;
                            }
                        default:
                            {
                                Log.Instance.Debug("unknown command:" + command.ToString());
                                break;
                            }
                    }
                    if (null != crp)
                    {
                        crp.Process(packet);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Instance.Exception("parse log packet error:" + e.Message);
            }
        }
    }
}