using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RealNews
{
    class ImageCache
    {
        public ImageCache(string file)
        {
        }

        public class urlhash
        {
            public string folder;
            public string fn;
        }


        private urlhash FixName(string url)
        {
            string fol = url.Substring(0, url.IndexOf('/'));

            var r = new urlhash
            {
                folder = fol,
                fn = fol + "/" + url.GetHashCode() + ".jpg"
            };

            return r;
        }


        public void Add(string url, byte[] data)
        {
            if (url == null)
                return;

            var key = FixName(url);

            Directory.CreateDirectory("cache/" + key.folder);
            File.WriteAllBytes("cache/" + key.fn, data);
            return;
        }

        public bool Contains(string url)
        {
            if (url == null)
                return true;

            var key = FixName(url);

            return File.Exists( "cache/" + key.fn);
        }

        public byte[] Get(string url)
        {
            if (url == null)
                return null;

            var key = FixName(url);
            if (File.Exists("cache/" + key.fn))
                return File.ReadAllBytes("cache/" + key.fn);
            else
                return null;
        }

        public void Shutdown()
        {

        }
    }
}
