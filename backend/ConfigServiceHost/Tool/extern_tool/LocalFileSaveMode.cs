
using static System.Net.Mime.MediaTypeNames;

namespace Tool
{
    class LocalFileSaveMode : LogSaveMode
    {
        public LocalFileSaveMode(int keeps)
        {
            m_Keeps = keeps;//保留的历史日志个数
        }
        public void Save(string str_log)
        {
            try
            {
                //写入文件,以天为单位
                DateTime date = DateTime.Today;
                string logdir = Path.Combine(AppContext.BaseDirectory, "log");
                if (false == Directory.Exists(logdir))
                {
                    Directory.CreateDirectory(logdir);
                    if (null != m_LogFile)
                    {
                        m_LogFile.Close();
                        m_LogFile = null;
                    }
                }
                //构造文件名
                string path = Path.Combine(logdir, date.ToString("yyyy.MM.dd") + ".txt");
                if (!File.Exists(path))
                {
                    m_LogFile = new FileStream(path, FileMode.Append);
                    //没生成一个新日志文件的时候做一次老旧日志清理操作
                    CleanLogFile();
                }
                else
                {
                    if (null == m_LogFile)
                    {
                        m_LogFile = new FileStream(path, FileMode.Append);
                    }
                }
                byte[] content = System.Text.Encoding.UTF8.GetBytes(str_log);
                m_LogFile.Write(content, 0, content.Length);
                m_LogFile.Flush();
            }
            catch (Exception)
            {
            }
        }

        void CleanLogFile()
        {
            try
            {
                List<DateTime> filestamp = new List<DateTime>(0);
                filestamp.Clear();
                Dictionary<DateTime, FileInfo> lookup = new Dictionary<DateTime, FileInfo>(1);
                //遍历临时文件夹
                string logdir = Path.Combine(AppContext.BaseDirectory, "log");
                DirectoryInfo root = new DirectoryInfo(logdir);
                FileInfo[] subs = root.GetFiles();
                foreach (FileInfo child in subs)
                {
                    DateTime timestamp = child.CreationTime;
                    filestamp.Add(timestamp);
                    try
                    {
                        lookup.Add(timestamp, child);
                    }
                    catch (Exception)
                    {
                    }
                }

                //对文件夹按时间排序
                filestamp.Sort();
                //删除时间较久的,仅保留指定个数
                int count = filestamp.Count;
                if (0 < count)
                {
                    int needelete = count - m_Keeps;
                    for (int i = 0; i < needelete; ++i)
                    {
                        DateTime key = filestamp[i];
                        FileInfo subdir = lookup[key];
                        if (null != subdir)
                        {
                            subdir.Delete();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private int m_Keeps = 7;
        private FileStream m_LogFile = null;
    }
}
