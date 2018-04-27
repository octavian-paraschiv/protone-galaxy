using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OPMedia.UI.Themes;
using System.IO;
using OPMedia.Core.Utilities;
using System.Threading;
using OPMedia.Core.TranslationSupport;
using OPMedia.Core;

namespace OPMedia.UI.Dialogs
{
    public delegate bool PerformPathValidationHandler(string path);

    public partial class OPMFolderBrowserDialog : ToolForm
    {
        public bool ShowSpecialFolders { get; set; }
        public bool ShowNewFolderButton { get; set; }
        public string SelectedPath { get; set; }
        public string Description { get; set; }

        public event PerformPathValidationHandler PerformPathValidation = null;

        public OPMFolderBrowserDialog()
        {
            InitializeComponent();

            this.InheritAppIcon = false;

            this.Description = Translator.Translate("TXT_SELECT_FOLDER");

            this.ShowNewFolderButton = true;
            this.SelectedPath = PathUtils.CurrentDir;

            btnOK.Enabled = false;
            tvExplorer.LabelEdit = true;

            tvExplorer.AfterSelect += new TreeViewEventHandler(tvExplorer_AfterSelect);
            this.Load += new EventHandler(OPMFolderBrowserDialog_Load);
        }

        void tvExplorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnOK.Enabled = false;
            btnNewFolder.Enabled = false;

            if (Directory.Exists(tvExplorer.SelectedNodePath))
            {
                btnOK.Enabled = (PerformPathValidation == null || PerformPathValidation(tvExplorer.SelectedNodePath));

                try
                {
                    DriveInfo drvInvo = new DriveInfo(Path.GetPathRoot(tvExplorer.SelectedNodePath));
                    btnNewFolder.Enabled = (drvInvo.AvailableFreeSpace > 0 && drvInvo.IsReady);
                }
                catch { }
            }

            if (btnOK.Enabled)
            {
                this.SelectedPath = tvExplorer.SelectedNodePath;
            }
            else
            {
                this.SelectedPath = string.Empty;
            }
        }

        void OPMFolderBrowserDialog_Load(object sender, EventArgs e)
        {
            SetTitle("TXT_SELECT_FOLDER");

            btnNewFolder.Visible = this.ShowNewFolderButton;
            lblDescription.Text = this.Description;

            tvExplorer.ShowSpecialFolders = this.ShowSpecialFolders;
            tvExplorer.InitOPMShellTreeView();

            tvExplorer.DrillToFolder(this.SelectedPath);
        }

        void OPMFolderBrowserDialog_Shown(object sender, EventArgs e)
        {
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(this.SelectedPath))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnNewFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(SelectedPath))
            {
                string newName = StringUtils.GetRandomNewFolderName();
                string fullName = Path.Combine(SelectedPath, newName);

                Directory.CreateDirectory(fullName);
                Thread.Sleep(700);
                if (Directory.Exists(fullName))
                {
                    TreeNode node = tvExplorer.CreateTreeNode(fullName);
                    tvExplorer.SelectedNode.Nodes.Add(node);
                    tvExplorer.SelectedNode = node;

                    tvExplorer.SelectedNode.BeginEdit();
                }
            }
        }
    }
}
