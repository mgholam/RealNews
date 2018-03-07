using System;
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
using System.Threading.Tasks;
using Westwind.Web.Utilities;

namespace RealNews
{
    public partial class frmMain : Form
    {

        // fix : designer problem with mytextbox
        // fix : download image thread
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
        List<Feed> _feeds = new List<Feed>();
        ConcurrentDictionary<string, List<FeedItem>> _feeditems = new ConcurrentDictionary<string, List<FeedItem>>();
        Feed _currentFeed = null;
        List<FeedItem> _currentList = null;
        ConcurrentQueue<string> _downloadimglist = new ConcurrentQueue<string>();
        private Regex _imghrefregex = new Regex("src\\s*=\\s*[\'\"]\\s*(?<href>.*?)\\s*[\'\"]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private FormWindowState _lastFormState = FormWindowState.Normal;
        private JSONParameters jp = new JSONParameters { UseExtensions = false };
        private string _localhostimageurl = "http://localhost:{port}/api/image?";
        private ImageCache _imageCache;
        private static ILog _log = LogManager.GetLogger(typeof(frmMain));
        private DateTime _lastUpdate = DateTime.Now;
        private System.Timers.Timer _minuteTimer;
        private bool _newItemsExist = false;
        private bool _DoDownloadImages = false;


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon1.Icon = Properties.Resources.tray;
            notifyIcon1.Visible = true;
            _minuteTimer = new System.Timers.Timer(1000);
            _minuteTimer.Elapsed += _minuteTimer_Elapsed;
            _minuteTimer.AutoReset = true;
            _minuteTimer.Enabled = true;

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

            LoadFeeds();
            Task.Factory.StartNew(downloadfeedicons);

            splitContainer1.SplitterDistance = Settings.treeviewwidth;
            splitContainer2.SplitterDistance = Settings.feeditemlistwidth;

            loaded = true;

            _imageCache = new ImageCache();

            web = new RealNewsWeb(Settings.webport, _imageCache, ShowFeedItemHtml);

            SkinForm();
            _lastFormState = this.WindowState;
            Log(" ");
            webBrowser1.Navigate("http://localhost:" + Settings.webport + "/api/show");
        }

        private object _minlock = new object();
        private int _minCount = 0;
        private void _minuteTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _minCount++;
            // tray icon yellow
            if (_newItemsExist)
                notifyIcon1.Icon = Properties.Resources.trayhighlight;
            else
                notifyIcon1.Icon = Properties.Resources.tray;

            if (_minCount < 60)
                return;
            _minCount = 0;
            lock (_minlock)
            {
                var now = DateTime.Now;

                if (now.Subtract(_lastUpdate).TotalMinutes > Settings.GlobalUpdateEveryMin)
                {
                    UpdateAll();
                    _lastUpdate = now;
                }

                foreach (var f in _feeds)
                {
                    if (f.UpdateEveryMin > 0)
                    {
                        if (now.Subtract(f.LastUpdate).TotalMinutes > f.UpdateEveryMin)
                            Task.Factory.StartNew(() => UpdateFeed(f, Log));
                    }
                }

                // download images
                var start = TimeSpan.Parse(Settings.StartDownloadImgTime);
                var end = TimeSpan.Parse(Settings.EndDownloadImgTime);
                TimeSpan timeBetween = now.TimeOfDay;

                _DoDownloadImages = false;
                if (timeBetween >= start && timeBetween < end)
                    _DoDownloadImages = true;
            }
        }

        private string _currhtml = "<html></html>";
        private string ShowFeedItemHtml()
        {
            return _currhtml;
        }

