using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using static RaptorDB.Common.ZipStorer;

namespace RealNews
{
    class ImageCache
    {
        public ImageCache()
        {
        }

        ConcurrentDictionary<string, List<ZipFileEntry>> _ziplookup = new ConcurrentDictionary<string, List<ZipFileEntry>>();

        public class urlhash
        {
            public string folder;
            public string fn;
        }

        private string _path = "cache/";

        private urlhash FixName(string url)
        {
            string fol = url.Substring(0, url.IndexOf('/'));
            url = url.Replace("amp;", ""); // KLUDGE : for engaget images
            //url = url.Replace("&amp;", "&");

            var r = new urlhash
            {
                folder = fol,
                fn = fol + "/" + url.GetHashCode() + ".jpg"
            };

            return r;
        }

        public string GetFilename(string url)
        {
            if (url == null)
                return _path + "null.jpg";

            var key = FixName(url);

            Directory.CreateDirectory(_path + key.folder);

            return _path + key.fn;
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

            if (File.Exists(_path + key.fn))
                return true;
            else
            {
                // check zip 
                if (File.Exists(_path + key.folder + ".zip"))
                {
                    if (_ziplookup.ContainsKey(key.folder) == false)
                    {
                        using (var zf = RaptorDB.Common.ZipStorer.Open(_path + key.folder + ".zip", FileAccess.Read))
                        {
                            _ziplookup.TryAdd(key.folder, zf.ReadCentralDir());
                        }
                    }
                    _ziplookup.TryGetValue(key.folder, out List<ZipFileEntry> l);
                    if (l != null)
                    {
                        var ze = l.Find(x => key.fn.EndsWith(x.FilenameInZip));
                        if (ze.FilenameInZip != null)
                            return true;
                    }
                }

                return false;
            }
        }

        public byte[] Get(string url)
        {
            if (url == null)
                return null;

            var key = FixName(url);

            if (File.Exists(_path + key.fn))
                return File.ReadAllBytes(_path + key.fn);
            else
            {
                if (File.Exists(_path + key.folder + ".zip"))
                {
                    try
                    {
                        using (var zf = RaptorDB.Common.ZipStorer.Open(_path + key.folder + ".zip", FileAccess.Read))
                        {
                            var ze = zf.ReadCentralDir().Find(x => key.fn.EndsWith(x.FilenameInZip));
                            if (ze.FilenameInZip != null)
                            {
                                MemoryStream ms = new MemoryStream();
                                zf.ExtractFile(ze, ms);
                                return ms.ToArray();
                            }
                        }
                    }
                    catch { }
                }
                return null;
            }
        }

        public void ClearLookup()
        {
            _ziplookup = new ConcurrentDictionary<string, List<ZipFileEntry>>();
        }
    }
}
