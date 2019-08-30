using CodeHollow.FeedReader;
using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace RealNews
{
    public partial class frmFeed : Form
    {
        public frmFeed()
        {
            InitializeComponent();
        }

        public Feed feed;
        public Feed ret;
        bool _isdirty = false;

        private void button3_Click(object sender, EventArgs e)
        {
            cancelclose();
        }

        private void cancelclose()
        {
            ret = feed;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmFeed_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (feed == null)
                feed = new Feed();

            txtURL.Text = feed.URL;
            txtName.Text = feed.Title;
            txtFolder.Text = feed.Folder;
            lblLastError.Text = feed.LastError;
            numUpdate.Value = feed.UpdateEveryMin <0?0:feed.UpdateEveryMin;
            chkRTL.Checked = feed.RTL;
            chkImages.Checked = feed.DownloadImages;
            chkExcludeCleanup.Checked = feed.ExcludeInCleanup;
            _isdirty = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var s = txtFolder.Text.ToLower();

            if(txtName.Text.Trim() == "")
            {
                MessageBox.Show("Please enter a name for the feed");
                txtName.Focus();
                return;
            }
            if(s == "unread" || s== "starred" || s== "search results")
            {
                MessageBox.Show("Category can not be one of : \r\n\r\n Unread, Starred, Search Results");
                txtFolder.Focus();
                return;
            }
            if(s.Contains("\\") || s.Contains("/"))
            {
                MessageBox.Show("Sub folders are not supported yet");
                txtFolder.Focus();
                return;
            }
            // save button
            ret = new Feed
            {
                Title = txtName.Text,
                URL = txtURL.Text,
                DownloadImages = chkImages.Checked,
                RTL = chkRTL.Checked,
                UpdateEveryMin = (int)numUpdate.Value,
                ExcludeInCleanup = chkExcludeCleanup.Checked,
                Folder = txtFolder.Text.Replace("\\", "/")
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // read feed header info
            txtName.Text = GetInfo(txtURL.Text);
        }

        private string GetInfo(string url)
        {
            try
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls  | SecurityProtocolType.Ssl3;
                mWebClient wc = new mWebClient();
                
                var feedxml = wc.DownloadString(url);
                var reader = FeedReader.ReadFromString(feedxml);
                return reader.Title;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return "";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ret = null;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmFeed_KeyUp(object sender, KeyEventArgs e)
        {
            if (_isdirty == false && e.KeyCode == Keys.Escape)
                cancelclose();
        }

        private void txtURL_TextChanged(object sender, EventArgs e)
        {
            _isdirty = true;
        }
    }
}
