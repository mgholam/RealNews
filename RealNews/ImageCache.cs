using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RealNews
{
    class ImageCache
    {
        public ImageCache()//(string file)
        {
        }

        public class urlhash
        {
            public string folder;
            public string fn;
        }

        private string _path = "cache/";


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

            Directory.CreateDirectory(_path + key.folder);
            File.WriteAllBytes(_path + key.fn, data);
            return;
        }

        public bool Contains(string url)
        {
            if (url == null)
                return true;

            var key = FixName(url);

            return File.Exists(_path + key.fn);
        }

        public byte[] Get(string url)
        {
            if (url == null)
                return null;

            var key = FixName(url);
            if (File.Exists(_path + key.fn))
                return File.ReadAllBytes(_path + key.fn);
            else
                return null;
        }

        //public void Shutdown()
        //{

        //}
    }
}
