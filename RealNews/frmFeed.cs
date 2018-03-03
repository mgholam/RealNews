using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void button3_Click(object sender, EventArgs e)
        {
            ret = feed;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmFeed_Load(object sender, EventArgs e)
        {
            txtURL.Text = feed.URL;
            txtName.Text = feed.Title;
            chkRTL.Checked = feed.RTL;
            chkImages.Checked = feed.DownloadImages;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // fix : save button

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // fix : read feed header info
        }
    }
}
