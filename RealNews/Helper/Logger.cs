using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;

namespace RealNews
{
    public interface ILog
    {
        /// <summary>
        /// Fatal log = log level 5
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="objs"></param>
        void Fatal(object msg, params object[] objs); // 5
        /// <summary>
        /// Error log = log level 4
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="objs"></param>
        void Error(object msg, params object[] objs); // 4
        /// <summary>
        /// Warning log = log level 3
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="objs"></param>
        void Warn(object msg, params object[] objs);  // 3
        /// <summary>
        /// Debug log = log level 2 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="objs"></param>
        void Debug(object msg, params object[] objs); // 2
        /// <summary>
        /// Info log = log level 1
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="objs"></param>
        void Info(object msg, params object[] objs);  // 1
    }

    internal class ConsoleLogger
    {
        // Sinlgeton pattern 4 from : http://csharpindepth.com/articles/general/singleton.aspx
        private static readonly ConsoleLogger instance = new ConsoleLogger();
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ConsoleLogger()
        {
        }
        private ConsoleLogger()
        {
        }
        public static ConsoleLogger Instance { get { return instance; } }
        private Queue<string> _log = new Queue<string>();
        private int _lastLogsToKeep = 100;
        internal bool Log2Console = false;

        private string FormatLog(string log, string msg, object[] objs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("HH:mm:ss"));
            sb.Append("|");
            sb.Append(log);
            //sb.Append("|");
            //sb.Append(Thread.CurrentThread.ManagedThreadId.ToString());
            sb.Append("| ");
            sb.Append(msg);

            if (objs != null)
                foreach (object o in objs)
                    if ("" + o != "")
                        sb.AppendLine("" + o);

            return sb.ToString();
        }

        public void Log(string logtype, string msg, params object[] objs)
        {
            var l = FormatLog(logtype, msg, objs);

            lock (_log)
            {
                _log.Enqueue(l);
                while (_log.Count > _lastLogsToKeep)
                    _log.Dequeue();
            }
            if (Log2Console)
                Task.Factory.StartNew(() => Console.WriteLine(l));
        }

        public string[] GetLastLogs()
        {
            return _log.ToArray();
        }
    }

    internal class logger : ILog
    {
        public logger(Type type)
        {
            typename = type.Namespace + "." + type.Name;
        }

        private string typename = "";

        #region ILog Members
        public void Fatal(object msg, params object[] objs)
        {
            ConsoleLogger.Instance.Log("FATAL", "" + msg, objs);
        }

        public void Error(object msg, params object[] objs)
        {
            ConsoleLogger.Instance.Log("ERROR", "" + msg, objs);
        }

        public void Warn(object msg, params object[] objs)
        {
            ConsoleLogger.Instance.Log("WARN", "" + msg, objs);
        }

        public void Debug(object msg, params object[] objs)
        {
            ConsoleLogger.Instance.Log("DEBUG", "" + msg, objs);
        }

        public void Info(object msg, params object[] objs)
        {
            ConsoleLogger.Instance.Log("INFO", "" + msg, objs);
        }
        #endregion
    }

    public static class LogManager
    {
        public static ILog GetLogger(Type obj)
        {
            return new logger(obj);
        }

        public static void ConsoleMode()
        {
            ConsoleLogger.Instance.Log2Console = true;
        }

        public static string[] GetLastLogs()
        {
            return ConsoleLogger.Instance.GetLastLogs();
        }
    }
}
