using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace RealNews
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //static Mutex mutex = null;
        private static ILog _log = LogManager.GetLogger(typeof(Program));

        [STAThread]
        static void Main()
        {
            //var p = "RealNews";// Assembly.GetExecutingAssembly().Location;
            //mutex = new Mutex(true,p);
            //if (mutex.WaitOne(TimeSpan.Zero, true))
            if(File.Exists("feeds\\$temp")== false)
            {
                Directory.CreateDirectory("feeds");
                File.WriteAllText("feeds\\$temp", "running");
                _path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                if (_path.EndsWith("\\") == false) _path += "\\";
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
                File.Delete("feeds\\$temp");
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

        static string _path;
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (File.Exists(args.Name))
                return Assembly.LoadFrom(args.Name);
            string[] ss = args.Name.Split(',');
            string fname = ss[0] + ".dll";
            if (File.Exists(fname))
                return Assembly.LoadFrom(fname);
            else if (File.Exists(_path + "bin\\" + fname))
                return Assembly.LoadFrom(_path + "bin\\" + fname);

            else return null;
        }
    }
}
