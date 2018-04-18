using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RealNews
{
    public partial class frmLog : Form
    {
        public frmLog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            refreshitems();
        }

        private void refreshitems()
        {
            listBox1.BeginUpdate();
            listBox1.Items.Clear();

            listBox1.Items.AddRange(ConsoleLogger.Instance.GetLastLogs());

            listBox1.EndUpdate();
        }

        private void frmLog_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            refreshitems();
        }

        private void frmLog_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
                refreshitems();
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