        private void LoadFeeds()
        {
            splitContainer1.Visible = false;
            if (File.Exists("feeds\\downloadimg.list"))
            {
                var l = JSON.ToObject<List<string>>(File.ReadAllText("feeds\\downloadimg.list"));
                _downloadimglist = new ConcurrentQueue<string>(l);
            }
            if (File.Exists("feeds\\feeds.list"))
            {
                _feeds = JSON.ToObject<List<Feed>>(File.ReadAllText("feeds\\feeds.list"));
                treeView1.BeginUpdate();
                treeView1.Nodes.Clear();
                var tn = treeView1.Nodes.Add("Unread");
                tn.ImageIndex = 1;
                tn.Name = "Unread";
                tn.SelectedImageIndex = 1;
                tn = treeView1.Nodes.Add("Starred");
                tn.Name = "Starred";
                tn.ImageIndex = 2;
                tn.SelectedImageIndex = 2;

                foreach (var f in _feeds)
                {
                    var fn = "feeds\\icons\\" + GetFeedFilenameOnly(f) + ".ico";
                    if (File.Exists(GetFeedFilename(f)))
                    {
                        var list = JSON.ToObject<List<FeedItem>>(File.ReadAllText(GetFeedFilename(f)));
                        _feeditems.TryAdd(f.Title, list);
                    }
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
                    tn = new TreeNode();
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
                UpdateStarCount();
            }
            splitContainer1.Visible = true;
        }

        private void downloadfeedicons()
        {
            mWebClient wc = new mWebClient();
            bool update = false;
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
                    update = true;
                }
                catch
                {
                    if (File.Exists(fn))
                        File.Delete(fn);
                    f.feediconfailed = true;
                }
            }

