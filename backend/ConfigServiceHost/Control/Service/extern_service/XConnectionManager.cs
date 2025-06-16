
namespace XService
{
    //拥有一组线程，用于执行各个连接，线程个数放到配置文件中
    class XConnectionManager
    {
        public static XConnectionManager Instance
        {
            get
            {
                return m_Instance;
            }
        }


        //一个连接对应一个执行器
        public void Add(XConnection connection)
        {
            if (null != connection)
            {
                //如果是异步连接，因为该连接的执行会被.net自动分配子线程执行，故无需再分配而外的线程去执行
                if (connection.IsAsync())
                {
                    connection.Execute();
                }
                else
                {
                    XExecuter executer = new XExecuter(m_ExecuterID);
                    executer.Accept(connection);
                    m_ExecuterID += 1;
                    if (int.MaxValue <= m_ExecuterID)
                    {
                        m_ExecuterID = 1;
                    }
                }
            }
        }

        int m_ExecuterID = 1;
        static XConnectionManager m_Instance = new XConnectionManager();
    }
}
