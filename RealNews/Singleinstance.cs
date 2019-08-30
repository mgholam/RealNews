using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RealNews
{
    class Singleinstance
    {
        public static int _msgID;

        public static bool Running(string path)
        {
            var s = "RealNews|" + path;
            _msgID = RegisterWindowMessage(s);

            var name = Process.GetCurrentProcess().MainModule.ModuleName.Split('.')[0];
            var pp = Process.GetProcessesByName(name);
            var found = 0;
            foreach (var pi in pp)
            {
                if (pi.MainModule.FileName.StartsWith(path))
                {
                    found++;
                    BringProcessToFront(pi);
                }
            }

            return found > 1 ? true : false;
        }

        public static void BringProcessToFront(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            PostMessage((IntPtr)0xffff, _msgID, IntPtr.Zero, IntPtr.Zero);
            SetForegroundWindow(handle);
        }


        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
    }
}
