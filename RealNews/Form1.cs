using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CodeHollow.FeedReader;
using RaptorDB;

namespace RealNews
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        RealNewsWeb web;
        bool loaded = false;
        private KeyStoreHF _imgcache;
        List<Feed> _feeds = new List<Feed>();
        ConcurrentDictionary<string, List<FeedItem>> _feeditems = new ConcurrentDictionary<string, List<FeedItem>>();
        bool _rendering = false;
        Feed _currentFeed = null;


        private void Form1_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory("feeds\\temp");
            Directory.CreateDirectory("feeds\\lists");
            Directory.CreateDirectory("configs");
            SetDoubleBuffering(listView1, true);
            SetHeight(listView1, 20);

            if (File.Exists("configs\\settings.config"))
                fastJSON.JSON.FillObject(new Settings(), File.ReadAllText("configs\\settings.config"));
            if (Settings.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;

            if (File.Exists("feeds\\feeds.list"))
            {
                _feeds = fastJSON.JSON.ToObject<List<Feed>>(File.ReadAllText("feeds\\feeds.list"));
                foreach (var f in _feeds)
                {
                    var tn = new TreeNode();
                    tn.Tag = f;
                    tn.Text = f.Title;
                    treeView1.Nodes.Add(tn);
                }
            }

            splitContainer1.SplitterDistance = Settings.treeviewwidth;
            splitContainer2.SplitterDistance = Settings.feeditemlistwidth;
            loaded = true;

            _imgcache = new RaptorDB.KeyStoreHF("cache", "images.dat");
            web = new RealNewsWeb(Settings.webport);
        }



        //-------------------------------------------------------------------------------------------------------------

        private void ReadOPML(string filename)
        {
            // fix : do if exists logic here
            if (File.Exists(filename))
            {
                var hd = new HtmlAgilityPack.HtmlDocument();
                hd.Load(filename, Encoding.UTF8);
                int id = 1;
                foreach (var f in hd.DocumentNode.SelectNodes("//outline"))
                {
                    try
                    {
                        var feed = new Feed()
                        {
                            Title = f.Attributes["text"].Value,
                            URL = f.Attributes["xmlUrl"].Value,
                            id = id++
                        };

                        _feeds.Add(feed);
                        var tn = new TreeNode();
                        tn.Tag = feed;
                        tn.Text = feed.Title;
                        treeView1.Nodes.Add(tn);
                    }
                    catch { }
                }
            }
        }

        private void ProceeeFeedURL(Feed feed)
        {
            var feedxml = "";
            if (feed != null && feed.URL != "")
            {
                toolMessage.Text = "Getting : " + feed.URL;
                Thread.Sleep(100);
                Application.DoEvents();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                if (Settings.UseSytemProxy)
                    wc.Proxy = WebRequest.DefaultWebProxy;
                feedxml = wc.DownloadString(feed.URL);
                File.WriteAllText(GetFeedXmlFilename(feed), feedxml, Encoding.UTF8);
            }
            else // testing
            {
                feedxml = File.ReadAllText(GetFeedXmlFilename("betanews"));
            }

            var reader = FeedReader.ReadFromString(feedxml);
            List<FeedItem> list = new List<FeedItem>();
            foreach (var item in reader.Items)
            {
                var i = new FeedItem
                {
                    Title = item.Title,
                    date = item.PublishingDate != null ? item.PublishingDate.Value : DateTime.Now,
                    feedid = feed.id,
                    Id = item.Id,
                    Link = item.Link,
                    Attachment = item.SpecificItem.Enclosure.Url,
                    Categories = item.Categories.Count > 5 ? string.Join(", ", item.Categories.ToArray(), 0, 4) : string.Join(", ", item.Categories),
                    Author = item.Author
                };


                var p = Html.Helpers.HtmlSanitizer.sanitize(item.Description);
                Regex r = new Regex("src\\s*=\\s*[\'\"]\\s*(?<href>.*?)\\s*[\'\"]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                List<string> imgs = new List<string>();
                foreach (var img in GetImagesInHTMLString(p))
                {
                    imgs.Add(r.Match(img).Groups["href"].Value);
                }

                foreach (var img in imgs)
                {
                    string n = "http://localhost:{port}/api/image?";
                    if (img.StartsWith("http://"))  // fix : upercase??
                        p = p.Replace(img, img.Replace("http://", n));
                    else
                        p = p.Replace(img, img.Replace("https://", n));
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(p);
                if (item.SpecificItem.ExtraData != null)
                {
                    sb.AppendLine("<table class='extradata'>");
                    foreach (var j in item.SpecificItem.ExtraData)
                    {
                        if (j.Key == "description" || j.Key == "title")
                            continue;
                        sb.Append("<tr><td>");
                        sb.Append(j.Key);
                        sb.Append("</td><td>");
                        sb.Append(j.Value);
                        sb.Append("</td></tr>");
                    }
                    sb.AppendLine("</table>");
                }

                i.Description = sb.ToString();
                list.Add(i);
            }
            string fn = GetFeedFilename("betanews"); // testing
            if (feed != null && feed.Title != "")
                fn = GetFeedFilename(feed);
            File.WriteAllText(fn, fastJSON.JSON.ToNiceJSON(list, new fastJSON.JSONParameters { UseExtensions = false, UseEscapedUnicode = false }), Encoding.UTF8);
            toolMessage.Text = "Downloading image x of y";
            // fix : download images


            // fix : check and add to existing list
            List<FeedItem> old = null;
            if (_feeditems.ContainsKey(feed.Title))
                _feeditems.TryRemove(feed.Title, out old);
            _feeditems.TryAdd(feed.Title, list);

            ShowFeedList(feed);
        }

        private void UpdateAll()
        {
            // fix : update all
            foreach (var f in _feeds)
            {
                ProceeeFeedURL(f);
            }
        }

        private void ShowItem(FeedItem item)
        {
            // fix : show item

            if (item == null)
            {
                item = new FeedItem
                {
                    Title = "Testing",
                    Link = "http://google.com",
                    Author = "m. gholam",
                    Categories = "testing, 123",
                    date = DateTime.Now
                };
            }

            var str = item.Description;
            //str = "\r\n<table>\r\n<tr><td>title</td><td>Mum S02E02 HDTV x264-RiVER</td></tr><tr><td>category</td><td>TV</td></tr><tr><td>link</td><td>https://eztv.ag/ep/617889/mum-s02e02-hdtv-x264-river/</td></tr><tr><td>guid</td><td>https://eztv.ag/ep/617889/mum-s02e02-hdtv-x264-river/</td></tr><tr><td>pubDate</td><td>Tue, 27 Feb 2018 18:00:02 -0500</td></tr><tr><td>contentLength</td><td>189773815</td></tr><tr><td>infoHash</td><td>932C6874CF5508513C26D1D698B60FB3B97A0538</td></tr><tr><td>magnetURI</td><td>magnet:?xt=urn:btih:932C6874CF5508513C26D1D698B60FB3B97A0538&dn=Mum.S02E02.HDTV.x264-RiVER%5Beztv%5D.mkv&tr=udp%3A%2F%2Ftracker.publicbt.com%2Fannounce&tr=udp%3A%2F%2Fopen.demonii.com%3A1337&tr=http%3A%2F%2Ftracker.trackerfix.com%3A80%2Fannounce&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Fexodus.desync.com%3A6969</td></tr><tr><td>seeds</td><td>0</td></tr><tr><td>peers</td><td>0</td></tr><tr><td>verified</td><td>0</td></tr><tr><td>fileName</td><td>Mum.S02E02.HDTV.x264-RiVER[eztv].mkv</td></tr><tr><td>enclosure</td><td></td></tr></table>\r\n";
            str = str.Replace("{port}", Settings.webport.ToString());

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html");
            if (_currentFeed.RTL)
                sb.Append("dir='rtl'>"); // fix : get if rtl
            else
                sb.Append(">");
            sb.Append("<link rel='stylesheet' href='http://localhost:"+Settings.webport+"/style.css'>");
            sb.Append("<div class='title'>");
            sb.Append("<h2><a href='" + item.Link + "'>" + item.Title + "</a></h2>");
            sb.Append("<label>");
            sb.Append("" + item.Author);
            sb.Append("</label>");
            sb.Append("<label>");
            sb.Append("" + item.Categories);
            sb.Append("</label>");
            sb.Append("<label>");
            sb.Append("" + item.date);
            sb.Append("</label>");
            sb.Append("</div>");
            sb.AppendLine(str);
            sb.AppendLine("</html>");

            //_rendering = true;
            //webBrowser1.Navigate("about:blank");
            //while (webBrowser1.Document == null || webBrowser1.Document.Body == null)
            //    Application.DoEvents();
            //webBrowser1.Document.OpenNew(true).Write(sb.ToString());
            ////webBrowser1.DocumentText = sb.ToString();
            //_rendering = false;
            DisplayHtml(sb.ToString());
        }

        private void MoveNextUnread()
        {
            // fix : move next
            ShowItem(null);
            MessageBox.Show("next");
        }

        private void Shutdown()
        {
            if (this.WindowState == FormWindowState.Normal)
                Settings.Maximized = false;
            else
                Settings.Maximized = true;
            var jp = new fastJSON.JSONParameters { UseExtensions = false };
            File.WriteAllText("configs\\settings.config", fastJSON.JSON.ToNiceJSON(new Settings(), jp));
            File.WriteAllText("feeds\\feeds.list", fastJSON.JSON.ToNiceJSON(_feeds, jp));
            foreach (var i in _feeditems)
            {
                File.WriteAllText(GetFeedFilename(i.Key), fastJSON.JSON.ToNiceJSON(i.Value, jp));
            }
            _imgcache.Shutdown();
        }

        private List<string> GetImagesInHTMLString(string htmlString)
        {
            List<string> images = new List<string>();
            string pattern = @"<(img)\b[^>]*>";

            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(htmlString);

            for (int i = 0, l = matches.Count; i < l; i++)
            {
                images.Add(matches[i].Value);
            }

            return images;
        }

        private void DisplayHtml(string html)
        {
            _rendering = true;
            webBrowser1.Navigate("about:blank");
            try
            {
                if (webBrowser1.Document != null)
                {
                    webBrowser1.Document.Write(string.Empty);
                }
            }
            catch (Exception e)
            { } // do nothing with this
            webBrowser1.DocumentText = html;
            _rendering = false;
        }

        private void ShowFeedList(Feed feed)
        {
            if (feed != null)
            {
                List<FeedItem> list = null;
                if (_feeditems.TryGetValue(feed.Title, out list) == false)
                {
                    if (File.Exists(GetFeedFilename(feed)))
                    {
                        list = fastJSON.JSON.ToObject<List<FeedItem>>(File.ReadAllText(GetFeedFilename(feed)));
                        _feeditems.TryAdd(feed.Title, list);
                    }
                }
                listView1.Items.Clear();
                listView1.View = View.Details;
                listView1.RightToLeft = feed.RTL? RightToLeft.Yes: RightToLeft.No;
                listView1.RightToLeftLayout = feed.RTL;

                if (list != null)
                {
                    listView1.SuspendLayout();
                    listView1.BeginUpdate();
                    foreach (var i in list)
                    {
                        var lvi = new ListViewItem();
                        lvi.Name = "Title";
                        lvi.Text = i.Title;
                        lvi.Tag = i;
                        //lvi.SubItems.Add(i.Title);
                        lvi.SubItems.Add(i.date.ToString());
                        listView1.Items.Add(lvi);
                        //listView1.Items.Add(i.Title);

                    }
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.EndUpdate();
                    listView1.ResumeLayout();
                }
            }
        }

        private string GetFeedFilename(Feed f)
        {
            return "feeds\\lists\\" + f.Title.Replace(":", "") + ".list";
        }
        private string GetFeedFilename(string title)
        {
            return "feeds\\lists\\" + title.Replace(":", "") + ".list";
        }
        private string GetFeedXmlFilename(Feed f)
        {
            return "feeds\\temp\\" + f.Title.Replace(":", "") + ".xml";
        }
        private string GetFeedXmlFilename(string title)
        {
            return "feeds\\temp\\" + title.Replace(":", "") + ".xml";
        }

        private static void SetDoubleBuffering(System.Windows.Forms.Control control, bool value)
        {
            System.Reflection.PropertyInfo controlProperty = typeof(System.Windows.Forms.Control)
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            controlProperty.SetValue(control, value, null);
        }

        private void SetHeight(ListView listView, int height)
        {
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, height);
            listView.SmallImageList = imgList;
        }
        // ---------------------------------------------------------------------------------------------------------
        // UI handlers
        // ---------------------------------------------------------------------------------------------------------

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (_rendering == false && e.Url.ToString() != "about:blank")
            {
                e.Cancel = true;
                Process.Start(e.Url.ToString());
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Shutdown();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // fix : after feed select -> show feed item list 
            var feed = e.Node.Tag as Feed;
            _currentFeed = feed;
            ShowFeedList(feed);
        }



        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (loaded)
                Settings.treeviewwidth = e.SplitX;
        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (loaded)
                Settings.feeditemlistwidth = e.SplitX;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveNextUnread();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                MoveNextUnread();
        }

        private void updateAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProceeeFeedURL(new Feed { id = 1 });
        }

        private void markAsReadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // fix : 
        }

        private void updateNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = treeView1.SelectedNode.Tag as Feed;
            ProceeeFeedURL(f);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            ShowItem(listView1.FocusedItem.Tag as FeedItem);
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            ShowItem(listView1.FocusedItem.Tag as FeedItem);
        }
    }
}
