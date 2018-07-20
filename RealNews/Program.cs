using System;
using System.Diagnostics;
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
            var name = Process.GetCurrentProcess().MainModule.ModuleName.Split('.')[0];
            if (_path.EndsWith("\\") == false) _path += "\\";
            var pp = Process.GetProcessesByName(name);
            var found = 0;
            foreach (var pi in pp)
            {
                if (pi.MainModule.FileName.StartsWith(_path))
                    found++;
            }

            if (found == 1)
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
            else
            {
                MessageBox.Show("Only one instance at a time");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.Error(e);
        }
    }
}
