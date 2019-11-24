using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RealNews.Helper
{
    class listviewapi
    {
        static int LVM_FIRST = 4096;
        static int LVM_SETGROUPMETRICS = (LVM_FIRST + 155);
        //int LVGMF_NONE = 0;
        //int LVGMF_BORDERSIZE = 1;
        //int LVGMF_BORDERCOLOR = 2;
        static uint LVGMF_TEXTCOLOR = 0x4;


        [StructLayout(LayoutKind.Sequential)]
        public struct LVGROUPMETRICS
        {
            public uint cbSize;
            public uint mask;
            public uint Left;
            public uint Top;
            public uint Right;
            public uint Bottom;
            public int crLeft;
            public int crTop;
            public int crRight;
            public int crBottom;
            public int crHeader;
            public int crFooter;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref LVGROUPMETRICS lParam);

        public static void SetGroupHeaderColor(ListView lv, Color icolor)
        {
            // NOT WORKING

            //var gm = new LVGROUPMETRICS();
            //int wparam = 0;
            //IntPtr lparam;

            //gm.mask = LVGMF_TEXTCOLOR;
            //gm.cbSize = (uint)Marshal.SizeOf(typeof(LVGROUPMETRICS));
            //gm.crHeader = icolor.ToArgb();

            //lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(LVGROUPMETRICS)));
            //Marshal.StructureToPtr(gm, lparam, false);

            //SendMessage(lv.Handle, LVM_SETGROUPMETRICS, wparam, ref gm);

            //Marshal.FreeCoTaskMem(lparam);
        }
    }
}

