using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace RealNews
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static ILog _log = LogManager.GetLogger(typeof(Program));
        private static string _path;

        [STAThread]
        static void Main()
        {
            _path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (_path.EndsWith("\\") == false) _path += "\\";

            if (Singleinstance.Running(_path) == false)
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
        }     

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.AppendAllText("error.txt", e.ExceptionObject.ToString());
            _log.Error(e);
        }
    }
}
