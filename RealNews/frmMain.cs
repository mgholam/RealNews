using CodeHollow.FeedReader;
using fastJSON;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Westwind.Web.Utilities;

namespace RealNews
{
    // TODO : drag move feeds in folders
    // TODO : download, cleanup on folder -> all feeds down
    // TODO : dark mode scrollbar colours

    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            (this.webBrowser1.ActiveXInstance as SHDocVw.WebBrowser).NewWindow3 += FrmMain_NewWindow3;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Singleinstance._msgID && this.Visible == false)
            {
                this.Show();
                this.WindowState = _lastFormState;
            }
            base.WndProc(ref m);
        }

        private void FrmMain_NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
            // for a tags with target = new window
            Cancel = true;
            Process.Start(bstrUrl);
        }

        RealNewsWeb web;
        bool loaded = false;
        List<Feed> _feeds = new List<Feed>();
        ConcurrentDictionary<string, List<FeedItem>> _feeditems = new ConcurrentDictionary<string, List<FeedItem>>();
        Feed _currentFeed = null;
        string _feedTitle = "";
        List<FeedItem> _currentList = null;
        ConcurrentQueue<string> _downloadimglist = new ConcurrentQueue<string>();
        private Regex _imghrefregex = new Regex("src\\s*=\\s*[\'\"]\\s*(?<href>.*?)\\s*[\'\"]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private FormWindowState _lastFormState = FormWindowState.Normal;
        private JSONParameters jp = new JSONParameters { UseExtensions = false, UseEscapedUnicode = false, UseUTCDateTime = false };
        private string _localhostimageurl = "http://localhost:{port}/api/image?";
        private ImageCache _imageCache;
        private static ILog _log = LogManager.GetLogger(typeof(frmMain));
        private System.Timers.Timer _minuteTimer;
        private bool _newItemsExist = false;
        private bool _DoDownloadImages = false;
        private bool _run = true;
        private int _visibleItems = 10;
        Color _ThemeBackground = Color.White;
        Color _ThemeNormal = Color.Black;
        Color _ThemeHighLight = Color.Black;
        CheckBox _menuCheckBox = new CheckBox();


        private void Form1_Load(object sender, EventArgs e)
        {
            Application.ThreadException += Application_ThreadException;
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon1.Icon = Properties.Resources.tray;
            notifyIcon1.Visible = true;

            Directory.CreateDirectory("feeds\\temp");
            Directory.CreateDirectory("feeds\\lists");
            Directory.CreateDirectory("feeds\\icons");
            Directory.CreateDirectory("configs");

            // save light dark css
            if (File.Exists("configs\\light.css") == false)
                File.WriteAllText("configs\\light.css", Properties.Resources.light);
            if (File.Exists("configs\\dark.css") == false)
                File.WriteAllText("configs\\dark.css", Properties.Resources.dark);

            if (!File.Exists("configs\\search.plugin"))
                File.WriteAllText("configs\\search.plugin", "public static string Process(string title)\r\n{\r\n\treturn title;\r\n}");

            JSON.Parameters.UseUTCDateTime = false;
            if (File.Exists("configs\\settings.config"))
                JSON.FillObject(new Settings(), File.ReadAllText("configs\\settings.config"));
            if (Settings.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;

            SetTheme();

            rssImages.Images.Add(Properties.Resources.folder);
            rssImages.Images.Add(Properties.Resources.rss);
            rssImages.Images.Add(Properties.Resources.news_new);
            rssImages.Images.Add(Properties.Resources.star_yellow);
            rssImages.Images.Add(Properties.Resources.Search);

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

            Task.Factory.StartNew(DownloadThread);

            _minuteTimer = new System.Timers.Timer(1000);
            _minuteTimer.Elapsed += _minuteTimer_Elapsed;
            _minuteTimer.AutoReset = true;
            _minuteTimer.Enabled = true;

            if (Settings.MGFeatures)
            {
                _menuCheckBox.Text = "System Proxy";
                _menuCheckBox.ForeColor = Color.White;// menuStrip1.Items[0].ForeColor;
                _menuCheckBox.BackColor = Color.Transparent;
                _menuCheckBox.CheckStateChanged += Cb_CheckStateChanged;
                _menuCheckBox.Checked = Settings.UseSytemProxy;
                ToolStripControlHost host = new ToolStripControlHost(_menuCheckBox);
                host.ForeColor = _menuCheckBox.ForeColor;
                menuStrip1.Items.Add(host);
            }
        }

        private void Cb_CheckStateChanged(object sender, EventArgs e)
        {
            Settings.UseSytemProxy = (sender as CheckBox).Checked;
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            _log.Error(e.Exception);
            Log("" + e.Exception.Message);
        }

        private void DownloadThread()
        {
            while (_run)
            {
                if (_DoDownloadImages && _downloadimglist.Count > 0)
                {
                    // download image thread
                    int c = 0;
                    int tot = _downloadimglist.Count;
                    Invoke(() =>
                    {
                        toolProgressBar.Value = 0;
                        toolProgressBar.Maximum = tot;
                        toolProgressBar.Visible = true;
                    });

                    while (_DoDownloadImages && _run)
                    {
                        for (int i = 0; i < 200 && _downloadimglist.Count > 0; i++)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                _downloadimglist.TryDequeue(out string url);
                                c++;
                                var ret = downloadImageFile(url);

                                if (ret.myContains("timed out")) // retry if timed out
                                {
                                    _downloadimglist.Enqueue(url);
                                }
                                Invoke(() =>
                                {
                                    toolCount.Text = $"{_downloadimglist.Count} images left";
                                    toolProgressBar.Maximum = tot;
                                    toolProgressBar.Value = c;
                                });
                            });
                        }
                        Thread.Sleep(4000);
                    }
                    Invoke(() =>
                    {
                        toolCount.Text = "";
                        toolProgressBar.Visible = false;
                        Log("Images done.");
                    });
                }
                else
                {
                    Invoke(() =>
                    {
                        toolProgressBar.Visible = false;
                    });
                }

                Thread.Sleep(1000);
            }
        }

        private string downloadImageFile(string url)
        {
            if (url == "" || url == null)
                return "";

            var ret = DownloadImage(url);
            Log(ret);
            return ret;
        }

        private string DownloadImage(string url)//string key, string url)
        {
            string err = "";
            url = Uri.UnescapeDataString(url);
            //url = url.Replace("&amp;", "&");
            url = url.Replace("amp;", "");
            if (_imageCache.Contains(url))
                return "Image already downloaded";

            try
            {
                long len;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                if (Settings.UseSytemProxy) // else define a proxy
                    req.Proxy = WebRequest.DefaultWebProxy;
                else if (Settings.CustomProxy != "")
                    req.Proxy = new WebProxy(Settings.CustomProxy);

                req.Timeout = 4000;
                req.Method = "HEAD";
                req.UserAgent = "Other";
                using (HttpWebResponse resp = (HttpWebResponse)(req.GetResponse()))
                {
                    len = resp.ContentLength;
                }

                if (len < Settings.DownloadImagesUnderKB * 1024)
                {
                    mWebClient wc = new mWebClient();
                    wc.Timeout = 10000;
                    wc.DownloadFileAsync(new Uri(url), _imageCache.GetFilename(url));
                }
                else
                    err = $"Image over size limit {Settings.DownloadImagesUnderKB}KB : {(len / 1024).ToString("#,#")}KB.";
            }
            catch (Exception ex)
            {
                err = "Error downloading images : " + ex.Message;
            }
            return err;
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
                if (now.Subtract(Settings.LastUpdateTime).TotalMinutes > Settings.GlobalUpdateEveryMin)
                {
                    UpdateAll();
                    Settings.LastUpdateTime = now;
                }

                _feeds.FindAll(
                    x => x.UpdateEveryMin > 0 &&
                    now.Subtract(x.LastUpdate).TotalMinutes > x.UpdateEveryMin
                    ).ForEach(f =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        UpdateFeed(f, Log);
                        UpdateFeedCount(f);
                    });
                });

                // download images
                var start = TimeSpan.Parse(Settings.StartDownloadImgTime);
                var end = TimeSpan.Parse(Settings.EndDownloadImgTime);
                TimeSpan timeBetween = now.TimeOfDay;

                _DoDownloadImages = false;
                if (timeBetween >= start && timeBetween < end)
                    _DoDownloadImages = true;
            }
        }

        private string _currhtml = "<html><link rel='stylesheet' href='http://localhost:" + Settings.webport + "/style.css'></html>";
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
                _feeds = JSON.ToObject<List<Feed>>(File.ReadAllText("feeds\\feeds.list")).OrderBy(x => x.Title).ToList();
                treeView1.BeginUpdate();

                treeView1.Nodes.Clear();

                AddTreeViewMain();

                _feeds.FindAll(x => x.Folder != "").OrderBy(x => x.Folder).ToList().ForEach(x => AddFeedToTree(x));

                _feeds.FindAll(x => x.Folder == "").OrderBy(x => x.Title).ToList().ForEach(x => AddFeedToTree(x));

                treeView1.EndUpdate();
            }
            else
            {
                AddTreeViewMain();
            }

            if (_feeds.Count > 0)
            {
                UpdateStarCount();
                UpdateAllFeedCounts();
            }
            splitContainer1.Visible = true;
            GC.Collect();
        }

        private void UpdateAllFeedCounts()
        {
            _feeds.ForEach(x => UpdateFeedCount(x));
        }

        private void AddTreeViewMain()
        {
            var tn = treeView1.Nodes.Add("Unread");
            tn.ForeColor = _ThemeNormal;
            tn.Name = "Unread";
            tn.ImageIndex = 2;
            tn.SelectedImageIndex = 2;
            tn = treeView1.Nodes.Add("Starred");
            tn.ForeColor = _ThemeNormal;
            tn.Name = "Starred";
            tn.ImageIndex = 3;
            tn.SelectedImageIndex = 3;
            tn = treeView1.Nodes.Add("Search Results");
            tn.ForeColor = _ThemeNormal;
            tn.Name = "Search";
            tn.ImageIndex = 4;
            tn.SelectedImageIndex = 4;
        }

        private TreeNode AddFeedToTree(Feed f)
        {
            TreeNode tn;
            var fn = "feeds\\icons\\" + GetFeedFilenameOnly(f) + ".ico";
            if (File.Exists(GetFeedFilename(f)))
            {
                var list = JSON.ToObject<List<FeedItem>>(File.ReadAllText(GetFeedFilename(f)));
                _feeditems.TryAdd(f.Title, list);
            }
            int imgidx = 1;
            if (File.Exists(fn))
            {
                try
                {
                    rssImages.Images.Add(Image.FromFile(fn));
                    imgidx = rssImages.Images.Count - 1;
                }
                catch { }
            }
            tn = TreeViewHelper.AddTreeNode(treeView1, f.Folder, f, f.Title);
            tn.ImageIndex = imgidx;
            tn.SelectedImageIndex = imgidx;

            tn.ForeColor = _ThemeNormal;
            if (f.LastError != "")
                tn.ForeColor = Color.Red;

            if (f.UnreadCount > 0)
            {
                tn.NodeFont = new Font(treeView1.Font, FontStyle.Bold);
                tn.Text = f.Title + $" ({f.UnreadCount})";
                tn.ForeColor = _ThemeHighLight;
            }
            else
                tn.Text = f.Title;
            return tn;
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
            feedContextMenu.Renderer = r;
            itemContextMenu.Renderer = r;

            this.Invalidate();
        }

        private void SetTheme()
        {
            if (Settings.DarkMode)
            {
                File.Copy("configs/dark.css", "configs/style.css", true);
                _ThemeBackground = Color.FromArgb(32, 32, 32);
                _ThemeNormal = Color.Silver;// DimGray;
                _ThemeHighLight = Color.White;
                myListBox1.GroupColor = _ThemeHighLight;
            }
            else
            {
                File.Copy("configs/light.css", "configs/style.css", true);
                _ThemeBackground = Color.White;
                _ThemeNormal = Color.Black;
                _ThemeHighLight = Color.Black;
                myListBox1.GroupColor = Color.Blue;
            }

            treeView1.BackColor = _ThemeBackground;
            treeView1.ForeColor = _ThemeNormal;

            panel1.BackColor = _ThemeBackground;
            panel2.BackColor = _ThemeBackground;

            myListBox1.BackColor = _ThemeBackground;
            myListBox1.ForeColor = _ThemeNormal;
            myListBox1.HighLightText = _ThemeHighLight;

            button1.ForeColor = _ThemeHighLight;
            placeHolderTextBox1.BackColor = _ThemeBackground;
            placeHolderTextBox1.ForeColor = _ThemeNormal;
            placeHolderTextBox1.NormalColor = _ThemeNormal;
            placeHolderTextBox1.HighlightColor = _ThemeHighLight;
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

        private void UpdateFeed(Feed feed, Action<string> log)
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
                    Log($"feed {feed.Title} xml size {feedxml.Length.ToString("#,#")}");
                    //File.WriteAllText(GetFeedXmlFilename(feed), feedxml);
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
                if (item.PublishingDate != null &&
                    DateTime.Now.Subtract(item.PublishingDate.Value).TotalDays > Settings.SkipFeedItemsDaysOlderThan)
                    continue;

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
                            if (long.TryParse(j.Value, out long val))
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
                    if (img.StartsWith("http://"))
                        tempdesc = tempdesc.Replace(img, img.Replace("http://", _localhostimageurl));
                    else
                        tempdesc = tempdesc.Replace(img, img.Replace("https://", _localhostimageurl));
                }

                if (feed.DownloadImages)
                    imgs.ForEach(x =>
                    {
                        if (_imageCache.Contains(x) == false)
                            _downloadimglist.Enqueue(x);
                    });

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
                //list = o.OrderBy(x=>x.date).ToList();
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
                            // cureent feed refresh on update 
                            if (_currentFeed == f)
                                ShowFeedList(f);
                            UpdateFeedCount(f);
                            toolCount.Text = $"{i++} of {c}";
                            toolProgressBar.Value = i;
                        });
                    }));
                }
                Task.Factory.StartNew(() =>
                {
                    Task.WaitAll(tasks.ToArray(), Settings.FeedUpdateTimeout);
                    Invoke(() =>
                    {
                        toolProgressBar.Visible = false;
                        toolProgressBar.Value = 0;
                        toolCount.Text = "";
                        Settings.LastUpdateTime = DateTime.Now;
                        SaveFeeds();
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
            if (isread)
                _newItemsExist = false;

            // show item
            //if (item == null)
            //{
            //    return;
            //}
            if (item == null || item.Id == "")
                return;

            Log("");

            var str = item.Description;
            str = str.Replace("{port}", Settings.webport.ToString());

            StringBuilder sb = new StringBuilder();
            sb.Append("<html");
            if (item.RTL)
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
            webBrowser1.SuspendLayout();
            webBrowser1.Refresh(WebBrowserRefreshOption.Completely);
            webBrowser1.Document.Window.ScrollTo(0, 0);
            webBrowser1.ResumeLayout();

            if (item.isRead != isread)
            {
                item.isRead = isread;
                UpdateFeedCount();
            }
        }

        private void UpdateStarCount()
        {
            treeView1.SuspendLayout();
            treeView1.BeginUpdate();

            var ur = treeView1.Nodes.Find("Starred", true)[0];
            long c = _feeditems.Sum(f => f.Value.Count(x => x.isStarred));
            if (c > 0)
                ur.Text = $"Starred ({c})";
            else
                ur.Text = "Starred";

            treeView1.EndUpdate();
            treeView1.ResumeLayout();
        }

        private void UpdateFeedCount(Feed feed)
        {
            if (_feeditems.TryGetValue(feed.Title, out List<FeedItem> list))
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
                treeView1.SuspendLayout();
                treeView1.BeginUpdate();

                if (list != null && feed != null)
                {
                    feed.UnreadCount = list.Count(x => x.isRead == false);
                    var n = treeView1.Nodes.Find(feed.Title, true)[0];
                    n.ForeColor = _ThemeNormal;
                    if (feed.LastError != "")
                        n.ForeColor = Color.Red;

                    if (feed.UnreadCount > 0)
                    {
                        n.Text = feed.Title + $" ({feed.UnreadCount})";
                        n.NodeFont = new Font(treeView1.Font, FontStyle.Bold);
                        n.ForeColor = _ThemeHighLight;
                    }
                    else
                    {
                        n.Text = feed.Title;
                        n.NodeFont = new Font(treeView1.Font, FontStyle.Regular);
                    }
                    // set folder count too
                    if (feed.Folder != "")
                    {
                        var cc = _feeds.FindAll(x => x.Folder == feed.Folder).Sum(x => x.UnreadCount);
                        var u = treeView1.Nodes.Find(feed.Folder, true)[0];

                        if (cc > 0)
                        {
                            u.Text = u.Name + $" ({cc})";
                            u.NodeFont = new Font(treeView1.Font, FontStyle.Bold);
                            u.ForeColor = _ThemeHighLight;
                        }
                        else
                        {
                            u.Text = u.Name;
                            u.NodeFont = new Font(treeView1.Font, FontStyle.Regular);
                            u.ForeColor = _ThemeNormal;
                        }
                    }
                }

                var ur = treeView1.Nodes.Find("Unread", true)[0];
                int tot = _feeditems.Sum(x => x.Value.Count);
                int c = _feeds.Sum(x => x.UnreadCount);

                if (c > 0)
                {
                    ur.Text = $"Unread ({c} of {tot})";
                    ur.NodeFont = new Font(treeView1.Font, FontStyle.Bold);
                    ur.ForeColor = _ThemeHighLight;
                }
                else
                {
                    ur.Text = $"Unread (0 of {tot})";
                    ur.NodeFont = new Font(treeView1.Font, FontStyle.Regular);
                    ur.ForeColor = _ThemeNormal;
                }
            }
            catch //(Exception ex)
            {

            }
            finally
            {
                treeView1.EndUpdate();
                treeView1.ResumeLayout();
            }
        }

        private void MoveNextUnread()
        {
            // handle folders when moving next
            if (_currentFeed == null)
                _currentFeed = _feeds[0];
            bool found = false;
            // move next
            if (_currentFeed.UnreadCount == 0)
            {
                // next feed down
                var f = _feeds.FindAll(x => x.UnreadCount > 0 &&
                                            x.FullTitle.CompareTo(_currentFeed.FullTitle) > 0)
                                          .OrderBy(x => x.FullTitle)
                                          .ToList();
                if (f.Count() > 0)
                {
                    _currentFeed = f[0];
                    found = true;
                }

                // wrap around
                if (found == false)
                {
                    f = _feeds.FindAll(x => x.UnreadCount > 0)
                                          .OrderBy(x => x.FullTitle)
                                          .ToList();
                    if (f.Count() > 0)
                        _currentFeed = f[0];
                }

                ShowFeedList(_currentFeed);
                // focus feed in treeview
                var n = treeView1.Nodes.Find(_currentFeed.Title, true);
                if (n.Length > 0)
                    treeView1.SelectedNode = n[0];
            }
            ShowNextItem();
        }

        private void ShowNextItem()
        {
            _newItemsExist = false; // reset tray icon

            int i = 0;
            while (i < myListBox1.Items.Count)
            {
                var f = myListBox1.Items[i] as FeedItem;
                if (f.isRead == false && f.Id != "")
                {
                    myListBox1.SelectedItems.Clear();
                    myListBox1.SelectedIndex = i;
                    myListBox1.EnsureVisible(i, _visibleItems);
                    ShowItem(myListBox1.SelectedItem as FeedItem);
                    return;
                }
                i++;
            }
        }

        private void SaveFeeds()
        {
            lock (_lock)
            {
                File.WriteAllText("configs\\settings.config", JSON.ToNiceJSON(new Settings(), jp));
                File.WriteAllText("feeds\\feeds.list", JSON.ToNiceJSON(_feeds, jp));
                File.WriteAllText("feeds\\downloadimg.list", JSON.ToNiceJSON(_downloadimglist, jp));
                foreach (var i in _feeditems)
                {
                    File.WriteAllText(GetFeedFilename(i.Key), JSON.ToNiceJSON(i.Value, jp));
                }
            }
            GC.Collect();
        }

        private void Shutdown()
        {
            _run = false;
            if (this.WindowState == FormWindowState.Normal)
                Settings.Maximized = false;
            else
                Settings.Maximized = true;
            SaveFeeds();
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
                    if (list != null)
                    {
                        list.ForEach(x => x.RTL = feed.RTL);
                        myListBox1.RightToLeft = feed.RTL ? RightToLeft.Yes : RightToLeft.No;
                        ShowFeedList(list);
                        UpdateFeedCount(feed, list);
                    }
                }
            }
            catch //(Exception ex)
            {

            }
        }
        private object _fllock = new object();
        private void ShowFeedList(List<FeedItem> list)
        {
            lock (_fllock)
            {
                if (list != null)
                {
                    _currentList = list;
                    myListBox1.SuspendLayout();
                    myListBox1.BeginUpdate();

                    myListBox1.Items.Clear();
                    var last = "";

                    foreach (var i in list)
                    {
                        var d = DateTime.Now.Subtract(i.date).TotalDays;
                        var grp = "";
                        if (d < 1)
                            grp = "Today";
                        else if (d >= 1 & d < 2)
                            grp = "Yesterday";
                        else if (d >= 2 && d < 7)
                            grp = "This week";
                        else
                            grp = "Older";

                        if (last != grp)
                        {
                            myListBox1.Items.Add(new FeedItem { Title = grp, Id = "" });
                            last = grp;
                        }
                        myListBox1.Items.Add(i);
                    }
                    myListBox1.EndUpdate();
                    myListBox1.ResumeLayout();
                }
            }
        }

        private string GetFeedFilenameOnly(Feed f)
        {
            return f.Title.Replace(':', ' ').Replace('/', ' ').Replace('\\', ' ');
        }
        private string GetFeedFilename(Feed f)
        {
            return "feeds\\lists\\" + GetFeedFilenameOnly(f) + ".list";
        }
        private string GetFeedFilename(string title)
        {
            return "feeds\\lists\\" + title.Replace(':', ' ').Replace('/', ' ').Replace('\\', ' ') + ".list";
        }
        private string GetFeedXmlFilename(Feed f)
        {
            return "feeds\\temp\\" + GetFeedFilenameOnly(f) + ".xml";
        }

        private void Log(string msg)
        {
            if (msg != "" && msg != " ")
                _log.Info(msg);
            Invoke(() => { toolMessage.Text = msg; });
        }

        private void ToggleStarred()
        {
            if (myListBox1.SelectedItem != null)
                return;
            // toggle star
            var f = myListBox1.SelectedItem as FeedItem;
            if (f != null && f.Id != "")
            {
                f.isStarred = !f.isStarred;
                UpdateStarCount();
                ShowItem(f);
            }
        }

        #region ----------------- UI handlers ---------------------
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
                if (Settings.OnCloseMinimize)
                {
                    //  minimize
                    _lastFormState = this.WindowState;
                    e.Cancel = true;
                    notifyIcon1.Visible = true;
                    this.Hide();
                }
                else
                    Shutdown();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // after feed select -> show feed item list 
            var feed = e.Node.Tag as Feed;
            if (feed != null)
            {
                _currentFeed = feed;
                _feedTitle = feed.Title;
                ShowFeedList(feed);
                if (_feeditems.TryGetValue(feed.Title, out List<FeedItem> fl))
                    Log(feed.Title + " item count = " + fl.Count);
            }
            else
            {
                ShowFeedFromTitle(e.Node.Name);
            }
        }

        private void ShowFeedFromTitle(string title)
        {
            myListBox1.RightToLeft = RightToLeft.No;
            List<FeedItem> list = new List<FeedItem>();
            _feedTitle = title;

            if (title == "Unread")
            {
                foreach (var f in _feeditems)
                    list.AddRange(f.Value.FindAll(x => x.isRead == false));

                list = list.OrderByDescending(x => x.date).ToList();

                ShowFeedList(list);
            }
            else if (title == "Starred")
            {
                foreach (var f in _feeditems)
                    list.AddRange(f.Value.FindAll(x => x.isStarred == true));

                list = list.OrderByDescending(x => x.date).ToList();

                ShowFeedList(list);
            }
            else if (title == "Search")
            {
                ShowSearchResults();
            }
            else
            {
                // category selected 
                var f = _feeds.FindAll(x => x.Folder == title).ToList();
                foreach (var ff in f)
                    list.AddRange(_feeditems[ff.Title]);

                list = list.OrderByDescending(x => x.date).ToList();

                ShowFeedList(list);
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
            // resize bug 
            myListBox1.DrawMode = DrawMode.OwnerDrawFixed;
            myListBox1.DrawMode = DrawMode.OwnerDrawVariable;
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
            var n = treeView1.SelectedNode;
            var f = n.Tag as Feed;
            if (f != null)
                _currentList.ForEach(x => x.isRead = true);
            else
                _feeds.FindAll(x => x.Folder == n.Name).ForEach(x => _feeditems[x.Title].ForEach(o => o.isRead = true));

            UpdateAllFeedCounts();
            ShowFeedList(_currentFeed);
        }

        private void updateNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var n = treeView1.SelectedNode;
            var f = n.Tag as Feed;
            if (f != null)
            {
                Task.Factory.StartNew(() =>
                {
                    UpdateFeed(f, Log);
                    Invoke(() => { ShowFeedList(f); });
                });
            }
            else
            {
                _feeds.FindAll(x => x.Folder == n.Name).ForEach(x =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        UpdateFeed(x, Log);
                        Invoke(() => { ShowFeedList(x); });
                    });
                });
            }
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
            ShowItem(myListBox1.SelectedItem as FeedItem);
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            ShowItem(myListBox1.SelectedItem as FeedItem);
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
                    //MessageBox.Show
                    Log(f.Title + " Already exists");//, "Error");
                    return;
                }
                _feeds.Add(f);
                SaveFeeds();
                LoadFeeds();
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
            if (form.feed == null)
            {
                Log("Please select a feed first");
                return;
            }
            if (form.ShowDialog() == DialogResult.OK)
            {
                var f = form.ret;
                if (f == null)
                {
                    var dr = MessageBox.Show($"Do you want to delete feed : {form.feed.Title} ?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                    if (dr == DialogResult.Yes)
                    {
                        // delete feed
                        _feeds.Remove(form.feed);
                        treeView1.Nodes.Remove(treeView1.SelectedNode);
                        SaveFeeds();
                    }
                    return;
                }
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
                bool reload = false;
                if (form.feed.Folder != f.Folder)
                    reload = true;
                form.feed.URL = f.URL;
                form.feed.RTL = f.RTL;
                form.feed.Folder = f.Folder;
                form.feed.UpdateEveryMin = f.UpdateEveryMin;
                form.feed.DownloadImages = f.DownloadImages;
                form.feed.ExcludeInCleanup = f.ExcludeInCleanup;
                SaveFeeds();
                if (reload)
                    LoadFeeds();
            }
        }

        private void toggleStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleStarred();
        }

        private void markUnreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // mark unread

            foreach (FeedItem fi in myListBox1.SelectedItems)
            {
                if (fi != null && fi.Id != "")
                    fi.isRead = false;// !f.isRead;
            }
            UpdateFeedCount();
        }

        private void downloadImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // force download image for item now
            var f = myListBox1.SelectedItem as FeedItem;
            if (f != null)
            {
                internalDownloadImage(f, true);
            }
        }

        private void internalDownloadImage(FeedItem f, bool show)
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
                    string url = "https://" + key;
                    err = DownloadImage(url);
                }
                Thread.Sleep(4000);
                if (show)
                {
                    Invoke(() =>
                    {
                        ShowItem(f);
                        if (err != "")
                            Log(err);
                    });
                }
            });
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
            // global cleanup old items
            int c = 0;
            var expr = new Predicate<FeedItem>(x =>
                DateTime.Now.Subtract(x.date).TotalDays >= Settings.CleanupItemAfterDays
                && x.isStarred == false
                && x.isRead == true);

            // clear errors 
            _feeds.ForEach(feed =>
            {
                var n = treeView1.Nodes.Find(feed.Title, true)[0];
                if (feed.LastError != "")
                {
                    feed.LastError = "";
                    n.ForeColor = _ThemeNormal;
                    if (n.NodeFont.Bold == true)
                        n.ForeColor = _ThemeHighLight;
                }
            });

            foreach (var f in _feeditems)
            {
                var ff = _feeds.Find(x => x.Title == f.Key);
                if (ff != null && ff.ExcludeInCleanup)
                    continue;

                c += f.Value.Count(x =>
                    DateTime.Now.Subtract(x.date).TotalDays >= Settings.CleanupItemAfterDays
                    && x.isStarred == false
                    && x.isRead == true);
            }

            if (c == 0)
            {
                //MessageBox.Show
                Log("No items to remove.");
                return;
            }
            var r = MessageBox.Show($"Do you want to remove {c} items?", "Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (r == DialogResult.Yes)
            {
                List<string> imgs = new List<string>();
                foreach (var f in _feeditems)
                {
                    var ff = _feeds.Find(x => x.Title == f.Key);
                    if (ff != null && ff.ExcludeInCleanup)
                        continue;

                    var list = f.Value.FindAll(expr);
                    list.ForEach(x =>
                    {
                        foreach (var img in GetImagesInHTMLString(x.Description))
                        {
                            var s = _imghrefregex.Match(img).Groups["href"].Value;
                            var url = s.Replace(_localhostimageurl, "");
                            imgs.Add(url);
                        }
                    });

                    f.Value.RemoveAll(expr);
                    File.WriteAllText(GetFeedFilename(f.Key), JSON.ToNiceJSON(f.Value, jp));
                }

                _imageCache.Remove(imgs);
                UpdateAllFeedCounts();
                Task.Factory.StartNew(SaveFeeds);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                treeView1.SelectedNode = treeView1.Nodes[2];
                ShowSearchResults();
            }
        }

        private void ShowSearchResults()
        {
            List<FeedItem> list = new List<FeedItem>();
            string s = placeHolderTextBox1.Text;//.ToLower();
            if (s != "")
                foreach (var f in _feeditems)
                {
                    list.AddRange(f.Value.FindAll(x =>
                        x.Title.myContains(s) ||
                        x.Description.myContains(s)));
                }
            treeView1.Nodes[2].Text = "Search Results";
            if (list.Count > 0)
                treeView1.Nodes[2].Text = $"Search Results ({list.Count})";
            _currentFeed = null;

            list = list.OrderByDescending(x => x.date).ToList();

            ShowFeedList(list);
            Log($"{list.Count} items found.");
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            placeHolderTextBox1.SelectAll();
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
            // settings form here
            frmSettings f = new frmSettings();
            if (f.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText("configs\\settings.config", JSON.ToNiceJSON(new Settings(), jp));
                SetTheme();
                string cf = _feedTitle;
                LoadFeeds();
                _menuCheckBox.Checked = Settings.UseSytemProxy;
                if (cf != "")
                {
                    ShowFeedFromTitle(cf);
                    // focus feed in treeview
                    var n = treeView1.Nodes.Find(cf, true);
                    if (n.Length > 0)
                        treeView1.SelectedNode = n[0];
                }
                // redo web browser content in theme
                webBrowser1.Refresh(WebBrowserRefreshOption.Completely);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // about box
            AboutBox1 a = new AboutBox1();
            a.ShowDialog();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (Settings.MGFeatures && keyData == Keys.Escape)
            {
                MoveNextUnread();
                return true;
            }
            if (keyData == Keys.Space && placeHolderTextBox1.Focused == false)
            {
                MoveNextUnread();
                return true;
            }
            else if (keyData == (Keys.D | Keys.Control))
            {
                deleteItems();
                return true;
            }
            else if (keyData == (Keys.Down | Keys.Alt))
            {
                //MessageBox.Show("down");
                MoveNext();
                return true;
            }
            else if (keyData == (Keys.Up | Keys.Alt))
            {
                MovePrev();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MovePrev()
        {
            if (myListBox1.SelectedIndices.Count == 0)
                return;
            int selectedIndex = myListBox1.SelectedIndices[0];
            if (selectedIndex > 0)
            {
                selectedIndex--;
                myListBox1.SelectedIndices.Clear();
                myListBox1.SelectedIndex = selectedIndex;
                ShowItem(myListBox1.SelectedItem as FeedItem);
            }
        }

        private void MoveNext()
        {
            int selectedIndex = myListBox1.SelectedIndex;
            if (selectedIndex < myListBox1.Items.Count)
            {
                selectedIndex++;

                myListBox1.SelectedIndices.Clear();
                myListBox1.SelectedIndex = selectedIndex;
                myListBox1.EnsureVisible(_visibleItems);
                ShowItem(myListBox1.SelectedItem as FeedItem);
            }
        }

        private void logMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show log form
            frmLog f = new frmLog();
            f.Show();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myListBox1.SelectedItem == null)
                return;
            // toggle star
            var f = myListBox1.SelectedItem as FeedItem;
            string title = f.Title;

            // preprocess title
            if (File.Exists("configs\\search.plugin"))
            {
                try
                {
                    title = Compiler.CompileAndRun("configs\\search.plugin", new object[] { title });
                }
                catch (Exception ex) { Log("" + ex); }
            }

            if (f != null)
                Process.Start("www.google.com/search?q=" + title);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // clear search text
            placeHolderTextBox1.Text = "";
            placeHolderTextBox1.setPlaceholder();
            ShowSearchResults();
            button1.Visible = false;
        }

        private void placeHolderTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                e.SuppressKeyPress = true;
            if (button1.Visible == false)
                button1.Visible = true;
        }

        //private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    FontDialog fd = new FontDialog();
        //    fd.Font = this.Font;
        //    if (fd.ShowDialog() == DialogResult.OK)
        //    {
        //        this.Font = fd.Font;
        //    }
        //}

        private void cleanupToolStripMenuItem_Click_2(object sender, EventArgs e)
        {
            // feed cleanup old items
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Tag == null)
                return;

            var expr = new Predicate<FeedItem>(x =>
                    DateTime.Now.Subtract(x.date).TotalDays >= Settings.CleanupItemAfterDays
                    && x.isStarred == false
                    && x.isRead == true);

            var feed = treeView1.SelectedNode.Tag as Feed;
            var f = _feeditems[feed.Title];

            var list = f.FindAll(expr);
            int c = list.Count;

            // clear feed errors
            _feeds.ForEach(x => x.LastError = "");


            if (c == 0)
            {
                //MessageBox.Show
                Log("No items to remove.");
                return;
            }
            var r = MessageBox.Show($"Do you want to remove {c} items from {feed.Title}?", "Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (r == DialogResult.Yes)
            {
                List<string> imgs = new List<string>();
                list.ForEach(x =>
                {
                    foreach (var img in GetImagesInHTMLString(x.Description))
                    {
                        var s = _imghrefregex.Match(img).Groups["href"].Value;
                        var url = s.Replace(_localhostimageurl, "");
                        imgs.Add(url);
                    }
                });

                _imageCache.Remove(imgs);

                f.RemoveAll(expr);
                File.WriteAllText(GetFeedFilename(feed.Title), JSON.ToNiceJSON(f, jp));

                UpdateFeedCount();
                ShowFeedList(feed);
            }
        }

        private object _dflock = new object();
        private void downloadImagesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // download images for feed now
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Tag == null)
                return;
            lock (_dflock)
            {
                var feed = treeView1.SelectedNode.Tag as Feed;
                var f = _feeditems[feed.Title];

                var r = MessageBox.Show($"Do you want to download images for {feed.Title} now?", "Download Images", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.Yes)
                {
                    foreach (var i in f)
                    {
                        if (i.isRead == false)
                        {
                            internalDownloadImage(i, false);
                        }
                        Application.DoEvents();
                    }
                }
            }
        }
        #endregion

        private object _zlock = new object();
        private void compressImageCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (_zlock)
            {
                var r = MessageBox.Show($"Do you want to zip compress the image cache?", "Compress Images", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.No)
                    return;
                // compress image cache
                var dirs = Directory.GetDirectories("cache");
                toolProgressBar.Value = 0;
                toolProgressBar.Maximum = dirs.Length;
                foreach (var dir in dirs)
                {
                    toolProgressBar.Visible = true;
                    toolProgressBar.Value++;
                    RaptorDB.Common.ZIP.Compress(dir + ".zip", dir, false, Log);
                    foreach (var f in Directory.GetFiles(dir, "*.jpg"))
                        File.Delete(f);
                    Application.DoEvents();
                }
                _imageCache.ClearLookup();
                Log("Compress images done.");
                toolProgressBar.Visible = false;
            }
        }

        private void deleteItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteItems();
        }

        private void deleteItems()
        {
            // FIX : handle search list delete
            // delete item
            int count = myListBox1.SelectedItems.Count;
            if (count == 0)// == null)
                return;
            var feed = treeView1.SelectedNode.Tag as Feed;
            if (feed == null)
                return;
            if (count > 1)
            {
                var r = MessageBox.Show($"Do you want to delete {count} items now?\r\n\r\nStarred Items will be ignored, and you must unstar first. ", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.No)
                    return;
            }
            var list = _feeditems[feed.Title];
            int last = myListBox1.SelectedIndices[count-1] - count;
            foreach (FeedItem f in myListBox1.SelectedItems)
            {
                if (f.isStarred)
                    continue;
                List<string> imgs = new List<string>();
                foreach (var img in GetImagesInHTMLString(f.Description))
                {
                    var s = _imghrefregex.Match(img).Groups["href"].Value;
                    var url = s.Replace(_localhostimageurl, "");
                    imgs.Add(url);
                }
                _imageCache.Remove(imgs);
                list.Remove(f);
            }
            File.WriteAllText(GetFeedFilename(feed.Title), JSON.ToNiceJSON(list, jp));
            UpdateFeedCount();
            ShowFeedList(feed);
            last++;
            ShowItem(list[last]);
            myListBox1.SelectedIndices.Add(last);
            myListBox1.EnsureVisible(_visibleItems);
        }

        private void cleanupImageCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // cleanup image cache

            // get list of images -> folder, img name
            Dictionary<string, bool> imgs = new Dictionary<string, bool>();
            if (Directory.Exists("Cache") == false)
                return;

            foreach (var f in Directory.EnumerateFiles("Cache", "*.*", SearchOption.AllDirectories))
            {
                imgs.Add(f.ToLowerInvariant().Replace("cache\\", "").Replace("\\", "/"), false);
            }

            List<ImageCache.urlhash> im = new List<ImageCache.urlhash>();
            // foreach feeditem in feeds
            foreach (var feed in _feeditems)
            {
                foreach (var fi in feed.Value)
                {
                    //    extract images 
                    foreach (var img in GetImagesInHTMLString(fi.Description))
                    {
                        var s = _imghrefregex.Match(img).Groups["href"].Value;
                        var url = s.Replace(_localhostimageurl, "");
                        im.Add(_imageCache.GetUrlhash(url));
                    }
                }
            }
            foreach (var i in im)
            {
                var s = i.fn.ToLowerInvariant();
                if (imgs.ContainsKey(s))
                    imgs.Remove(s);
            }
            if (imgs.Count == 0)
            {
                Log("Image cache is clean.");
                return;
            }
            var r = MessageBox.Show($"Do you want to cleanup {imgs.Count} orphaned images from cache?", "Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (r == DialogResult.Yes)
            {
                // foreach img in above list -> delete img file
                foreach (var i in imgs)
                {
                    //if (i.Value == false)
                    File.Delete("Cache\\" + i.Key.Replace("/", "\\"));
                }
            }
        }
    }
}
