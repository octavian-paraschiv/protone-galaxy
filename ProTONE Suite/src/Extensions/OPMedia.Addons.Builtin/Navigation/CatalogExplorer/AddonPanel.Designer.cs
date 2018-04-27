using OPMedia.UI.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace OPMedia.Addons.Builtin.CatalogExplorer
{
    partial class AddonPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddonPanel));
            this.tss2 = new OPMedia.UI.Controls.OPMToolStripSeparator();
            this.tss3 = new OPMedia.UI.Controls.OPMToolStripSeparator();
            this.tsbCatalog = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.toolStripMain = new OPMedia.UI.Controls.OPMToolStrip();
            this.tsbNew = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.tsbOpen = new OPMedia.UI.Controls.OPMToolStripSplitButton();
            this.tsbSave = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.tsbSaveAs = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.tss1 = new OPMedia.UI.Controls.OPMToolStripSeparator();
            this.tsbBack = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.tsbForward = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.tsbUpLevel = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.tsbSearch = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.tsbReload = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.tsbRename = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.tsbDelete = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.tssMergeCatalogs = new OPMedia.UI.Controls.OPMToolStripSeparator();
            this.tsbMergeCatalogs = new OPMedia.UI.Controls.OPMTriStateToolStripButton();
            this.contextMenuStrip = new OPMedia.UI.Controls.OPMContextMenuStrip();
            this.tsmiBack = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiFwd = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiUp = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiSep1 = new OPMedia.UI.Controls.OPMMenuStripSeparator();
            this.tsmiSearch = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiReload = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiSep2 = new OPMedia.UI.Controls.OPMMenuStripSeparator();
            this.tsmiCopy = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiCut = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiPaste = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiCopyPaste = new OPMedia.UI.Controls.OPMToolStripSeparator();
            this.tsmiRename = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiDelete = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiSep4 = new OPMedia.UI.Controls.OPMMenuStripSeparator();
            this.tsmiCatalog = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiSepProTONE = new OPMedia.UI.Controls.OPMMenuStripSeparator();
            this.tsmiProTONEPlay = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tsmiProTONEEnqueue = new OPMedia.UI.Controls.OPMToolStripMenuItem();
            this.tableLayoutPanel1 = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.pnlSplitter = new OPMedia.UI.Controls.OPMSplitContainer();
            this.tvCatalog = new OPMedia.Addons.Builtin.CatalogExplorer.Controls.CatalogTreeView();
            this.lvCatalogFolder = new OPMedia.Addons.Builtin.CatalogExplorer.Controls.CatalogListView();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMain.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplitter)).BeginInit();
            this.pnlSplitter.Panel1.SuspendLayout();
            this.pnlSplitter.Panel2.SuspendLayout();
            this.pnlSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // tss2
            // 
            this.tss2.AutoSize = false;
            this.tss2.Name = "tss2";
            this.tss2.Size = new System.Drawing.Size(6, 53);
            // 
            // tss3
            // 
            this.tss3.AutoSize = false;
            this.tss3.Name = "tss3";
            this.tss3.Size = new System.Drawing.Size(6, 53);
            // 
            // tsbCatalog
            // 
            this.tsbCatalog.ActiveImage = null;
            this.tsbCatalog.AutoSize = false;
            this.tsbCatalog.CheckedImage = null;
            this.tsbCatalog.DisabledImage = null;
            this.tsbCatalog.Image = null;
            this.tsbCatalog.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbCatalog.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbCatalog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCatalog.InactiveImage = null;
            this.tsbCatalog.Name = "tsbCatalog";
            this.tsbCatalog.Size = new System.Drawing.Size(40, 40);
            this.tsbCatalog.Tag = "ToolActionCatalog";
            this.tsbCatalog.Text = "TXT_CATALOG";
            this.tsbCatalog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbCatalog.ToolTipText = "Add selection to catalog";
            this.tsbCatalog.Click += new System.EventHandler(this.OnToolAction);
            // 
            // toolStripMain
            // 
            this.toolStripMain.AllowMerge = false;
            this.toolStripMain.AutoSize = false;
            this.toolStripMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.toolStripMain.CanOverflow = false;
            this.toolStripMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripMain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbNew,
            this.tsbOpen,
            this.tsbSave,
            this.tsbSaveAs,
            this.tss1,
            this.tsbBack,
            this.tsbForward,
            this.tsbUpLevel,
            this.tss2,
            this.tsbSearch,
            this.tsbReload,
            this.tss3,
            this.tsbRename,
            this.tsbDelete,
            this.tssMergeCatalogs,
            this.tsbMergeCatalogs,
            this.tsbCatalog});
            this.toolStripMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Padding = new System.Windows.Forms.Padding(0);
            this.toolStripMain.ShowBorder = true;
            this.toolStripMain.Size = new System.Drawing.Size(838, 53);
            this.toolStripMain.TabIndex = 0;
            this.toolStripMain.VerticalGradient = false;
            // 
            // tsbNew
            // 
            this.tsbNew.ActiveImage = null;
            this.tsbNew.AutoSize = false;
            this.tsbNew.CheckedImage = null;
            this.tsbNew.DisabledImage = null;
            this.tsbNew.Image = ((System.Drawing.Image)(resources.GetObject("tsbNew.Image")));
            this.tsbNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNew.InactiveImage = ((System.Drawing.Image)(resources.GetObject("tsbNew.InactiveImage")));
            this.tsbNew.Name = "tsbNew";
            this.tsbNew.Size = new System.Drawing.Size(40, 40);
            this.tsbNew.Tag = "ToolActionNew";
            this.tsbNew.Text = "TXT_NEW";
            this.tsbNew.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbNew.ToolTipText = "Create new catalog";
            this.tsbNew.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsbOpen
            // 
            this.tsbOpen.ActiveImage = null;
            this.tsbOpen.AutoSize = false;
            this.tsbOpen.CheckedImage = null;
            this.tsbOpen.DisabledImage = null;
            this.tsbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpen.Image")));
            this.tsbOpen.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.InactiveImage = ((System.Drawing.Image)(resources.GetObject("tsbOpen.InactiveImage")));
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(60, 40);
            this.tsbOpen.Tag = "ToolActionOpen";
            this.tsbOpen.Text = "TXT_OPEN";
            this.tsbOpen.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbOpen.ToolTipText = "Open catalog";
            this.tsbOpen.ButtonClick += new System.EventHandler(this.OnToolAction);
            this.tsbOpen.DropDownOpening += new System.EventHandler(this.OnPrepareRecentFileList);
            this.tsbOpen.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.OnOpenRecentFile);
            // 
            // tsbSave
            // 
            this.tsbSave.ActiveImage = null;
            this.tsbSave.AutoSize = false;
            this.tsbSave.CheckedImage = null;
            this.tsbSave.DisabledImage = null;
            this.tsbSave.Enabled = false;
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.InactiveImage = ((System.Drawing.Image)(resources.GetObject("tsbSave.InactiveImage")));
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(40, 40);
            this.tsbSave.Tag = "ToolActionSave";
            this.tsbSave.Text = "TXT_SAVE";
            this.tsbSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbSave.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsbSaveAs
            // 
            this.tsbSaveAs.ActiveImage = null;
            this.tsbSaveAs.AutoSize = false;
            this.tsbSaveAs.CheckedImage = null;
            this.tsbSaveAs.DisabledImage = null;
            this.tsbSaveAs.Enabled = false;
            this.tsbSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("tsbSaveAs.Image")));
            this.tsbSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveAs.InactiveImage = ((System.Drawing.Image)(resources.GetObject("tsbSaveAs.InactiveImage")));
            this.tsbSaveAs.Name = "tsbSaveAs";
            this.tsbSaveAs.Size = new System.Drawing.Size(40, 40);
            this.tsbSaveAs.Tag = "ToolActionSaveAs";
            this.tsbSaveAs.Text = "TXT_SAVE_AS";
            this.tsbSaveAs.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbSaveAs.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tss1
            // 
            this.tss1.AutoSize = false;
            this.tss1.Name = "tss1";
            this.tss1.Size = new System.Drawing.Size(6, 53);
            // 
            // tsbBack
            // 
            this.tsbBack.ActiveImage = null;
            this.tsbBack.AutoSize = false;
            this.tsbBack.CheckedImage = null;
            this.tsbBack.DisabledImage = null;
            this.tsbBack.Enabled = false;
            this.tsbBack.Image = ((System.Drawing.Image)(resources.GetObject("tsbBack.Image")));
            this.tsbBack.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbBack.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbBack.InactiveImage = ((System.Drawing.Image)(resources.GetObject("tsbBack.InactiveImage")));
            this.tsbBack.Name = "tsbBack";
            this.tsbBack.Size = new System.Drawing.Size(40, 40);
            this.tsbBack.Tag = "ToolActionBack";
            this.tsbBack.Text = "TXT_BACK";
            this.tsbBack.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbBack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbBack.ToolTipText = "Back to ";
            this.tsbBack.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsbForward
            // 
            this.tsbForward.ActiveImage = null;
            this.tsbForward.AutoSize = false;
            this.tsbForward.CheckedImage = null;
            this.tsbForward.DisabledImage = null;
            this.tsbForward.Enabled = false;
            this.tsbForward.Image = ((System.Drawing.Image)(resources.GetObject("tsbForward.Image")));
            this.tsbForward.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbForward.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbForward.InactiveImage = ((System.Drawing.Image)(resources.GetObject("tsbForward.InactiveImage")));
            this.tsbForward.Name = "tsbForward";
            this.tsbForward.Size = new System.Drawing.Size(40, 40);
            this.tsbForward.Tag = "ToolActionFwd";
            this.tsbForward.Text = "TXT_FORWARD";
            this.tsbForward.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbForward.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbForward.ToolTipText = "Forward to";
            this.tsbForward.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsbUpLevel
            // 
            this.tsbUpLevel.ActiveImage = null;
            this.tsbUpLevel.AutoSize = false;
            this.tsbUpLevel.CheckedImage = null;
            this.tsbUpLevel.DisabledImage = null;
            this.tsbUpLevel.Enabled = false;
            this.tsbUpLevel.Image = ((System.Drawing.Image)(resources.GetObject("tsbUpLevel.Image")));
            this.tsbUpLevel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbUpLevel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbUpLevel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUpLevel.InactiveImage = ((System.Drawing.Image)(resources.GetObject("tsbUpLevel.InactiveImage")));
            this.tsbUpLevel.Name = "tsbUpLevel";
            this.tsbUpLevel.Size = new System.Drawing.Size(40, 40);
            this.tsbUpLevel.Tag = "ToolActionUp";
            this.tsbUpLevel.Text = "TXT_UP";
            this.tsbUpLevel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbUpLevel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbUpLevel.ToolTipText = "Up to";
            this.tsbUpLevel.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsbSearch
            // 
            this.tsbSearch.ActiveImage = null;
            this.tsbSearch.AutoSize = false;
            this.tsbSearch.CheckedImage = null;
            this.tsbSearch.DisabledImage = null;
            this.tsbSearch.Enabled = false;
            this.tsbSearch.Image = global::OPMedia.UI.Properties.Resources.Search;
            this.tsbSearch.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSearch.InactiveImage = global::OPMedia.UI.Properties.Resources.Search;
            this.tsbSearch.Name = "tsbSearch";
            this.tsbSearch.Size = new System.Drawing.Size(40, 40);
            this.tsbSearch.Tag = "ToolActionSearch";
            this.tsbSearch.Text = "TXT_SEARCH";
            this.tsbSearch.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbSearch.ToolTipText = "Search ...";
            this.tsbSearch.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsbReload
            // 
            this.tsbReload.ActiveImage = null;
            this.tsbReload.AutoSize = false;
            this.tsbReload.CheckedImage = null;
            this.tsbReload.DisabledImage = null;
            this.tsbReload.Enabled = false;
            this.tsbReload.Image = ((System.Drawing.Image)(resources.GetObject("tsbReload.Image")));
            this.tsbReload.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbReload.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReload.InactiveImage = ((System.Drawing.Image)(resources.GetObject("tsbReload.InactiveImage")));
            this.tsbReload.Name = "tsbReload";
            this.tsbReload.Size = new System.Drawing.Size(40, 40);
            this.tsbReload.Tag = "ToolActionReload";
            this.tsbReload.Text = "TXT_REFRESH";
            this.tsbReload.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbReload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbReload.ToolTipText = "Reload";
            this.tsbReload.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsbRename
            // 
            this.tsbRename.ActiveImage = null;
            this.tsbRename.AutoSize = false;
            this.tsbRename.CheckedImage = null;
            this.tsbRename.DisabledImage = null;
            this.tsbRename.Enabled = false;
            this.tsbRename.Image = ((System.Drawing.Image)(resources.GetObject("tsbRename.Image")));
            this.tsbRename.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbRename.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbRename.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRename.InactiveImage = ((System.Drawing.Image)(resources.GetObject("tsbRename.InactiveImage")));
            this.tsbRename.Name = "tsbRename";
            this.tsbRename.Size = new System.Drawing.Size(40, 40);
            this.tsbRename.Tag = "ToolActionRename";
            this.tsbRename.Text = "TXT_RENAME";
            this.tsbRename.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbRename.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbRename.ToolTipText = "Rename Selection";
            this.tsbRename.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsbDelete
            // 
            this.tsbDelete.ActiveImage = null;
            this.tsbDelete.AutoSize = false;
            this.tsbDelete.CheckedImage = null;
            this.tsbDelete.DisabledImage = null;
            this.tsbDelete.Enabled = false;
            this.tsbDelete.Image = ((System.Drawing.Image)(resources.GetObject("tsbDelete.Image")));
            this.tsbDelete.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbDelete.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelete.InactiveImage = ((System.Drawing.Image)(resources.GetObject("tsbDelete.InactiveImage")));
            this.tsbDelete.Name = "tsbDelete";
            this.tsbDelete.Size = new System.Drawing.Size(40, 40);
            this.tsbDelete.Tag = "ToolActionDelete";
            this.tsbDelete.Text = "TXT_DELETE";
            this.tsbDelete.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbDelete.ToolTipText = "Delete ";
            this.tsbDelete.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tssMergeCatalogs
            // 
            this.tssMergeCatalogs.Name = "tssMergeCatalogs";
            this.tssMergeCatalogs.Size = new System.Drawing.Size(6, 53);
            // 
            // tsbMergeCatalogs
            // 
            this.tsbMergeCatalogs.ActiveImage = null;
            this.tsbMergeCatalogs.AutoSize = false;
            this.tsbMergeCatalogs.CheckedImage = null;
            this.tsbMergeCatalogs.DisabledImage = null;
            this.tsbMergeCatalogs.Image = global::OPMedia.Addons.Builtin.Properties.Resources.Merge;
            this.tsbMergeCatalogs.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tsbMergeCatalogs.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbMergeCatalogs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMergeCatalogs.InactiveImage = global::OPMedia.Addons.Builtin.Properties.Resources.Merge;
            this.tsbMergeCatalogs.Name = "tsbMergeCatalogs";
            this.tsbMergeCatalogs.Size = new System.Drawing.Size(40, 40);
            this.tsbMergeCatalogs.Tag = "ToolActionMerge";
            this.tsbMergeCatalogs.Text = "TXT_MERGECATALOGS";
            this.tsbMergeCatalogs.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbMergeCatalogs.ToolTipText = "Merge Catalogs";
            this.tsbMergeCatalogs.Click += new System.EventHandler(this.OnToolAction);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.contextMenuStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiBack,
            this.tsmiFwd,
            this.tsmiUp,
            this.tsmiSep1,
            this.tsmiSearch,
            this.tsmiReload,
            this.tsmiSep2,
            this.tsmiCopy,
            this.tsmiCut,
            this.tsmiPaste,
            this.tsmiCopyPaste,
            this.tsmiRename,
            this.tsmiDelete,
            this.tsmiSep4,
            this.tsmiCatalog,
            this.tsmiSepProTONE,
            this.tsmiProTONEPlay,
            this.tsmiProTONEEnqueue});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(210, 320);
            // 
            // tsmiBack
            // 
            this.tsmiBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiBack.Image = ((System.Drawing.Image)(resources.GetObject("tsmiBack.Image")));
            this.tsmiBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiBack.Name = "tsmiBack";
            this.tsmiBack.Size = new System.Drawing.Size(209, 22);
            this.tsmiBack.Tag = "ToolActionBack";
            this.tsmiBack.Text = "TXT_BACK";
            this.tsmiBack.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiFwd
            // 
            this.tsmiFwd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiFwd.Image = ((System.Drawing.Image)(resources.GetObject("tsmiFwd.Image")));
            this.tsmiFwd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiFwd.Name = "tsmiFwd";
            this.tsmiFwd.Size = new System.Drawing.Size(209, 22);
            this.tsmiFwd.Tag = "ToolActionFwd";
            this.tsmiFwd.Text = "TXT_FORWARD";
            this.tsmiFwd.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiUp
            // 
            this.tsmiUp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiUp.Image = ((System.Drawing.Image)(resources.GetObject("tsmiUp.Image")));
            this.tsmiUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiUp.Name = "tsmiUp";
            this.tsmiUp.Size = new System.Drawing.Size(209, 22);
            this.tsmiUp.Tag = "ToolActionUp";
            this.tsmiUp.Text = "TXT_UP";
            this.tsmiUp.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiSep1
            // 
            this.tsmiSep1.Name = "tsmiSep1";
            this.tsmiSep1.Size = new System.Drawing.Size(206, 6);
            // 
            // tsmiSearch
            // 
            this.tsmiSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiSearch.Image = global::OPMedia.UI.Properties.Resources.Search16;
            this.tsmiSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiSearch.Name = "tsmiSearch";
            this.tsmiSearch.Size = new System.Drawing.Size(209, 22);
            this.tsmiSearch.Tag = "ToolActionSearch";
            this.tsmiSearch.Text = "TXT_SEARCH";
            this.tsmiSearch.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiReload
            // 
            this.tsmiReload.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiReload.Image = ((System.Drawing.Image)(resources.GetObject("tsmiReload.Image")));
            this.tsmiReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiReload.Name = "tsmiReload";
            this.tsmiReload.Size = new System.Drawing.Size(209, 22);
            this.tsmiReload.Tag = "ToolActionReload";
            this.tsmiReload.Text = "TXT_REFRESH";
            this.tsmiReload.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiSep2
            // 
            this.tsmiSep2.Name = "tsmiSep2";
            this.tsmiSep2.Size = new System.Drawing.Size(206, 6);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiCopy.Image = ((System.Drawing.Image)(resources.GetObject("tsmiCopy.Image")));
            this.tsmiCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.Size = new System.Drawing.Size(209, 22);
            this.tsmiCopy.Tag = "ToolActionCopy";
            this.tsmiCopy.Text = "TXT_COPY";
            this.tsmiCopy.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiCut
            // 
            this.tsmiCut.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiCut.Image = ((System.Drawing.Image)(resources.GetObject("tsmiCut.Image")));
            this.tsmiCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiCut.Name = "tsmiCut";
            this.tsmiCut.Size = new System.Drawing.Size(209, 22);
            this.tsmiCut.Tag = "ToolActionCut";
            this.tsmiCut.Text = "TXT_CUT";
            this.tsmiCut.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiPaste
            // 
            this.tsmiPaste.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiPaste.Image = ((System.Drawing.Image)(resources.GetObject("tsmiPaste.Image")));
            this.tsmiPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiPaste.Name = "tsmiPaste";
            this.tsmiPaste.Size = new System.Drawing.Size(209, 22);
            this.tsmiPaste.Tag = "ToolActionPaste";
            this.tsmiPaste.Text = "TXT_PASTE";
            this.tsmiPaste.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiCopyPaste
            // 
            this.tsmiCopyPaste.Name = "tsmiCopyPaste";
            this.tsmiCopyPaste.Size = new System.Drawing.Size(206, 6);
            this.tsmiCopyPaste.Tag = "";
            // 
            // tsmiRename
            // 
            this.tsmiRename.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiRename.Image = ((System.Drawing.Image)(resources.GetObject("tsmiRename.Image")));
            this.tsmiRename.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiRename.Name = "tsmiRename";
            this.tsmiRename.Size = new System.Drawing.Size(209, 22);
            this.tsmiRename.Tag = "ToolActionRename";
            this.tsmiRename.Text = "TXT_RENAME";
            this.tsmiRename.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiDelete.Image = ((System.Drawing.Image)(resources.GetObject("tsmiDelete.Image")));
            this.tsmiDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(209, 22);
            this.tsmiDelete.Tag = "ToolActionDelete";
            this.tsmiDelete.Text = "TXT_DELETE";
            this.tsmiDelete.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiSep4
            // 
            this.tsmiSep4.Name = "tsmiSep4";
            this.tsmiSep4.Size = new System.Drawing.Size(206, 6);
            // 
            // tsmiCatalog
            // 
            this.tsmiCatalog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiCatalog.ImageTransparentColor = System.Drawing.Color.White;
            this.tsmiCatalog.Name = "tsmiCatalog";
            this.tsmiCatalog.Size = new System.Drawing.Size(209, 22);
            this.tsmiCatalog.Tag = "";
            this.tsmiCatalog.Text = "TXT_CATALOG";
            this.tsmiCatalog.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiSepProTONE
            // 
            this.tsmiSepProTONE.Name = "tsmiSepProTONE";
            this.tsmiSepProTONE.Size = new System.Drawing.Size(206, 6);
            // 
            // tsmiProTONEPlay
            // 
            this.tsmiProTONEPlay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiProTONEPlay.Name = "tsmiProTONEPlay";
            this.tsmiProTONEPlay.Size = new System.Drawing.Size(209, 22);
            this.tsmiProTONEPlay.Tag = "ToolActionProTONEPlay";
            this.tsmiProTONEPlay.Text = "TXT_PROTONE_PLAY";
            this.tsmiProTONEPlay.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tsmiProTONEEnqueue
            // 
            this.tsmiProTONEEnqueue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.tsmiProTONEEnqueue.Name = "tsmiProTONEEnqueue";
            this.tsmiProTONEEnqueue.Size = new System.Drawing.Size(209, 22);
            this.tsmiProTONEEnqueue.Tag = "ToolActionProTONEEnqueue";
            this.tsmiProTONEEnqueue.Text = "TXT_PROTONE_ENQUEUE";
            this.tsmiProTONEEnqueue.Click += new System.EventHandler(this.OnToolAction);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.toolStripMain, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pnlSplitter, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.OverrideBackColor = System.Drawing.Color.Empty;
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(838, 478);
            this.tableLayoutPanel1.TabIndex = 4;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // pnlSplitter
            // 
            this.pnlSplitter.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnlSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.pnlSplitter.Location = new System.Drawing.Point(0, 53);
            this.pnlSplitter.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSplitter.Name = "pnlSplitter";
            // 
            // pnlSplitter.Panel1
            // 
            this.pnlSplitter.Panel1.Controls.Add(this.tvCatalog);
            // 
            // pnlSplitter.Panel2
            // 
            this.pnlSplitter.Panel2.Controls.Add(this.lvCatalogFolder);
            this.pnlSplitter.Size = new System.Drawing.Size(838, 425);
            this.pnlSplitter.SplitterDistance = 217;
            this.pnlSplitter.SplitterWidth = 3;
            this.pnlSplitter.TabIndex = 1;
            // 
            // tvCatalog
            // 
            this.tvCatalog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvCatalog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvCatalog.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.tvCatalog.FullRowSelect = true;
            this.tvCatalog.HideSelection = false;
            this.tvCatalog.ImageIndex = 0;
            this.tvCatalog.Location = new System.Drawing.Point(0, 0);
            this.tvCatalog.Margin = new System.Windows.Forms.Padding(0);
            this.tvCatalog.Name = "tvCatalog";
            this.tvCatalog.SelectedImageIndex = 0;
            this.tvCatalog.Size = new System.Drawing.Size(217, 425);
            this.tvCatalog.TabIndex = 0;
            // 
            // lvCatalogFolder
            // 
            this.lvCatalogFolder.AllowEditing = true;
            this.lvCatalogFolder.AlternateRowColors = true;
            this.lvCatalogFolder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvCatalogFolder.ContextMenuStrip = this.contextMenuStrip;
            this.lvCatalogFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvCatalogFolder.LabelEdit = true;
            this.lvCatalogFolder.Location = new System.Drawing.Point(0, 0);
            this.lvCatalogFolder.Margin = new System.Windows.Forms.Padding(0);
            this.lvCatalogFolder.MultiSelect = false;
            this.lvCatalogFolder.Name = "lvCatalogFolder";
            this.lvCatalogFolder.OverrideBackColor = System.Drawing.Color.Empty;
            this.lvCatalogFolder.ShowItemToolTips = true;
            this.lvCatalogFolder.Size = new System.Drawing.Size(618, 425);
            this.lvCatalogFolder.TabIndex = 0;
            this.lvCatalogFolder.UseCompatibleStateImageBehavior = false;
            this.lvCatalogFolder.View = System.Windows.Forms.View.Details;
            this.lvCatalogFolder.DoubleClickDirectory += new OPMedia.UI.Controls.DoubleClickDirectoryEventHandler(this.OnDoubleClickDirectory);
            this.lvCatalogFolder.DoubleClickFile += new OPMedia.UI.Controls.DoubleClickFileEventHandler(this.OnDoubleClickFile);
            this.lvCatalogFolder.SelectDirectory += new OPMedia.UI.Controls.SelectDirectoryEventHandler(this.OnSelectDirectory);
            this.lvCatalogFolder.SelectFile += new OPMedia.UI.Controls.SelectFileEventHandler(this.OnSelectFile);
            this.lvCatalogFolder.SelectMultipleItems += new OPMedia.UI.Controls.SelectMultipleItemsEventHandler(this.OnSelectMultipleItems);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItem2.Text = "toolStripMenuItem2";
            // 
            // AddonPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "AddonPanel";
            this.Size = new System.Drawing.Size(838, 478);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlSplitter.Panel1.ResumeLayout(false);
            this.pnlSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplitter)).EndInit();
            this.pnlSplitter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private OPMTriStateToolStripButton tsbNew;
        private OPMToolStripSplitButton tsbOpen;
        private OPMTriStateToolStripButton tsbSave;
        private OPMTriStateToolStripButton tsbSaveAs;

        private OPMToolStripSeparator tss1;

        private OPMTriStateToolStripButton tsbBack;
        private OPMTriStateToolStripButton tsbForward;
        private OPMTriStateToolStripButton tsbUpLevel;
        
        private OPMToolStripSeparator tss2;

        private OPMTriStateToolStripButton tsbSearch;
        private OPMTriStateToolStripButton tsbReload;

        private OPMToolStripSeparator tss3;

        private OPMTriStateToolStripButton tsbRename;
        private OPMTriStateToolStripButton tsbDelete;

        private OPMToolStripSeparator tssMergeCatalogs;

        private OPMTriStateToolStripButton tsbMergeCatalogs;
        private OPMTriStateToolStripButton tsbCatalog;

        private OPMToolStrip toolStripMain;
        private OPMContextMenuStrip contextMenuStrip;
        private OPMToolStripMenuItem tsmiBack;
        private OPMToolStripMenuItem tsmiFwd;
        private OPMToolStripMenuItem tsmiUp;
        private OPMMenuStripSeparator tsmiSep1;
        private OPMToolStripMenuItem tsmiSearch;
        private OPMToolStripMenuItem tsmiReload;
        private OPMMenuStripSeparator tsmiSep2;
        private OPMToolStripMenuItem tsmiCopy;
        private OPMToolStripMenuItem tsmiCut;
        private OPMToolStripMenuItem tsmiPaste;
        private OPMToolStripSeparator tsmiCopyPaste;
        private OPMToolStripMenuItem tsmiRename;
        private OPMToolStripMenuItem tsmiDelete;
        private OPMMenuStripSeparator tsmiSep4;
        private OPMToolStripMenuItem tsmiCatalog;
        private OPMMenuStripSeparator tsmiSepProTONE;
        private OPMToolStripMenuItem tsmiProTONEPlay;
        private OPMToolStripMenuItem tsmiProTONEEnqueue;
        private OPMedia.Addons.Builtin.CatalogExplorer.Controls.CatalogListView lvCatalogFolder;
        private OPMedia.Addons.Builtin.CatalogExplorer.Controls.CatalogTreeView tvCatalog;
        
        
        private OPMTableLayoutPanel tableLayoutPanel1;
        private OPMSplitContainer pnlSplitter;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
    }
}
