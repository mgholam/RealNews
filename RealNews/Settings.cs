using System;

namespace RealNews
{
    public class Settings
    {
        public static int webport = 11111;

        public static int treeviewwidth = 200;

        public static int feeditemlistwidth = 400;

        public static bool Maximized = true;

        public static bool UseSytemProxy = true;

        public static int GlobalUpdateEveryMin = 4*60;

        public static int CleanupItemAfterDays = 14;

        public static int DownloadImagesUnderKB = 200;

        public static DateTime LastUpdateTime = DateTime.Now;

        public static string StartDownloadImgTime = "03:00";
        public static string EndDownloadImgTime = "06:00";

        public static int FeedUpdateTimeout = 5 * 60 * 1000; 

        //public static int DownloadImgThreads = 20;

    }
}
