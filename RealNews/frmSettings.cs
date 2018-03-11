using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealNews
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // setup form
            chkUseSystemProxy.Checked = Settings.UseSytemProxy;
            numUpdate.Value = Settings.GlobalUpdateEveryMin;
            numWebPort.Value = Settings.webport;
            numDownloadSize.Value = Settings.DownloadImagesUnderKB;
            txtStart.Text = Settings.StartDownloadImgTime;
            txtEnd.Text = Settings.EndDownloadImgTime;
            lblLastUpdated.Text = Settings.LastUpdateTime.ToLongTimeString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.UseSytemProxy = chkUseSystemProxy.Checked;
                Settings.GlobalUpdateEveryMin = (int)numUpdate.Value;
                Settings.webport = (int)numWebPort.Value;
                Settings.DownloadImagesUnderKB = (int)numDownloadSize.Value;

                var st = TimeSpan.Parse(txtStart.Text);
                var ed = TimeSpan.Parse(txtEnd.Text);
                // validate start end times
                if (st < ed)
                {
                    Settings.StartDownloadImgTime = txtStart.Text;
                    Settings.EndDownloadImgTime = txtEnd.Text;

                    DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                    MessageBox.Show("Start Time must be less than End Time");
            }
            catch
            {
                MessageBox.Show("Invalid settings");
            }
        }
    }
}
