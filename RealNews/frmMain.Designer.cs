namespace RealNews
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.feedContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.markAsReadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadImagesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.updateNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.editFeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.cleanupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rssImages = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.placeHolderTextBox1 = new PlaceHolderTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.itemContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toggleStarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markUnreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewFeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importOPMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanupToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logMessagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressImageCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.starToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.feedContextMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.itemContextMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.trayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1021, 587);
            this.splitContainer1.SplitterDistance = 340;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.White;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.ContextMenuStrip = this.feedContextMenu;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ForeColor = System.Drawing.Color.Black;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.rssImages;
            this.treeView1.Indent = 23;
            this.treeView1.ItemHeight = 20;
            this.treeView1.Location = new System.Drawing.Point(0, 29);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowLines = false;
            this.treeView1.Size = new System.Drawing.Size(340, 558);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // feedContextMenu
            // 
            this.feedContextMenu.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.feedContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.markAsReadToolStripMenuItem,
            this.downloadImagesToolStripMenuItem1,
            this.toolStripMenuItem1,
            this.updateNowToolStripMenuItem,
            this.toolStripMenuItem2,
            this.editFeedToolStripMenuItem,
            this.toolStripMenuItem4,
            this.cleanupToolStripMenuItem});
            this.feedContextMenu.Name = "contextMenuStrip1";
            this.feedContextMenu.Size = new System.Drawing.Size(172, 132);
            // 
            // markAsReadToolStripMenuItem
            // 
            this.markAsReadToolStripMenuItem.Image = global::RealNews.Properties.Resources.Eye;
            this.markAsReadToolStripMenuItem.Name = "markAsReadToolStripMenuItem";
            this.markAsReadToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.markAsReadToolStripMenuItem.Text = "Mark as read";
            this.markAsReadToolStripMenuItem.Click += new System.EventHandler(this.markAsReadToolStripMenuItem_Click);
            // 
            // downloadImagesToolStripMenuItem1
            // 
            this.downloadImagesToolStripMenuItem1.Image = global::RealNews.Properties.Resources.Image1;
            this.downloadImagesToolStripMenuItem1.Name = "downloadImagesToolStripMenuItem1";
            this.downloadImagesToolStripMenuItem1.Size = new System.Drawing.Size(171, 22);
            this.downloadImagesToolStripMenuItem1.Text = "Download Images";
            this.downloadImagesToolStripMenuItem1.Click += new System.EventHandler(this.downloadImagesToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(168, 6);
            // 
            // updateNowToolStripMenuItem
            // 
            this.updateNowToolStripMenuItem.Image = global::RealNews.Properties.Resources.refresh;
            this.updateNowToolStripMenuItem.Name = "updateNowToolStripMenuItem";
            this.updateNowToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.updateNowToolStripMenuItem.Text = "Update now";
            this.updateNowToolStripMenuItem.Click += new System.EventHandler(this.updateNowToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(168, 6);
            // 
            // editFeedToolStripMenuItem
            // 
            this.editFeedToolStripMenuItem.Image = global::RealNews.Properties.Resources.Rss2;
            this.editFeedToolStripMenuItem.Name = "editFeedToolStripMenuItem";
            this.editFeedToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.editFeedToolStripMenuItem.Text = "Edit Feed";
            this.editFeedToolStripMenuItem.Click += new System.EventHandler(this.editFeedToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(168, 6);
            // 
            // cleanupToolStripMenuItem
            // 
            this.cleanupToolStripMenuItem.Image = global::RealNews.Properties.Resources.TrashO;
            this.cleanupToolStripMenuItem.Name = "cleanupToolStripMenuItem";
            this.cleanupToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.cleanupToolStripMenuItem.Text = "Cleanup";
            this.cleanupToolStripMenuItem.Click += new System.EventHandler(this.cleanupToolStripMenuItem_Click_2);
            // 
            // rssImages
            // 
            this.rssImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.rssImages.ImageSize = new System.Drawing.Size(16, 16);
            this.rssImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.placeHolderTextBox1);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(340, 29);
            this.panel1.TabIndex = 1;
            // 
            // placeHolderTextBox1
            // 
            this.placeHolderTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.placeHolderTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.placeHolderTextBox1.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Italic);
            this.placeHolderTextBox1.ForeColor = System.Drawing.Color.Gray;
            this.placeHolderTextBox1.Location = new System.Drawing.Point(0, 0);
            this.placeHolderTextBox1.Margin = new System.Windows.Forms.Padding(0);
            this.placeHolderTextBox1.Multiline = true;
            this.placeHolderTextBox1.Name = "placeHolderTextBox1";
            this.placeHolderTextBox1.PlaceHolderText = "Search...";
            this.placeHolderTextBox1.Size = new System.Drawing.Size(312, 27);
            this.placeHolderTextBox1.TabIndex = 1;
            this.placeHolderTextBox1.Text = "Search...";
            this.placeHolderTextBox1.Enter += new System.EventHandler(this.txtSearch_Enter);
            this.placeHolderTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.placeHolderTextBox1_KeyDown);
            this.placeHolderTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(312, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.webBrowser1);
            this.splitContainer2.Size = new System.Drawing.Size(678, 587);
            this.splitContainer2.SplitterDistance = 225;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer2_SplitterMoved);
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.White;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.ContextMenuStrip = this.itemContextMenu;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.ForeColor = System.Drawing.Color.Black;
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(223, 585);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Click += new System.EventHandler(this.listView1_Click);
            this.listView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Title";
            // 
            // itemContextMenu
            // 
            this.itemContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleStarToolStripMenuItem,
            this.markUnreadToolStripMenuItem,
            this.downloadImagesToolStripMenuItem});
            this.itemContextMenu.Name = "contextMenuStrip2";
            this.itemContextMenu.Size = new System.Drawing.Size(170, 70);
            // 
            // toggleStarToolStripMenuItem
            // 
            this.toggleStarToolStripMenuItem.Image = global::RealNews.Properties.Resources.Star;
            this.toggleStarToolStripMenuItem.Name = "toggleStarToolStripMenuItem";
            this.toggleStarToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.toggleStarToolStripMenuItem.Text = "Toggle Star";
            this.toggleStarToolStripMenuItem.Click += new System.EventHandler(this.toggleStarToolStripMenuItem_Click);
            // 
            // markUnreadToolStripMenuItem
            // 
            this.markUnreadToolStripMenuItem.Image = global::RealNews.Properties.Resources.EyeSlash;
            this.markUnreadToolStripMenuItem.Name = "markUnreadToolStripMenuItem";
            this.markUnreadToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.markUnreadToolStripMenuItem.Text = "Mark Unread";
            this.markUnreadToolStripMenuItem.Click += new System.EventHandler(this.markUnreadToolStripMenuItem_Click);
            // 
            // downloadImagesToolStripMenuItem
            // 
            this.downloadImagesToolStripMenuItem.Image = global::RealNews.Properties.Resources.Image1;
            this.downloadImagesToolStripMenuItem.Name = "downloadImagesToolStripMenuItem";
            this.downloadImagesToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.downloadImagesToolStripMenuItem.Text = "Download Images";
            this.downloadImagesToolStripMenuItem.Click += new System.EventHandler(this.downloadImagesToolStripMenuItem_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(23, 25);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(446, 585);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser1_Navigating);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.nextToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.updateAllToolStripMenuItem,
            this.starToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.ShowItemToolTips = true;
            this.menuStrip1.Size = new System.Drawing.Size(1021, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewFeedToolStripMenuItem,
            this.importOPMLToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // addNewFeedToolStripMenuItem
            // 
            this.addNewFeedToolStripMenuItem.Image = global::RealNews.Properties.Resources.Rss2;
            this.addNewFeedToolStripMenuItem.Name = "addNewFeedToolStripMenuItem";
            this.addNewFeedToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.addNewFeedToolStripMenuItem.Text = "Add New Feed";
            this.addNewFeedToolStripMenuItem.Click += new System.EventHandler(this.addNewFeedToolStripMenuItem_Click);
            // 
            // importOPMLToolStripMenuItem
            // 
            this.importOPMLToolStripMenuItem.Name = "importOPMLToolStripMenuItem";
            this.importOPMLToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.importOPMLToolStripMenuItem.Text = "Import OPML";
            this.importOPMLToolStripMenuItem.Click += new System.EventHandler(this.importOPMLToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::RealNews.Properties.Resources.PowerOff;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // nextToolStripMenuItem
            // 
            this.nextToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.nextToolStripMenuItem.Image = global::RealNews.Properties.Resources.next;
            this.nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            this.nextToolStripMenuItem.Size = new System.Drawing.Size(104, 20);
            this.nextToolStripMenuItem.Text = "Next Unread";
            this.nextToolStripMenuItem.ToolTipText = "Press (space)";
            this.nextToolStripMenuItem.Click += new System.EventHandler(this.nextToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cleanupToolStripMenuItem1,
            this.settingsToolStripMenuItem,
            this.logMessagesToolStripMenuItem,
            this.fontToolStripMenuItem,
            this.compressImageCacheToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // cleanupToolStripMenuItem1
            // 
            this.cleanupToolStripMenuItem1.Image = global::RealNews.Properties.Resources.TrashO;
            this.cleanupToolStripMenuItem1.Name = "cleanupToolStripMenuItem1";
            this.cleanupToolStripMenuItem1.Size = new System.Drawing.Size(201, 22);
            this.cleanupToolStripMenuItem1.Text = "Cleanup";
            this.cleanupToolStripMenuItem1.Click += new System.EventHandler(this.cleanupToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::RealNews.Properties.Resources.Cog;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // logMessagesToolStripMenuItem
            // 
            this.logMessagesToolStripMenuItem.Image = global::RealNews.Properties.Resources.FileTextO;
            this.logMessagesToolStripMenuItem.Name = "logMessagesToolStripMenuItem";
            this.logMessagesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.logMessagesToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.logMessagesToolStripMenuItem.Text = "Log messages";
            this.logMessagesToolStripMenuItem.Click += new System.EventHandler(this.logMessagesToolStripMenuItem_Click);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fontToolStripMenuItem.Text = "font";
            this.fontToolStripMenuItem.Visible = false;
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // compressImageCacheToolStripMenuItem
            // 
            this.compressImageCacheToolStripMenuItem.Image = global::RealNews.Properties.Resources.Compress;
            this.compressImageCacheToolStripMenuItem.Name = "compressImageCacheToolStripMenuItem";
            this.compressImageCacheToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.compressImageCacheToolStripMenuItem.Text = "Compress Image Cache";
            this.compressImageCacheToolStripMenuItem.Click += new System.EventHandler(this.compressImageCacheToolStripMenuItem_Click);
            // 
            // updateAllToolStripMenuItem
            // 
            this.updateAllToolStripMenuItem.Image = global::RealNews.Properties.Resources.refresh;
            this.updateAllToolStripMenuItem.Name = "updateAllToolStripMenuItem";
            this.updateAllToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.updateAllToolStripMenuItem.Text = "Update All";
            this.updateAllToolStripMenuItem.Click += new System.EventHandler(this.updateAllToolStripMenuItem_Click);
            // 
            // starToolStripMenuItem
            // 
            this.starToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.starToolStripMenuItem.Image = global::RealNews.Properties.Resources.Star;
            this.starToolStripMenuItem.Name = "starToolStripMenuItem";
            this.starToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.starToolStripMenuItem.Size = new System.Drawing.Size(99, 20);
            this.starToolStripMenuItem.Text = "Toggle Star";
            this.starToolStripMenuItem.ToolTipText = "Press (ctrl+S)";
            this.starToolStripMenuItem.Click += new System.EventHandler(this.starToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.editToolStripMenuItem.Image = global::RealNews.Properties.Resources.Edit;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolProgressBar,
            this.toolCount,
            this.toolMessage});
            this.statusStrip1.Location = new System.Drawing.Point(0, 611);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1021, 23);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolProgressBar
            // 
            this.toolProgressBar.Name = "toolProgressBar";
            this.toolProgressBar.Size = new System.Drawing.Size(100, 17);
            this.toolProgressBar.Visible = false;
            // 
            // toolCount
            // 
            this.toolCount.Name = "toolCount";
            this.toolCount.Size = new System.Drawing.Size(13, 18);
            this.toolCount.Text = " ";
            // 
            // toolMessage
            // 
            this.toolMessage.Name = "toolMessage";
            this.toolMessage.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolMessage.Size = new System.Drawing.Size(860, 18);
            this.toolMessage.Spring = true;
            this.toolMessage.Text = "loading...";
            this.toolMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.trayMenu;
            this.notifyIcon1.Text = "Real News";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // trayMenu
            // 
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem1});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.Size = new System.Drawing.Size(114, 54);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(110, 6);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Image = global::RealNews.Properties.Resources.PowerOff;
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(113, 22);
            this.exitToolStripMenuItem1.Text = "Exit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1021, 634);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "Real News";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.feedContextMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.itemContextMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.trayMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolMessage;
        private System.Windows.Forms.ToolStripMenuItem updateAllToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip feedContextMenu;
        private System.Windows.Forms.ToolStripMenuItem markAsReadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem updateNowToolStripMenuItem;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ToolStripMenuItem starToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewFeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importOPMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editFeedToolStripMenuItem;
        private System.Windows.Forms.ImageList rssImages;
        private System.Windows.Forms.ContextMenuStrip itemContextMenu;
        private System.Windows.Forms.ToolStripMenuItem toggleStarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem markUnreadToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolCount;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem downloadImagesToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel1;
        //private myTextBox txtSearch;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanupToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logMessagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private PlaceHolderTextBox placeHolderTextBox1;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cleanupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadImagesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem compressImageCacheToolStripMenuItem;
    }
}

