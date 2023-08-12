using System;
using System.Diagnostics;

namespace SanatoriumEntities.ServicesClasses
{
    public class Log
    {
        // Trace = 0, Debug = 1, Information = 2, Warning = 3, Error = 4, Critical = 5, and None = 6.
        
        public const int ERR    = 4;
        public const int WRN    = 3;
        public const int INF    = 2;
        public const int DEBUG  = 1;
        public const int TRACE  = 0;

        private static int _level = 1;
        private static Log _instance;
        private static readonly object _lock = new object();
        public static Log Get()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Log();
                    }
                }
            }
            return _instance;
        }

        public int setLevel (int level)
        {
            _level = level;

            return _level;
        }

        public bool put(int level, string message)
        {
            if (level > 5) return true;
            
            var stackTrace = new StackTrace(2);
            string meta = $"{{\"level\":{level},\"deep\":{stackTrace.FrameCount},\"method\":\"{stackTrace.GetFrame(0).GetMethod().Name}\"}}";

            if (_level <= level) {
                
                string logMessage = $"{{\"datetime\":\"{DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff")}\",\"meta\":{meta},\"message\":\"{message}\"}}";

                Console.WriteLine(logMessage);

                return true;
            }
            
            return false;
        }
    }
}
