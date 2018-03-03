﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CodeHollow.FeedReader;
using fastJSON;
using RaptorDB;
using System.Threading.Tasks;
using Westwind.Web.Utilities;

namespace RealNews
{
    public partial class frmMain : Form
    {
        // fix : show unread list
        // fix : show starred list
        public frmMain()
        {
            InitializeComponent();
            //treeView1.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            //treeView1.DrawNode += (o, e) =>
            //{
            //    if (!e.Node.TreeView.Focused && e.Node == e.Node.TreeView.SelectedNode)
            //    {
            //        Font treeFont = e.Node.NodeFont ?? e.Node.TreeView.Font;
            //        e.Graphics.FillRectangle(Brushes.Blue, e.Bounds);
            //        //ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, SystemColors.HighlightText, SystemColors.Highlight);
            //        TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds, Color.Black, TextFormatFlags.GlyphOverhangPadding);
            //    }
            //    else
            //        e.DrawDefault = true;
            //};
        }

        RealNewsWeb web;
        bool loaded = false;
        private KeyStoreHF _imgcache;
        List<Feed> _feeds = new List<Feed>();
        ConcurrentDictionary<string, List<FeedItem>> _feeditems = new ConcurrentDictionary<string, List<FeedItem>>();
        bool _rendering = false;
        Feed _currentFeed = null;
        List<FeedItem> _currentList = null;
        List<string> _downloadimglist = new List<string>();
        private BizFX.UI.Skin.SkinningManager _skin = new BizFX.UI.Skin.SkinningManager();
        private Regex _imghrefregex = new Regex("src\\s*=\\s*[\'\"]\\s*(?<href>.*?)\\s*[\'\"]", RegexOptions.IgnoreCase | RegexOptions.Compiled);


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Directory.CreateDirectory("feeds\\temp");
            Directory.CreateDirectory("feeds\\lists");
            Directory.CreateDirectory("feeds\\icons");
            Directory.CreateDirectory("configs");

            if (File.Exists("configs\\style.css") == false)
                File.WriteAllText("configs\\style.css", Properties.Resources.style);

            SetDoubleBuffering(listView1, true);
            SetHeight(listView1, 20);

            if (File.Exists("configs\\settings.config"))
                JSON.FillObject(new Settings(), File.ReadAllText("configs\\settings.config"));
            if (Settings.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;

            rssImages.Images.Add(Properties.Resources.rss);
            rssImages.Images.Add(Properties.Resources.news_new);
            rssImages.Images.Add(Properties.Resources.star_yellow);
            var tt = treeView1.Nodes.Find("Unread", true);
            if (tt.Length > 0)
            {
                tt[0].ImageIndex = 1;
                tt[0].SelectedImageIndex = 1;
            }

            tt = treeView1.Nodes.Find("Starred", true);
            if (tt.Length > 0)
            {
                tt[0].ImageIndex = 2;
                tt[0].SelectedImageIndex = 2;
            }

            LoadFeeds();

            splitContainer1.SplitterDistance = Settings.treeviewwidth;
            splitContainer2.SplitterDistance = Settings.feeditemlistwidth;

            loaded = true;

            _imgcache = new KeyStoreHF("cache", "images.dat");
            web = new RealNewsWeb(Settings.webport);

            SkinForm();
            Log(" ");
        }

        private void LoadFeeds()
        {
            splitContainer1.Visible = false;
            if (File.Exists("feeds\\downloadimg.list"))
                _downloadimglist = JSON.ToObject<List<string>>(File.ReadAllText("feeds\\downloadimg.list"));

            if (File.Exists("feeds\\feeds.list"))
            {
                _feeds = JSON.ToObject<List<Feed>>(File.ReadAllText("feeds\\feeds.list"));
                treeView1.BeginUpdate();
                foreach (var f in _feeds)
                {
                    var fn = "feeds\\icons\\" + GetFeedFilenameOnly(f) + ".ico";
                    int imgidx = 0;
                    if (File.Exists(fn))
                    {
                        try
                        {
                            rssImages.Images.Add(Image.FromFile(fn));
                            imgidx = rssImages.Images.Count - 1;
                        }
                        catch { }
                    }
                    var tn = new TreeNode();
                    tn.Tag = f;
                    tn.Name = f.Title;
                    if (imgidx > 0)
                    {
                        tn.ImageIndex = imgidx;
                        tn.SelectedImageIndex = imgidx;
                    }
                    if (f.UnreadCount > 0)
                    {
                        tn.NodeFont = new Font(treeView1.Font, FontStyle.Bold);
                        tn.Text = f.Title + $" ({f.UnreadCount})";
                    }
                    else
                        tn.Text = f.Title;
                    treeView1.Nodes.Add(tn);
                    _currentFeed = f;
                    ShowFeedList(f);
                }
                treeView1.EndUpdate();
                Task.Factory.StartNew(downloadfeedicons);
            }
            splitContainer1.Visible = true;
        }