            if (update)
                Invoke(() =>
                {
                    LoadFeeds();
                    Log("Feed icons downloaded.");
                });
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
                hd.LoadHtml(File.ReadAllText(filename, Encoding.UTF8));
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
                    mWebClient wc = new mWebClient();
                    feedxml = wc.DownloadString(feed.URL);
                    feed.LastUpdate = DateTime.Now;
                }
                catch (Exception ex)
                {
                    feed.LastError = ex.Message;
                    log($"ERROR {feed.Title}  : {ex.Message}");
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
                    FeedName = feed.Title,
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
                    if (img.StartsWith("http://"))  // FEATURE : upercase??
                        tempdesc = tempdesc.Replace(img, img.Replace("http://", _localhostimageurl));
                    else
                        tempdesc = tempdesc.Replace(img, img.Replace("https://", _localhostimageurl));
                }

                if (feed.DownloadImages)
                    imgs.ForEach(x => _downloadimglist.Enqueue(x));

                i.Description = tempdesc;
                list.Add(i);
            }
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

            if (list.Count(x => x.isRead == false) > 0)
                _newItemsExist = true;

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
                toolProgressBar.Visible = true;
                toolProgressBar.Maximum = c + 1;
                toolProgressBar.Value = i;
                foreach (var f in _feeds)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        UpdateFeed(f, Log);
                        Invoke(() =>
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
                    Invoke(() =>
                    {
                        toolProgressBar.Visible = false;
                        toolProgressBar.Value = 0;
                        toolCount.Text = "";
                        _lastUpdate = DateTime.Now;
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
            try
            {
                webBrowser1.Document.DomDocument.GetType().GetProperty("designMode").SetValue(webBrowser1.Document.DomDocument, "Off", null);
            }
            catch
            {//(Exception ex){
                //_log.Error(ex);
            }
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

            Log("");

            var str = item.Description;
            str = str.Replace("{port}", Settings.webport.ToString());

            StringBuilder sb = new StringBuilder();
            sb.Append("<html");
            if (_currentFeed.RTL)
                sb.Append(" dir='rtl'>"); // get if rtl
            else
                sb.AppendLine(">");
            sb.Append("<head><meta charset='UTF-8'><meta http-equiv='cache-control' content='no-cache'></head>");
            sb.Append("<link rel='stylesheet' href='http://localhost:" + Settings.webport + "/style.css'>");
            sb.Append("<div class='title'>");
            sb.Append("<h2><a href='" + item.Link + "'>" + item.Title + "</a></h2>");
            if (item.isStarred)
                sb.Append("<img src='star.png' />");
            sb.Append("<label>");
            sb.Append("" + item.Author);
            sb.Append("</label>");
            sb.Append("<label>");
            sb.Append("" + item.Categories);
            sb.Append("</label>");
            sb.Append("<label>");
            sb.Append("" + item.date);
            sb.Append("</label>");
            sb.Append("<label>");
            sb.Append("" + item.FeedName);
            sb.Append("</label>");
            sb.Append("</div>");
            sb.AppendLine(str);
            sb.AppendLine("</html>");

            _currhtml = sb.ToString();
            webBrowser1.Refresh(WebBrowserRefreshOption.Completely);

            if (item.isRead != isread)
            {
                item.isRead = isread;
                UpdateFeedCount();
            }
        }

        private void UpdateStarCount()
        {
            treeView1.BeginUpdate();

            var ur = treeView1.Nodes.Find("Starred", true);
            if (ur.Length > 0)
            {
                long c = 0;
                foreach (var f in _feeditems)
                    c += f.Value.Count(x => x.isStarred);
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
                    int c = 0;
                    int tot = 0;
                    foreach (var f in _feeditems)
                        tot += f.Value.Count;

                    foreach (var f in _feeds)
                        c += f.UnreadCount;

                    if (c > 0)
                    {
                        ur[0].Text = $"Unread ({c} of {tot})";
                        ur[0].NodeFont = new Font(treeView1.Font, FontStyle.Bold);
                    }
                    else
                    {
                        ur[0].Text = $"Unread (0 of {tot})";
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
                int i = _feeds.FindIndex(x => x.Title == _currentFeed.Title);
                int c = _feeds.Count;
                i++;
                while (c > 0)
                {
                    if (i == _feeds.Count)
                        i = 0;
                    c--;

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
            _newItemsExist = false; // reset tray icon
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

                listView1.FocusedItem = l;
            }
        }

        private void SaveFeeds()
        {
            File.WriteAllText("configs\\settings.config", JSON.ToNiceJSON(new Settings(), jp));
            File.WriteAllText("feeds\\feeds.list", JSON.ToNiceJSON(_feeds, jp));
            File.WriteAllText("feeds\\downloadimg.list", JSON.ToJSON(_downloadimglist, jp));
            foreach (var i in _feeditems)
            {
                File.WriteAllText(GetFeedFilename(i.Key), JSON.ToNiceJSON(i.Value, jp));
            }
        }

        private void Shutdown()
        {
            if (this.WindowState == FormWindowState.Normal)
                Settings.Maximized = false;
            else
                Settings.Maximized = true;
            SaveFeeds();
            //_imageCache.Shutdown();
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
                    listView1.RightToLeft = feed.RTL ? RightToLeft.Yes : RightToLeft.No;
                    listView1.RightToLeftLayout = feed.RTL;
                    ShowFeedList(list);
                    UpdateFeedCount(feed, list);
                }
            }
            catch //(Exception ex)
            {

            }
        }

        private void ShowFeedList(List<FeedItem> list)
        {
            listView1.Items.Clear();
            listView1.View = View.Details;

            if (list != null)
            {
                _currentList = list;
                listView1.SuspendLayout();
                listView1.BeginUpdate();
                listView1.Groups.Clear();
                var today = new ListViewGroup("Today");
                today.HeaderAlignment = HorizontalAlignment.Left;
                var yesterday = new ListViewGroup("Yesterday");
                yesterday.HeaderAlignment = HorizontalAlignment.Left;
                var thisweek = new ListViewGroup("This Week");
                thisweek.HeaderAlignment = HorizontalAlignment.Left;
                var older = new ListViewGroup("Older");
                older.HeaderAlignment = HorizontalAlignment.Left;

                listView1.Groups.Add(today);
                listView1.Groups.Add(yesterday);
                listView1.Groups.Add(thisweek);
                listView1.Groups.Add(older);
                listView1.ShowGroups = true;
                foreach (var i in list)
                {
                    var lvi = new ListViewItem();
                    lvi.Name = "Title";
                    lvi.Text = i.Title;
                    lvi.Tag = i;
                    lvi.SubItems.Add(i.date.ToString());
                    var grp = today;
                    var d = DateTime.Now.Subtract(i.date).TotalDays;
                    if (d < 1)
                        grp = today;
                    else if (d >= 1 & d < 2)
                        grp = yesterday;
                    else if (d >= 2 && d < 7)
                        grp = thisweek;
                    else
                        grp = older;
                    lvi.Group = grp;
                    listView1.Items.Add(lvi);
                    if (i.isRead == false)
                        lvi.Font = new Font(lvi.Font, FontStyle.Bold);
                }
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.EndUpdate();
                listView1.ResumeLayout();
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
        //private string GetFeedXmlFilename(Feed f)
        //{
        //    return "feeds\\temp\\" + f.Title.Replace(':', ' ').Replace('/', ' ').Replace('\\', ' ') + ".xml";
        //}
        private string GetTempHTMLFilename(string title)
        {
            return "feeds\\temp\\" + title + ".html";
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
            if (msg != "" && msg!= " ")
                _log.Info(msg);
            Invoke(() => { toolMessage.Text = msg; });
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
        // ---------------------------------------------------------------------------------------------------------
        // UI handlers
        // ---------------------------------------------------------------------------------------------------------

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().StartsWith("http://localhost:" + Settings.webport) == false)
            {
                e.Cancel = true;
                Process.Start(e.Url.ToString());
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_exit == true)
                Shutdown();
            else
            {
                //  minimize
                _lastFormState = this.WindowState;
                e.Cancel = true;
                notifyIcon1.Visible = true;
                this.Hide();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // after feed select -> show feed item list 
            var feed = e.Node.Tag as Feed;
            if (feed != null)
            {
                _currentFeed = feed;
                ShowFeedList(feed);
            }
            else
            {
                listView1.RightToLeft = RightToLeft.No;
                listView1.RightToLeftLayout = false;
                List<FeedItem> list = new List<FeedItem>();
                if (e.Node.Name == "Unread")
                {
                    foreach (var f in _feeditems)
                        list.AddRange(f.Value.FindAll(x => x.isRead == false));

                    ShowFeedList(list);
                }
                else if (e.Node.Name == "Starred")
                {
                    foreach (var f in _feeditems)
                        list.AddRange(f.Value.FindAll(x => x.isStarred == true));

                    ShowFeedList(list);
                }
            }
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

        private bool _exit = false;

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _exit = true;
            this.Close();
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                Invoke(() => { ShowFeedList(f); });
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
            if (listView1.FocusedItem == null)
                return;
            ShowItem(listView1.FocusedItem.Tag as FeedItem);
            listView1.FocusedItem.Font = new Font(listView1.FocusedItem.Font, FontStyle.Regular);
        }

        private void starToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleStarred();
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
                    // rename files
                    string old = form.feed.Title;
                    try
                    {
                        File.Move(GetFeedFilename(old), GetFeedFilename(f.Title));
                    }
                    catch { }
                    var l = _feeditems[old];
                    _feeditems.TryRemove(old, out l);
                    _feeditems.TryAdd(f.Title, l);
                    _feeds.Find(x => x.Title == old).Title = f.Title;

                    treeView1.SelectedNode.Name = f.Title;
                    form.feed.Title = f.Title;
                    UpdateFeedCount(form.feed);
                }
                form.feed.URL = f.URL;
                form.feed.RTL = f.RTL;
                form.feed.DownloadImages = f.DownloadImages;
                SaveFeeds();
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
            // force download image for item now
            var f = listView1.FocusedItem.Tag as FeedItem;
            if (f != null)
            {
                List<string> imgs = new List<string>();
                foreach (var img in GetImagesInHTMLString(f.Description))
                {
                    var s = _imghrefregex.Match(img).Groups["href"].Value;
                    if (_imageCache.Contains(s) == false)
                        imgs.Add(s);
                }
                Task.Factory.StartNew(() =>
                {
                    string err = "";
                    foreach (var i in imgs)
                    {
                        string key = i.Replace(_localhostimageurl, "");
                        string url = "http://" + key;
                        err = DownloadImage(f, key, url);
                    }
                    Invoke(() =>
                    {
                        ShowItem(f);
                        if (err != "")
                            Log(err);
                    });
                });
            }
        }

        private string DownloadImage(FeedItem f, string key, string url)
        {
            string err = "";

            try
            {
                mWebClient wc = new mWebClient();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "HEAD";
                long len;
                using (HttpWebResponse resp = (HttpWebResponse)(req.GetResponse()))
                {
                    len = resp.ContentLength;
                }
                if (len < Settings.DownloadImagesUnderKB * 1024)
                {
                    var b = wc.DownloadData(url);
                    //var o = new ImgCache
                    //{
                    //    FeedName = f.FeedName,
                    //    Title = f.Title,
                    //    data = b
                    //};
                    _imageCache.Add(key, b);
                }
                else
                    err = $"Image over size limit {Settings.DownloadImagesUnderKB}KB : {(len / 1024).ToString("#,#")}KB.";
            }
            catch
            {
                err = "Error downloading images";
            }
            return err;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // show form
                this.Show();
                this.WindowState = _lastFormState;
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }
        }

        private void cleanupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // cleanup old items
            int c = 0;
            foreach (var f in _feeditems)
                c += f.Value.Count(x =>
                    DateTime.Now.Subtract(x.date).TotalDays >= Settings.CleanupItemAfterDays
                    && x.isStarred == false
                    && x.isRead == true);
            if (c == 0)
            {
                MessageBox.Show("No items to remove.");
                return;
            }
            var r = MessageBox.Show($"Do you want to remove {c} items?", "Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (r == DialogResult.Yes)
            {
                foreach (var f in _feeditems)
                {
                    // FEATURE : cleanup -> remove images from cache also
                    f.Value.RemoveAll(x =>
                       DateTime.Now.Subtract(x.date).TotalDays >= Settings.CleanupItemAfterDays
                       && x.isStarred == false
                       && x.isRead == true);
                    File.WriteAllText(GetFeedFilename(f.Key), JSON.ToNiceJSON(f.Value, jp));
                }

                UpdateFeedCount();
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                List<FeedItem> list = new List<FeedItem>();
                string s = txtSearch.Text;
                if (s == "")
                    return;
                foreach (var f in _feeditems)
                {
                    list.AddRange(f.Value.FindAll(x =>
                        x.Title.ToLower().Contains(s)
                        || x.Description.ToLower().Contains(s)));
                }
                ShowFeedList(list);
                Log($"{list.Count} items found.");
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.placeHolderTextBox1.SelectAll();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _exit = true;
            this.Close();
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show form
            this.Show();
            this.WindowState = _lastFormState;
        }

        private void Invoke(Action a)
        {
            this.Invoke((Delegate)a);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // fix : settings form here
            frmSettings f = new frmSettings();
            f.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // fix : about here
            AboutBox1 a = new AboutBox1();
            a.ShowDialog();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space && txtSearch.placeHolderTextBox1.Focused == false)
            {
                MoveNextUnread();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void logMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show log form
            frmLog f = new frmLog();
            f.Show();
        }

        private void downloadImagesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("download now?", "?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                toolProgressBar.Maximum = _downloadimglist.Count;
                toolProgressBar.Value = 0;
                toolProgressBar.Visible = true;

                mWebClient wc = new mWebClient();
                int c = 0;
                int tot = _downloadimglist.Count;
                while (_downloadimglist.Count > 0)
                {
                    if (DateTime.Now.Hour > 6)
                        return;
                    wc.Timeout = 4000;
                    string url = "";
                    _downloadimglist.TryDequeue(out url);
                    c++;
                    if (url != "" && url != null && _imageCache.Contains(url) == false)
                    {
                        try
                        {
                            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                            req.Method = "HEAD";
                            req.Timeout = wc.Timeout;
                            long len;
                            using (HttpWebResponse resp = (HttpWebResponse)(req.GetResponse()))
                            {
                                len = resp.ContentLength;
                            }
                            if (len < Settings.DownloadImagesUnderKB * 1024)
                            {
                                var b = wc.DownloadData(url);
                                _imageCache.Add(url, b);
                            }
                            else
                                Log($"Image over size limit {Settings.DownloadImagesUnderKB}KB : {(len / 1024).ToString("#,#")}KB.");
                        }
                        catch
                        {
                            Log("Error downloading images : " + url);
                        }
                    }
                    toolCount.Text = $"{c} of {tot}";
                    toolProgressBar.Maximum = _downloadimglist.Count;
                    toolProgressBar.Value = c;
                    Application.DoEvents();
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                webBrowser1.Document.DomDocument.GetType().GetProperty("designMode").SetValue(webBrowser1.Document.DomDocument, "On", null);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
    }
}
