namespace OPMedia.UI.ProTONE.Dialogs
{
    partial class OnlineContentBrowser
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
            this.opmTableLayoutPanel1 = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.opmLabel1 = new OPMedia.UI.Controls.OPMLabel();
            this.cmbSearch = new System.Windows.Forms.ComboBox();
            this.btnSearch = new OPMedia.UI.Controls.OPMButton();
            this.tabContentBrowser = new OPMedia.UI.Controls.OPMTabControl();
            this.tpLocalDatabase = new System.Windows.Forms.TabPage();
            this.localDbBrowser = new OPMedia.UI.ProTONE.Controls.OnlineMediaBrowser.LocalDatabaseBrowserCtl();
            this.tpShoutcastDir = new System.Windows.Forms.TabPage();
            this.shoutcastDirBrowser = new OPMedia.UI.ProTONE.Controls.OnlineMediaBrowser.ShoutcastDirectoryBrowserCtl();
            this.tpDeezerContent = new System.Windows.Forms.TabPage();
            this.deezerTrackBrowser = new OPMedia.UI.ProTONE.Controls.OnlineMediaBrowser.DeezerTrackBrowserCtl();
            this.pnlContent.SuspendLayout();
            this.opmTableLayoutPanel1.SuspendLayout();
            this.tabContentBrowser.SuspendLayout();
            this.tpLocalDatabase.SuspendLayout();
            this.tpShoutcastDir.SuspendLayout();
            this.tpDeezerContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.opmTableLayoutPanel1);
            // 
            // opmTableLayoutPanel1
            // 
            this.opmTableLayoutPanel1.AutoSize = true;
            this.opmTableLayoutPanel1.ColumnCount = 3;
            this.opmTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.opmTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.opmTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.opmTableLayoutPanel1.Controls.Add(this.opmLabel1, 0, 3);
            this.opmTableLayoutPanel1.Controls.Add(this.cmbSearch, 1, 3);
            this.opmTableLayoutPanel1.Controls.Add(this.btnSearch, 2, 3);
            this.opmTableLayoutPanel1.Controls.Add(this.tabContentBrowser, 0, 2);
            this.opmTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opmTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.opmTableLayoutPanel1.Name = "opmTableLayoutPanel1";
            this.opmTableLayoutPanel1.OverrideBackColor = System.Drawing.Color.Empty;
            this.opmTableLayoutPanel1.RowCount = 4;
            this.opmTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.opmTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmTableLayoutPanel1.Size = new System.Drawing.Size(945, 652);
            this.opmTableLayoutPanel1.TabIndex = 0;
            // 
            // opmLabel1
            // 
            this.opmLabel1.AutoSize = true;
            this.opmLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opmLabel1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.opmLabel1.FontSize = OPMedia.UI.Themes.FontSizes.NormalBold;
            this.opmLabel1.Location = new System.Drawing.Point(3, 621);
            this.opmLabel1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.opmLabel1.Name = "opmLabel1";
            this.opmLabel1.OverrideBackColor = System.Drawing.Color.Empty;
            this.opmLabel1.OverrideForeColor = System.Drawing.Color.Empty;
            this.opmLabel1.Size = new System.Drawing.Size(82, 31);
            this.opmLabel1.TabIndex = 0;
            this.opmLabel1.Text = "TXT_SEARCH:";
            this.opmLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbSearch
            // 
            this.cmbSearch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbSearch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbSearch.Location = new System.Drawing.Point(85, 625);
            this.cmbSearch.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.cmbSearch.MaximumSize = new System.Drawing.Size(2000, 0);
            this.cmbSearch.MaxLength = 256;
            this.cmbSearch.MinimumSize = new System.Drawing.Size(20, 0);
            this.cmbSearch.Name = "cmbSearch";
            this.cmbSearch.Size = new System.Drawing.Size(754, 21);
            this.cmbSearch.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.AutoSize = true;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Location = new System.Drawing.Point(842, 621);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.OverrideBackColor = System.Drawing.Color.Empty;
            this.btnSearch.OverrideForeColor = System.Drawing.Color.Empty;
            this.btnSearch.ShowDropDown = false;
            this.btnSearch.Size = new System.Drawing.Size(100, 27);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "TXT_SEARCH";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // tabContentBrowser
            // 
            this.opmTableLayoutPanel1.SetColumnSpan(this.tabContentBrowser, 3);
            this.tabContentBrowser.Controls.Add(this.tpLocalDatabase);
            this.tabContentBrowser.Controls.Add(this.tpShoutcastDir);
            this.tabContentBrowser.Controls.Add(this.tpDeezerContent);
            this.tabContentBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabContentBrowser.InnerPadding = new System.Windows.Forms.Padding(0);
            this.tabContentBrowser.ItemSize = new System.Drawing.Size(106, 55);
            this.tabContentBrowser.Location = new System.Drawing.Point(3, 5);
            this.tabContentBrowser.Margin = new System.Windows.Forms.Padding(3, 5, 1, 0);
            this.tabContentBrowser.Name = "tabContentBrowser";
            this.tabContentBrowser.SelectedIndex = 0;
            this.tabContentBrowser.Size = new System.Drawing.Size(941, 616);
            this.tabContentBrowser.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabContentBrowser.TabIndex = 3;
            this.tabContentBrowser.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // tpLocalDatabase
            // 
            this.tpLocalDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.tpLocalDatabase.Controls.Add(this.localDbBrowser);
            this.tpLocalDatabase.Location = new System.Drawing.Point(4, 59);
            this.tpLocalDatabase.Margin = new System.Windows.Forms.Padding(0);
            this.tpLocalDatabase.Name = "tpLocalDatabase";
            this.tpLocalDatabase.Size = new System.Drawing.Size(933, 553);
            this.tpLocalDatabase.TabIndex = 0;
            this.tpLocalDatabase.Text = "TXT_LOCAL_DB";
            // 
            // localDbBrowser
            // 
            this.localDbBrowser.AutoSize = true;
            this.localDbBrowser.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.localDbBrowser.ContextMenuStrip = null;
            this.localDbBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.localDbBrowser.FontSize = OPMedia.UI.Themes.FontSizes.Normal;
            this.localDbBrowser.Location = new System.Drawing.Point(0, 0);
            this.localDbBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.localDbBrowser.Name = "localDbBrowser";
            this.localDbBrowser.OverrideBackColor = System.Drawing.Color.Empty;
            this.localDbBrowser.Size = new System.Drawing.Size(933, 553);
            this.localDbBrowser.TabIndex = 0;
            // 
            // tpShoutcastDir
            // 
            this.tpShoutcastDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.tpShoutcastDir.Controls.Add(this.shoutcastDirBrowser);
            this.tpShoutcastDir.Location = new System.Drawing.Point(4, 59);
            this.tpShoutcastDir.Margin = new System.Windows.Forms.Padding(0);
            this.tpShoutcastDir.Name = "tpShoutcastDir";
            this.tpShoutcastDir.Size = new System.Drawing.Size(933, 553);
            this.tpShoutcastDir.TabIndex = 1;
            this.tpShoutcastDir.Text = "TXT_SHOUTCAST_DIR";
            // 
            // shoutcastDirBrowser
            // 
            this.shoutcastDirBrowser.ContextMenuStrip = null;
            this.shoutcastDirBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shoutcastDirBrowser.FontSize = OPMedia.UI.Themes.FontSizes.Normal;
            this.shoutcastDirBrowser.Location = new System.Drawing.Point(0, 0);
            this.shoutcastDirBrowser.Name = "shoutcastDirBrowser";
            this.shoutcastDirBrowser.OverrideBackColor = System.Drawing.Color.Empty;
            this.shoutcastDirBrowser.Size = new System.Drawing.Size(933, 553);
            this.shoutcastDirBrowser.TabIndex = 0;
            // 
            // tpDeezerContent
            // 
            this.tpDeezerContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.tpDeezerContent.Controls.Add(this.deezerTrackBrowser);
            this.tpDeezerContent.Location = new System.Drawing.Point(4, 59);
            this.tpDeezerContent.Margin = new System.Windows.Forms.Padding(0);
            this.tpDeezerContent.Name = "tpDeezerContent";
            this.tpDeezerContent.Size = new System.Drawing.Size(933, 553);
            this.tpDeezerContent.TabIndex = 2;
            this.tpDeezerContent.Text = "TXT_DEEZER_CONTENT";
            // 
            // deezerTrackBrowser
            // 
            this.deezerTrackBrowser.ContextMenuStrip = null;
            this.deezerTrackBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deezerTrackBrowser.FontSize = OPMedia.UI.Themes.FontSizes.Normal;
            this.deezerTrackBrowser.Location = new System.Drawing.Point(0, 0);
            this.deezerTrackBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.deezerTrackBrowser.Name = "deezerTrackBrowser";
            this.deezerTrackBrowser.OverrideBackColor = System.Drawing.Color.Empty;
            this.deezerTrackBrowser.Size = new System.Drawing.Size(933, 553);
            this.deezerTrackBrowser.TabIndex = 0;
            // 
            // OnlineContentBrowser
            // 
            this.AcceptButton = this.btnSearch;
            this.ClientSize = new System.Drawing.Size(947, 676);
            this.MinimumSize = new System.Drawing.Size(200, 85);
            this.Name = "OnlineContentBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.opmTableLayoutPanel1.ResumeLayout(false);
            this.opmTableLayoutPanel1.PerformLayout();
            this.tabContentBrowser.ResumeLayout(false);
            this.tpLocalDatabase.ResumeLayout(false);
            this.tpLocalDatabase.PerformLayout();
            this.tpShoutcastDir.ResumeLayout(false);
            this.tpDeezerContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private UI.Controls.OPMTableLayoutPanel opmTableLayoutPanel1;
        private UI.Controls.OPMLabel opmLabel1;
        private System.Windows.Forms.ComboBox cmbSearch;
        private UI.Controls.OPMButton btnSearch;
        private UI.Controls.OPMTabControl tabContentBrowser;
        private System.Windows.Forms.TabPage tpLocalDatabase;
        private System.Windows.Forms.TabPage tpShoutcastDir;
        private System.Windows.Forms.TabPage tpDeezerContent;
        private Controls.OnlineMediaBrowser.LocalDatabaseBrowserCtl localDbBrowser;
        private Controls.OnlineMediaBrowser.ShoutcastDirectoryBrowserCtl shoutcastDirBrowser;
        private Controls.OnlineMediaBrowser.DeezerTrackBrowserCtl deezerTrackBrowser;
    }
}