        private void downloadfeedicons()
        {
            mWebClient wc = new mWebClient();
            wc.Encoding = Encoding.UTF8;

            if (Settings.UseSytemProxy)
                wc.Proxy = WebRequest.DefaultWebProxy;
            foreach (var f in _feeds)
            {
                var fn = "feeds\\icons\\" + GetFeedFilenameOnly(f) + ".ico";
                if (File.Exists(fn) || f.feediconfailed)
                    continue;

                var u = new Uri(f.URL);
                var ss = u.Host.Split('.');
                if (ss[0] != "www" && ss.Length > 2)
                    ss[0] = "www";
                var s = u.Scheme + "://" + string.Join(".", ss) + "/favicon.ico";

                try
                {
                    wc.DownloadFile(s, fn);
                }
                catch
                {
                    if (File.Exists(fn))
                        File.Delete(fn);
                    f.feediconfailed = true;
                }
            }
        }

        private void SkinForm()
        {
            //_skin.ParentForm = this; // FIX : skinner mangles the form when window less set 
            //_skin.DefaultSkin = BizFX.UI.Skin.DefaultSkin.Office2007Obsidian;
            var r = new BSE.Windows.Forms.Office2007Renderer(new BSE.Windows.Forms.Office2007BlackColorTable());
            menuStrip1.Renderer = r;
            statusStrip1.Renderer = r;
            contextMenuStrip1.Renderer = r;
            contextMenuStrip2.Renderer = r;
            this.Invalidate();
        }

        //-------------------------------------------------------------------------------------------------------------

