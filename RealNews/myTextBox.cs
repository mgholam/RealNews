using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RealNews
{
    public partial class myTextBox : UserControl
    {
        public myTextBox()
        {
            InitializeComponent();
        }

        public new string Text { get { return placeHolderTextBox1.Text; } set { placeHolderTextBox1.Text = value; } }

        private void button1_Click(object sender, EventArgs e)
        {
            placeHolderTextBox1.Text = "";
            placeHolderTextBox1.setPlaceholder();

        }

        private void myTextBox_Load(object sender, EventArgs e)
        {

        }
    }
}
