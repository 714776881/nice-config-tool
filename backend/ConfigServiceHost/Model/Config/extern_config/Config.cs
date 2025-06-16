
namespace Model.Config
{
    class Config
    {
        public static Config Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public string UID { get; set; }

        Config()
        {
            UID = "UnknownService";
        }

        static Config m_Instance = new Config();
    }
}