        private void ReadOPML(string filename)
        {
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

                        var r = _feeds.Find(x => x.URL == feed.URL);
                        if (r == null)
                        {
                            _feeds.Add(feed);
                            var tn = new TreeNode();
                            tn.Tag = feed;
                            tn.Text = feed.Title;
                            treeView1.Nodes.Add(tn);
                        }
                    }
                    catch { }
                }
            }
        }


        private void UpdateFeed(Feed feed, Action<string> log) // fix : force download images
        {
            var feedxml = "";
            if (feed != null && feed.URL != "")
            {
                try
                {
                    feed.LastError = "";
                    log("Getting : " + feed.URL);
                    Thread.Sleep(100);
                    Application.DoEvents();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    mWebClient wc = new mWebClient();
                    wc.Encoding = Encoding.UTF8;
                    if (Settings.UseSytemProxy)
                        wc.Proxy = WebRequest.DefaultWebProxy;
                    feedxml = wc.DownloadString(feed.URL);
                    feed.LastUpdate = DateTime.Now;
                    File.WriteAllText(GetFeedXmlFilename(feed), feedxml, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    feed.LastError = ex.Message;
                    log("ERROR : " + feed.Title);
                    return;
                }
            }

            log("Processing : " + feed.Title);
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
                    Attachment = item.SpecificItem.Enclosure != null ? item.SpecificItem.Enclosure.Url : "",
                    Categories = item.Categories.Count > 5 ? string.Join(", ", item.Categories.ToArray(), 0, 4) : string.Join(", ", item.Categories),
                    Author = item.Author
                };

                StringBuilder sb = new StringBuilder(item.Description);
                if (i.Attachment != "")
                    sb.AppendLine("<br/> <a href='" + i.Attachment + "'>" + i.Attachment + "</a>");

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

                        if (j.Value.StartsWith("magnet") || j.Value.StartsWith("http"))
                            sb.Append("<a href='" + j.Value + "' rel='noreferrer'>" + j.Value + "</a>");

                        else if (j.Key.ToLower().Contains("length") || j.Key.ToLower() == "size")
                        {
                            long val = 0;
                            if (long.TryParse(j.Value, out val))
                                sb.Append(val.ToString("#,#"));
                            else
                                sb.Append(j.Value);
                        }
                        else
                            sb.Append(j.Value);
                        sb.Append("</td></tr>");
                    }
                    sb.AppendLine("</table>");
                }
                var tempdesc = HtmlSanitizer.SanitizeHtml(sb.ToString());

                List<string> imgs = new List<string>();
                foreach (var img in GetImagesInHTMLString(tempdesc))
                {
                    imgs.Add(_imghrefregex.Match(img).Groups["href"].Value);
                }

                foreach (var img in imgs)
                {
                    string n = "http://localhost:{port}/api/image?";
                    if (img.StartsWith("http://"))  // fix : upercase??
                        tempdesc = tempdesc.Replace(img, img.Replace("http://", n));
                    else
                        tempdesc = tempdesc.Replace(img, img.Replace("https://", n));
                }

                if (feed.DownloadImages)
                    _downloadimglist.AddRange(imgs);


                i.Description = tempdesc;
                list.Add(i);
            }
            log("Downloading image x of y");
            // fix : download images


            // check and add to existing list
            List<FeedItem> old = null;
            if (_feeditems.ContainsKey(feed.Title))
                _feeditems.TryRemove(feed.Title, out old);

            if (old != null)
            {
                var o = old.Union(list, new FeedItemComparer()).ToList();
                o.Sort(new FeedItemSort());
                list = o;
            }

            log("Saving feed : " + feed.Title);
            string fn = GetFeedFilename(feed);
            File.WriteAllText(fn, JSON.ToNiceJSON(list, new JSONParameters { UseExtensions = false, UseEscapedUnicode = false }), Encoding.UTF8);

            _feeditems.TryAdd(feed.Title, list);
        }

        private object _lock = new object();
        private void UpdateAll()
        {
            // update all
            lock (_lock)
            {
                List<Task> tasks = new List<Task>();
                int c = _feeds.Count;
                int i = 1;
                toolProgressBar.Maximum = c + 1;
                toolProgressBar.Value = i;
                foreach (var f in _feeds)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        UpdateFeed(f, Log);
                        this.Invoke((MethodInvoker)delegate
                        {
                            UpdateFeedCount(f);
                            toolCount.Text = $"{i++} of {c}";
                            toolProgressBar.Value = i;
                        });
                    }));
                }
                Task.Factory.StartNew(() =>
                {
                    Task.WaitAll(tasks.ToArray());
                    this.Invoke((MethodInvoker)delegate
                    {
                        toolProgressBar.Value = 0;
                        toolCount.Text = "";
                        Log("Update Done.");
                    });
                });
            }
        }

        private void ShowItem(FeedItem item)
        {
            ShowItem(item, true);
        }

        private void ShowItem(FeedItem item, bool isread)
        {
            // show item
            if (item == null)
            {
                item = new FeedItem
                {
                    Title = "Testing",
                    Link = "http://google.com",
                    Author = "m. gholam",
                    Categories = "testing, 123",
                    date = DateTime.Now,
                    Description = "",
                    Attachment = ""
                };
            }

            item.isRead = isread;

            var str = item.Description;
            str = str.Replace("{port}", Settings.webport.ToString());

            StringBuilder sb = new StringBuilder();
            sb.Append("<html");
            if (_currentFeed.RTL)
                sb.Append(" dir='rtl'>"); // get if rtl
            else
                sb.AppendLine(">");
            sb.Append("<link rel='stylesheet' href='http://localhost:" + Settings.webport + "/style.css'>");
            sb.Append("<div class='title'>");
            sb.Append("<h2><a href='" + item.Link + "'>" + item.Title + "</a></h2>");
            if (item.isStarred)
                sb.Append("<label>STAR</label>"); //fix : better starred ui
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

            DisplayHtml(sb.ToString());

            UpdateFeedCount();
        }

        private void UpdateStarCount()
        {
            if (_currentList == null || _currentFeed == null)
                return;
            List<FeedItem> list = _currentList;
            treeView1.BeginUpdate();
            _currentFeed.StarredCount = list.Count(x => x.isStarred == true);

            var ur = treeView1.Nodes.Find("Starred", true);
            if (ur.Length > 0)
            {
                long c = 0;
                foreach (var f in _feeds)
                    c += f.StarredCount;
                if (c > 0)
                {
                    ur[0].Text = $"Starred ({c})";
                }
                else
                {
                    ur[0].Text = "Starred";
                }
            }
            treeView1.EndUpdate();
        }

        private void UpdateFeedCount(Feed feed)
        {
            List<FeedItem> list = null;
            if (_feeditems.TryGetValue(feed.Title, out list))
            {
                UpdateFeedCount(feed, list);
            }
        }

        private void UpdateFeedCount()
        {
            UpdateFeedCount(_currentFeed, _currentList);
        }

        private void UpdateFeedCount(Feed feed, List<FeedItem> list)
        {
            try
            {
                if (feed == null || list == null)
                    return;
                treeView1.BeginUpdate();
                if (list != null)
                {
                    feed.UnreadCount = list.Count(x => x.isRead == false);
                    foreach (TreeNode n in treeView1.Nodes)
                    {
                        if (n.Text.StartsWith(feed.Title))
                        {
                            if (feed.UnreadCount > 0)
                            {
                                n.Text = feed.Title + $" ({feed.UnreadCount})";
                                n.NodeFont = new Font(treeView1.Font, FontStyle.Bold);
                            }
                            else
                            {
                                n.Text = feed.Title;
                                n.NodeFont = new Font(treeView1.Font, FontStyle.Regular);
                            }
                        }
                    }
                }

                var ur = treeView1.Nodes.Find("Unread", true);
                if (ur.Length > 0)
                {
                    long c = 0;
                    foreach (var f in _feeds)
                        c += f.UnreadCount;
                    if (c > 0)
                    {
                        ur[0].Text = $"Unread ({c})";
                        ur[0].NodeFont = new Font(treeView1.Font, FontStyle.Bold);
                    }
                    else
                    {
                        ur[0].Text = "Unread";
                        ur[0].NodeFont = new Font(treeView1.Font, FontStyle.Regular);
                    }
                }
                treeView1.EndUpdate();
            }
            catch //(Exception ex)
            {

            }
        }

        private void MoveNextUnread()
        {
            if (_currentFeed == null)
                _currentFeed = _feeds[0];

            // move next
            if (_currentFeed.UnreadCount == 0)
            {
                // find next feed
                int i = 0;// _feeds.FindIndex(x => x.Title == _currentFeed.Title);
                //i++;
                while (i < _feeds.Count)
                {
                    _currentFeed = _feeds[i++];
                    if (_currentFeed.UnreadCount > 0)
                    {
                        ShowFeedList(_currentFeed);
                        // focus feed in treeview
                        var n = treeView1.Nodes.Find(_currentFeed.Title, true);
                        if (n.Length > 0)
                            treeView1.SelectedNode = n[0];
                        break;
                    }
                }
            }
            ShowNextItem();
        }

        private void ShowNextItem()
        {
            var item = _currentList.Find(x => x.isRead == false);
            if (item == null)
                return;
            ShowItem(item);
            var l = listView1.FindItemWithText(item.Title);
            if (l.Index + 5 < listView1.Items.Count)
                listView1.EnsureVisible(l.Index + 5);
            if (l != null)
            {
                l.Font = new Font(listView1.Font, FontStyle.Regular);
                listView1.SelectedItems.Clear();

                //if (listView1.FocusedItem != null)
                //    listView1.FocusedItem.Focused = false;
                listView1.FocusedItem = l;
            }
        }

        private void Shutdown()
        {
            if (this.WindowState == FormWindowState.Normal)
                Settings.Maximized = false;
            else
                Settings.Maximized = true;
            var jp = new JSONParameters { UseExtensions = false };
            File.WriteAllText("configs\\settings.config", JSON.ToNiceJSON(new Settings(), jp));
            File.WriteAllText("feeds\\feeds.list", JSON.ToNiceJSON(_feeds, jp));
            File.WriteAllText("feeds\\downloadimg.list", JSON.ToJSON(_downloadimglist, jp));
            foreach (var i in _feeditems)
            {
                File.WriteAllText(GetFeedFilename(i.Key), JSON.ToNiceJSON(i.Value, jp));
                //File.WriteAllBytes(GetFeedFilename(i.Key) + ".bin", fastBinaryJSON.BJSON.ToBJSON(i.Value, new fastBinaryJSON.BJSONParameters { UseExtensions = false, UseUnicodeStrings = false }));
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
            catch //(Exception e)
            { } // do nothing with this
            webBrowser1.DocumentText = html;
            _rendering = false;
        }

        private void ShowFeedList(Feed feed)
        {
            try
            {
                if (feed != null)
                {
                    List<FeedItem> list = null;
                    if (_feeditems.TryGetValue(feed.Title, out list) == false)
                    {
                        if (File.Exists(GetFeedFilename(feed)))
                        {
                            list = JSON.ToObject<List<FeedItem>>(File.ReadAllText(GetFeedFilename(feed)));
                            _feeditems.TryAdd(feed.Title, list);
                        }
                    }
                    listView1.Items.Clear();
                    listView1.View = View.Details;
                    listView1.RightToLeft = feed.RTL ? RightToLeft.Yes : RightToLeft.No;
                    listView1.RightToLeftLayout = feed.RTL;

                    if (list != null)
                    {
                        _currentList = list;
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
                            if (i.isRead == false)
                                lvi.Font = new Font(lvi.Font, FontStyle.Bold);
                        }
                        listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        listView1.EndUpdate();
                        listView1.ResumeLayout();
                        UpdateFeedCount(feed, list);
                    }
                }
            }
            catch //(Exception ex)
            {

            }
        }

        private string GetFeedFilenameOnly(Feed f)
        {
            return f.Title.Replace(':', ' ').Replace('/', ' ').Replace('\\', ' ');
        }
        private string GetFeedFilename(Feed f)
        {
            return "feeds\\lists\\" + f.Title.Replace(':', ' ').Replace('/', ' ').Replace('\\', ' ') + ".list";
        }
        private string GetFeedFilename(string title)
        {
            return "feeds\\lists\\" + title.Replace(':', ' ').Replace('/', ' ').Replace('\\', ' ') + ".list";
        }
        private string GetFeedXmlFilename(Feed f)
        {
            return "feeds\\temp\\" + f.Title.Replace(':', ' ').Replace('/', ' ').Replace('\\', ' ') + ".xml";
        }
        private string GetFeedXmlFilename(string title)
        {
            return "feeds\\temp\\" + title.Replace(':', ' ').Replace('/', ' ').Replace('\\', ' ') + ".xml";
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

        private void Log(string msg)
        {
            toolMessage.Text = msg;
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
            // after feed select -> show feed item list 
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

        private void markAsReadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentList.ForEach(x => x.isRead = true);
            UpdateFeedCount();
            ShowFeedList(_currentFeed);
        }

        private void updateNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = treeView1.SelectedNode.Tag as Feed;
            Task.Factory.StartNew(() =>
            {
                UpdateFeed(f, Log);
                this.Invoke((MethodInvoker)delegate { ShowFeedList(f); });
            });
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
            listView1.FocusedItem.Font = new Font(listView1.FocusedItem.Font, FontStyle.Regular);
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            ShowItem(listView1.FocusedItem.Tag as FeedItem);
            listView1.FocusedItem.Font = new Font(listView1.FocusedItem.Font, FontStyle.Regular);
        }

        private void starToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleStarred();
        }

        private void ToggleStarred()
        {
            // toggle star
            var f = listView1.FocusedItem.Tag as FeedItem;
            if (f != null)
            {
                f.isStarred = !f.isStarred;
                UpdateStarCount();
                ShowItem(f);
            }
        }

        private void addNewFeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // add new feed
            frmFeed form = new frmFeed();
            form.feed = new Feed();
            if (form.ShowDialog() == DialogResult.OK)
            {
                // check if already exists
                var f = form.ret;
                var r = _feeds.Find(x => x.Title == f.Title);
                if (r != null)
                {
                    MessageBox.Show(f.Title + " Already exists", "Error");
                    return;
                }
                _feeds.Add(f);
                var tn = new TreeNode();
                tn.Tag = f;
                tn.Name = f.Title;
                tn.Text = f.Title;
                treeView1.Nodes.Add(tn);
            }
        }

        private void importOPMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // import opml file
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "OPML files (*.opml)|*.opml|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ReadOPML(openFileDialog1.FileName);
            }
        }

        private void editFeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // edit feed
            frmFeed form = new frmFeed();
            form.feed = treeView1.SelectedNode.Tag as Feed;
            if (form.ShowDialog() == DialogResult.OK)
            {
                var f = form.ret;
                if (form.feed.Title != f.Title)
                {
                    // fix : rename files
                }
                form.feed.URL = f.URL;
                form.feed.RTL = f.RTL;
                form.feed.DownloadImages = f.DownloadImages;
            }
        }

        private void toggleStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleStarred();
        }

        private void markUnreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // mark unread
            var f = listView1.FocusedItem.Tag as FeedItem;
            if (f != null)
            {
                f.isRead = !f.isRead;
                UpdateFeedCount();
                ShowItem(f, false);
                listView1.FocusedItem.Font = new Font(listView1.FocusedItem.Font, FontStyle.Bold);
            }
        }

        private void downloadImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // fix : download image for item now
        }
    }
}
