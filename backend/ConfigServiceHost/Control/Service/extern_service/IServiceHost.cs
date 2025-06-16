namespace XService
{
    class IServiceHost
    {
        public IServiceHost(int port)
        {
            m_ListenPort = port;
        }

        public virtual bool Start() { return false; }//开启服务
        public virtual void Stop() { }//停止服务

        protected int m_ListenPort;
    }
}