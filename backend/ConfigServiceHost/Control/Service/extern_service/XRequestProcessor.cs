namespace XService
{
    class XRequestProcessor
    {
        public XRequestProcessor(XConnection connection)
        {
            m_Connection = connection;
        }

        public virtual void Process(MemoryStream packet) { }

        protected XConnection m_Connection = null;
    }
}