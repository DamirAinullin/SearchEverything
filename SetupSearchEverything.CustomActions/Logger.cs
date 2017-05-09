using System;
using Microsoft.Deployment.WindowsInstaller;

namespace SetupSearchEverything.CustomActions
{
    public class Logger
    {
        private readonly string _name;
        private readonly Session _session;

        public Logger(Session session, string customActionName)
        {
            _session = session;
            _name = customActionName;
        }

        public void Log(string format, params object[] values)
        {
            _session.Log(string.Format("[{0}] [{1}] {2}", _name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                string.Format(format, values)));
        }

        public void Log(string message)
        {
            Log("{0}", message);
        }
    }
}
