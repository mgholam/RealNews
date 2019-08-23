namespace RealNews
{
    partial class frmFeed
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkExcludeCleanup = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.lblLastError = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numUpdate = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkImages = new System.Windows.Forms.CheckBox();
            this.chkRTL = new System.Windows.Forms.CheckBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpdate)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Feed Address";
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(101, 7);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(501, 22);
            this.txtURL.TabIndex = 0;
            this.txtURL.TextChanged += new System.EventHandler(this.txtURL_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(613, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 22);
            this.button1.TabIndex = 1;
            this.button1.Text = "get info";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 217);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 33);
            this.button2.TabIndex = 9;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(583, 217);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(90, 33);
            this.button3.TabIndex = 10;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtFolder);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.chkExcludeCleanup);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.lblLastError);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numUpdate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.chkImages);
            this.groupBox1.Controls.Add(this.chkRTL);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(674, 176);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(95, 54);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(501, 22);
            this.txtFolder.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(33, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 14);
            this.label6.TabIndex = 19;
            this.label6.Text = "Category";
            // 
            // chkExcludeCleanup
            // 
            this.chkExcludeCleanup.AutoSize = true;
            this.chkExcludeCleanup.Location = new System.Drawing.Point(370, 115);
            this.chkExcludeCleanup.Name = "chkExcludeCleanup";
            this.chkExcludeCleanup.Size = new System.Drawing.Size(164, 18);
            this.chkExcludeCleanup.TabIndex = 7;
            this.chkExcludeCleanup.Text = "Exclude in Global Cleanup";
            this.chkExcludeCleanup.UseVisualStyleBackColor = true;
            this.chkExcludeCleanup.CheckedChanged += new System.EventHandler(this.txtURL_TextChanged);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(577, 107);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(90, 33);
            this.button4.TabIndex = 8;
            this.button4.Text = "Delete Feed";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // lblLastError
            // 
            this.lblLastError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblLastError.Location = new System.Drawing.Point(90, 139);
            this.lblLastError.Name = "lblLastError";
            this.lblLastError.Size = new System.Drawing.Size(572, 25);
            this.lblLastError.TabIndex = 17;
            this.lblLastError.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 14);
            this.label5.TabIndex = 16;
            this.label5.Text = "Last Error";
            // 
            // numUpdate
            // 
            this.numUpdate.Location = new System.Drawing.Point(95, 84);
            this.numUpdate.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.numUpdate.Name = "numUpdate";
            this.numUpdate.Size = new System.Drawing.Size(123, 22);
            this.numUpdate.TabIndex = 4;
            this.numUpdate.ValueChanged += new System.EventHandler(this.txtURL_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(221, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(169, 14);
            this.label4.TabIndex = 14;
            this.label4.Text = "minutes ( 0 = global default )";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 14);
            this.label3.TabIndex = 12;
            this.label3.Text = "Update Every";
            // 
            // chkImages
            // 
            this.chkImages.AutoSize = true;
            this.chkImages.Location = new System.Drawing.Point(95, 115);
            this.chkImages.Name = "chkImages";
            this.chkImages.Size = new System.Drawing.Size(123, 18);
            this.chkImages.TabIndex = 5;
            this.chkImages.Text = "Download Images";
            this.chkImages.UseVisualStyleBackColor = true;
            this.chkImages.CheckedChanged += new System.EventHandler(this.txtURL_TextChanged);
            // 
            // chkRTL
            // 
            this.chkRTL.AutoSize = true;
            this.chkRTL.Location = new System.Drawing.Point(250, 115);
            this.chkRTL.Name = "chkRTL";
            this.chkRTL.Size = new System.Drawing.Size(96, 18);
            this.chkRTL.TabIndex = 6;
            this.chkRTL.Text = "Right to Left";
            this.chkRTL.UseVisualStyleBackColor = true;
            this.chkRTL.CheckedChanged += new System.EventHandler(this.txtURL_TextChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(95, 21);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(501, 22);
            this.txtName.TabIndex = 2;
            this.txtName.TextChanged += new System.EventHandler(this.txtURL_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 14);
            this.label2.TabIndex = 8;
            this.label2.Text = "Name";
            // 
            // frmFeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 256);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtURL);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "frmFeed";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmFeed";
            this.Load += new System.EventHandler(this.frmFeed_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmFeed_KeyUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpdate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkRTL;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkImages;
        private System.Windows.Forms.NumericUpDown numUpdate;
        private System.Windows.Forms.Label lblLastError;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox chkExcludeCleanup;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Label label6;
    }
}