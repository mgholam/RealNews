using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RealNews
{
    public class Feed
    {
        public int id;
        public string URL = "";
        public string Title = "";
        public bool RTL;
        public bool DownloadImages = true;
        public int UpdateEveryMin = 0; // use global settings else e.g. 8*60 min
        public DateTime LastUpdate;
        public DateTime LastItemDate;
        public string LastError;
        public int UnreadCount = 0;
        public int StarredCount = 0;
        public bool feediconfailed = false;
        public bool ExcludeInCleanup = false;
        public string Folder = "";
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
        public string FeedName;
        public string Attachment;

        public bool isRead = false;
        public bool isStarred = false;
        [XmlIgnore]
        public bool RTL = false;
    }

    class FeedItemComparer : IEqualityComparer<FeedItem>
    {
        public bool Equals(FeedItem p1, FeedItem p2)
        {
            return p1.Title == p2.Title;
        }

        public int GetHashCode(FeedItem p)
        {
            return p.Title.GetHashCode();
        }
    }
    class FeedItemSort : IComparer<FeedItem>
    {
        public int Compare(FeedItem x, FeedItem y)
        {
            return y.date.CompareTo(x.date);
        }
    }
}
