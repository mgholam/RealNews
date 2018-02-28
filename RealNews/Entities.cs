using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealNews
{
    public class Feed
    {
        public int id;
        public string URL = "";
        public string Title = "";
        public string IconName;
        public bool RTL;
        public DateTime LastUpdate;
        public int UpdateEveryHour;
        public bool DownloadImages = true;
        public DateTime LastItemDate;
    }


    public class FeedItem
    {
        public string Title;
        public string Link;
        public string Categories;
        public DateTime date;
        public string Author;
        public string Id;

        public string Description;
        public int feedid;
        public string Attachment;
    }



}
