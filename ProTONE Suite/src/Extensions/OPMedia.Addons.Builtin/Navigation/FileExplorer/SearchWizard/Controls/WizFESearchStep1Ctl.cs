using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OPMedia.UI.Wizards;
using System.IO;
using OPMedia.Addons.Builtin.FileExplorer.SearchWizard.Tasks;
using OPMedia.Core.Configuration;
using OPMedia.Core.TranslationSupport;
using OPMedia.UI.Themes;
using OPMedia.Core;

using OPMedia.Runtime;
using OPMedia.Runtime.ProTONE;
using OPMedia.Runtime.ProTONE.FileInformation;
using OPMedia.Runtime.ProTONE.Rendering;
using OPMedia.Runtime.ProTONE.ExtendedInfo;

using OPMedia.UI.Controls;
using OPMedia.Core.Utilities;
using OPMedia.UI.Dialogs;
using OPMedia.Addons.Builtin.Configuration;
using OPMedia.Runtime.ProTONE.Configuration;

using System.Linq;
using System.Threading;
using System.Diagnostics;
using OPMedia.Addons.Builtin.Shared;
using OPMedia.UI.Generic;

namespace OPMedia.Addons.Builtin.FileExplorer.SearchWizard.Controls
{
    public partial class WizFESearchStep1Ctl : WizardBaseCtl
    {
        FileSystemImageListManager _ilm = new FileSystemImageListManager(false);

        private Task theTask
        {
            get
            {
                return (BkgTask as Task);
            }
        }

        public override Size DesiredSize
        {
            get
            {
                return new Size(700, 600);
            }
        }


        public WizFESearchStep1Ctl()
        {
            InitializeComponent();

            this.tsmiProTONEPlay.Image = ImageProcessing.Player16;
            this.tsmiProTONEEnqueue.Image = ImageProcessing.Player16;

            this.ShowImage = false;
            this.ShowSeparator = false;

            lbAttributes.Height = 0;
            lbAttributes.Visible = false;

            lvResults.MultiSelect = true;
            lvResults.SmallImageList = _ilm.ImageList;

            foreach(FileAttributes attr in Enum.GetValues(typeof(FileAttributes)))
            {
                lbAttributes.Items.Add(attr);
            }

            Translator.TranslateToolStrip(contextMenuStrip, DesignMode);
        }

        private void OnClearSearchPatternHistory(object sender, EventArgs e)
        {
            BuiltinAddonConfig.SearchPatterns = string.Empty;
            
            PopulateSearchPattern();
        }

        private void OnClearSearchValueHistory(object sender, EventArgs e)
        {
            BuiltinAddonConfig.SearchTexts = string.Empty;
            
            PopulateSearchText();
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (!enableEvents) return;

            try
            {
                enableEvents = false;

                if (sender == txtSearchPath)
                {
                    if (txtSearchPath.Text == Translator.Translate("TXT_BROWSE"))
                    {
                        return;
                    }
                    else
                    {
                        theTask.SearchPath = txtSearchPath.Text;

                        //cmbSearchPath.SelectedItem = null;
                        //cmbSearchPath.AddUniqueItem(theTask.SearchPath.ToLowerInvariant());
                        //cmbSearchPath.SelectedItem = theTask.SearchPath.ToLowerInvariant();
                    }
                }

                if (SearchBookmarksActive())
                {
                    chkOption1.Visible = true; chkOption2.Visible = true;
                    chkOption1.Text = Translator.Translate("TXT_NON_ORPHAN_BOOKMARKS");
                    chkOption2.Text = Translator.Translate("TXT_ORPHAN_BOOKMARKS");
                }
                else if (SearchMediaFilesActive())
                {
                    chkOption1.Visible = true; chkOption2.Visible = true;
                    chkOption1.Text = Translator.Translate("TXT_MEDIA_WITH_BOOKMARKS");
                    chkOption2.Text = Translator.Translate("TXT_MEDIA_WITHOUT_BOOKMARKS");
                }
                else
                {
                    chkOption1.Visible = false; chkOption2.Visible = false;
                }

                lbAttributes.Height = (chkAttrSearch.Checked ? 73 : 0);
                lbAttributes.Visible = chkAttrSearch.Checked;
                lbAttributes.Enabled = chkAttrSearch.Checked;

                theTask.SearchPattern = cmbSearchPattern.Text;
                theTask.SearchText = cmbSearchText.Text;
                theTask.SearchPath = txtSearchPath.Text;

                theTask.IsCaseInsensitive = chkNoCase.Checked;
                theTask.IsRecursive = chkRecursive.Checked;
                theTask.UseAttributes = chkAttrSearch.Checked;

                theTask.SearchProperties = chkPropSearch.Checked;
            }
            finally
            {
                enableEvents = true;
            }
        }


        protected override void OnPageEnter_Initializing()
        {
            base.OnPageEnter_Initializing();

            Wizard.AllowResize = true;

            pbProgress.BackColor = ThemeManager.BackColor;

            Display();
        }

        bool enableEvents = false;
        private void Display()
        {
            Wizard.CanFinish = false;
            Wizard.CanMoveBack = false;
            Wizard.CanMoveNext = false;
            Wizard.StepButtonsVisible = false;
            Wizard.AcceptButton = btnSearch.Button;

            enableEvents = false;

            PopulateSearchPattern();
            PopulateSearchText();
            txtSearchPath.Text = theTask.SearchPath;

            chkNoCase.Checked = theTask.IsCaseInsensitive;
            chkRecursive.Checked = theTask.IsRecursive;
            chkAttrSearch.Checked = theTask.UseAttributes;

            for(int i = 0; i < lbAttributes.Items.Count; i++)
            {
                FileAttributes attr = (FileAttributes)lbAttributes.Items[i];
                lbAttributes.SetItemChecked(i, ((theTask.Attributes & attr) == attr));
            }

            enableEvents = true;
        }

        private void PopulateSearchText()
        {
            cmbSearchText.Items.Clear();
            string[] fields = StringUtils.ToStringArray(BuiltinAddonConfig.SearchTexts, '?');

            if (fields != null)
            {
                foreach (String str in fields)
                {
                    cmbSearchText.AddUniqueItem(str);
                }
            }

            cmbSearchText.AddUniqueItem(theTask.SearchText);

            cmbSearchText.SelectedItem = theTask.SearchText;
        }

        private void PopulateSearchPattern()
        {
            cmbSearchPattern.Items.Clear();
            string[] fields = StringUtils.ToStringArray(BuiltinAddonConfig.SearchPatterns, '?');

            if (fields != null)
            {
                foreach (String str in fields)
                {
                    cmbSearchPattern.AddUniqueItem(str);
                }
            }
            
            cmbSearchPattern.AddUniqueItem(theTask.SearchPattern);

            cmbSearchPattern.AddUniqueItem(Translator.Translate("TXT_SEARCH_BOOKMARKS"));
            cmbSearchPattern.AddUniqueItem(Translator.Translate("TXT_SEARCH_MEDIAFILES"));

            cmbSearchPattern.SelectedItem = theTask.SearchPattern;
        }

        private void SaveSearchSettings()
        {
            if (txtSearchPath.Text != Translator.Translate("TXT_BROWSE"))
            {
                BuiltinAddonConfig.SearchPaths = SaveSetting(BuiltinAddonConfig.SearchPaths, txtSearchPath.Text);
            }

            if (!SearchMediaFilesActive() && !SearchBookmarksActive())
            {
                BuiltinAddonConfig.SearchPatterns = SaveSetting(BuiltinAddonConfig.SearchPatterns, cmbSearchPattern.Text);
            }

            BuiltinAddonConfig.SearchTexts = SaveSetting(BuiltinAddonConfig.SearchTexts, cmbSearchText.Text);
            
        }

        private string SaveSetting(string initialSetting, string settingToAdd)
        {
            if (string.IsNullOrEmpty(initialSetting))
                return settingToAdd;

            Queue<string> parts = new Queue<string>();

            string[] fields = StringUtils.ToStringArray(initialSetting, '?');
            if (fields != null)
            {
                foreach (string part in fields)
                {
                    if (!parts.Contains(part))
                    {
                        parts.Enqueue(part);
                    }
                }
            }

            if (!parts.Contains(settingToAdd))
            {
                parts.Enqueue(settingToAdd);
            }

            while (parts.Count > 20)
            {
                parts.Dequeue();
            }

            return StringUtils.FromStringArray(parts.ToArray(), '?');
        }

        bool _searchInProgress = false;

        ManualResetEvent _shouldCancelSearch = new ManualResetEvent(false);

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (_searchInProgress)
            {
                _shouldCancelSearch.Set();
                return;
            }

            theTask.Attributes ^= theTask.Attributes;

            if (lbAttributes.Enabled)
            {
                foreach (FileAttributes attr in lbAttributes.CheckedItems)
                {
                    theTask.Attributes |= attr;
                }
            }

            SaveSearchSettings();

            _shouldCancelSearch.Reset();

            _searchInProgress = true;
            btnSearch.Text = Translator.Translate("TXT_CANCEL");

            lvResults.Items.Clear();
            _ilm.Clear();

            ExecuteSearch();

            _searchInProgress = false;
            btnSearch.Text = Translator.Translate("TXT_SEARCH");
        }

        List<ListViewItem> _listViewItems = new List<ListViewItem>();

        private void CreateListViewItem(string path)
        {
            ListViewItem item = new ListViewItem(new string[] { path } );
            item.Tag = path;
            item.ImageKey = _ilm.GetImageKey(path);

            _listViewItems.Add(item);
        }

        List<string> _matchingItems = new List<string>();
        List<string> _displayItems = new List<string>();

        private void ExecuteSearch()
        {
            _matchingItems.Clear();
            _displayItems.Clear();
            _listViewItems.Clear();
            lvResults.Items.Clear();

            if (_shouldCancelSearch.WaitOne(0))
                return;

            try
            {
                UseWaitCursor = true;

                pbProgress.Maximum = 1;
                pbProgress.Value = 0;
                pbProgress.Visible = true;

                ssStatus.Text = Translator.Translate("TXT_INIT_SEARCH");
                Application.DoEvents();

                bool needFolderLookup = 
                    (theTask.UseAttributes == true) && ((theTask.Attributes & FileAttributes.Directory) == FileAttributes.Directory);

                string actualSearchPattern = theTask.SearchPattern;
                if (theTask.SearchPattern == Translator.Translate("TXT_SEARCH_BOOKMARKS"))
                {
                    actualSearchPattern = "*.bmk";
                    needFolderLookup = false;
                }
                else if (theTask.SearchPattern == Translator.Translate("TXT_SEARCH_MEDIAFILES"))
                {
                    needFolderLookup = false;
                    actualSearchPattern = MediaRenderer.AllMediaTypesMultiFilter;
                }

                List<string> fsEntries = new List<string>();
                
                SearchOption searchOption = theTask.IsRecursive ?
                    SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                if (needFolderLookup)
                {
                    List<string> subdirs = PathUtils.EnumDirectoriesUsingMultiFilter(_shouldCancelSearch,
                        theTask.SearchPath, actualSearchPattern, searchOption);
                    if (subdirs != null)
                        fsEntries.AddRange(subdirs);
                }

                List<string> files = PathUtils.EnumFilesUsingMultiFilter(_shouldCancelSearch,
                    theTask.SearchPath, actualSearchPattern, searchOption);
                if (files != null)
                    fsEntries.AddRange(files);

                pbProgress.Maximum = fsEntries.Count + 1;

                foreach (string fsi in fsEntries)
                {
                    if (_shouldCancelSearch.WaitOne(0))
                        return;

                    if (TestFolder(fsi) == false)
                        TestFile(fsi);

                    ssStatus.Text = fsi;
                    pbProgress.Value = pbProgress.Value + 1;
                    Application.DoEvents();
                }

                _displayItems.Sort();

                foreach (string str in _displayItems)
                    CreateListViewItem(str);
            }
            catch(Exception ex)
            {
                ssStatus.Text = ex.Message;
            }
            finally
            {
                lvResults.Items.AddRange(_listViewItems.ToArray());

                if (lvResults.Items.Count > 0)
                    ssStatus.Text = Translator.Translate("TXT_TOTAL_ITEMS_FOUND", lvResults.Items.Count);
                else
                    ssStatus.Text = Translator.Translate("TXT_NO_ITEMS_FOUND");

                pbProgress.Visible = false;
                UseWaitCursor = false;

                GC.Collect();
            }
        }

        private bool TestFolder(string folder)
        {
            if (_shouldCancelSearch.WaitOne(0))
                return false;

            if (folder != null && Directory.Exists(folder))
            {
                if (!theTask.UseAttributes || AttributesMatch(File.GetAttributes(folder), false))
                {
                    List<string> childFolders = new List<string>();
                    childFolders.AddRange(PathUtils.EnumDirectoriesUsingMultiFilter(folder, theTask.SearchPattern, SearchOption.TopDirectoryOnly));

                    if (_matchingItems.Contains(folder.ToLowerInvariant()) == false)
                    {
                        _matchingItems.Add(folder.ToLowerInvariant());
                        _displayItems.Add(folder);

                        return true;
                    }
                }
            }

            return false;
        }

        private bool TestFile(string file)
        {
            if (_shouldCancelSearch.WaitOne(0))
                return false;

            if (file == null || File.Exists(file) == false)
                return false;

            if (SearchBookmarksActive())
            {
                BookmarkFileInfo bfi = new BookmarkFileInfo(file, false);
                if (bfi == null ||
                    !bfi.IsValid ||
                    (theTask.Option1 && bfi.IsOrphan) || /* for BMK files: option 1 = non-orphan bookmarks */
                    (theTask.Option2 && !bfi.IsOrphan))  /* for BMK files: option 2 = orphan bookmarks */
                {
                    return false; // skip this file
                }
            }
            else if (SearchMediaFilesActive())
            {
                if (!MediaRenderer.IsSupportedMedia(file))
                    return false; // not a media file

                MediaFileInfo mfi = MediaFileInfo.FromPath(file);
                if (mfi == null ||
                    !mfi.IsValid ||
                    (theTask.Option1 && mfi.Bookmarks.Count < 1) || /* for media files: option 1 = files with bookmarks */
                    (theTask.Option2 && mfi.Bookmarks.Count > 0))  /* for media files: option 2 = files w/o bookmarks */
                {
                    return false; // skip this file
                }
            }

            if (!theTask.UseAttributes || AttributesMatch(File.GetAttributes(file), true))
            {
                bool match = 
                    (theTask.SearchText.Length == 0) ||
                    (theTask.SearchProperties && ContainsProperty(file)) ||
                    (!theTask.SearchProperties && ContainsText(file));

                if (match && !_matchingItems.Contains(file.ToLowerInvariant()))
                {
                    _matchingItems.Add(file.ToLowerInvariant());
                    _displayItems.Add(file);

                    return true;
                }
            }

            return false;
        }

        private bool ContainsProperty(string file)
        {
            if (_shouldCancelSearch.WaitOne(0))
                return false;

            try
            {
                // So far only media files can have "text" properties
                if (MediaRenderer.IsSupportedMedia(file))
                {
                    MediaFileInfo mfi = MediaFileInfo.FromPath(file);
                    if (mfi != null)
                    {
                        foreach (KeyValuePair<string, string> kvp in mfi.ExtendedInfo)
                        {
                            string keyword = theTask.SearchText;
                            string name = kvp.Key;
                            string value = kvp.Value;

                            if (theTask.IsCaseInsensitive)
                            {
                                keyword = keyword != null ? keyword.ToLowerInvariant() : string.Empty;
                                name = name != null ? name.ToLowerInvariant() : string.Empty;
                                value = value != null ? value.ToLowerInvariant() : string.Empty;
                            }

                            if (name.Contains(keyword) || value.Contains(keyword))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            finally
            {
                GC.Collect();
            }
        }

        private bool ContainsText(string file)
        {
            if (_shouldCancelSearch.WaitOne(0))
                return false;

            try
            {
                string contents = string.Empty;
                using (StreamReader sr = new StreamReader(file))
                {
                    contents = sr.ReadToEnd();
                }

                if (!string.IsNullOrEmpty(contents))
                {
                    string keyword = theTask.SearchText;

                    if (theTask.IsCaseInsensitive)
                    {
                        keyword = keyword != null ? keyword.ToLowerInvariant() : string.Empty;
                        contents = contents.ToLowerInvariant();
                    }

                    return contents.Contains(keyword);
                }

                return false;
            }
            finally
            {
                GC.Collect();
            }
        }

        private bool AttributesMatch(FileAttributes fileAttributes, bool isFile)
        {
            if (_shouldCancelSearch.WaitOne(0))
                return false;

            if (isFile && (theTask.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                return false; // A file cannot have the Directory attrribute

            foreach (FileAttributes attr in Enum.GetValues(typeof(FileAttributes)))
            {
                if ((theTask.Attributes & attr) != attr)
                    continue;

                if ((fileAttributes & attr) == attr)
                    return true;
            }

            return false;
        }

        private void OnMenuOpening(object sender, CancelEventArgs e)
        {
            bool playerInstalled = File.Exists(ProTONEConfig.PlayerInstallationPath);
            tsmiSepProTONE.Visible = tsmiProTONEEnqueue.Visible = tsmiProTONEPlay.Visible =
                playerInstalled;

            bool enable = false;
            foreach (string path in GetSelectedItems())
            {
                if (MediaRenderer.IsSupportedMedia(path))
                {
                    enable = true;
                    break;
                }
            }

            tsmiProTONEEnqueue.Enabled = tsmiProTONEPlay.Enabled = enable;

            tsmiTaggingWizard.Enabled = (lvResults.SelectedItems.Count == 1);
        }

        private void OnToolAction(object sender, EventArgs e)
        {
            HandleAction(sender as ToolStripItem);
        }

        private void HandleAction(ToolStripItem tsi)
        {
            if (tsi == null || string.IsNullOrEmpty(tsi.Tag as string))
                return;

            theTask.Action = (ToolAction)Enum.Parse(typeof(ToolAction),
                   tsi.Tag as string);
            theTask.MatchingItems = GetSelectedItems();

            FindForm().DialogResult = DialogResult.OK;
            FindForm().Close();
        }

        private List<string> GetSelectedItems()
        {
            List<string> selPaths = new List<string>();
            foreach (ListViewItem item in lvResults.SelectedItems)
            {
                string pathToAdd = item.Tag as string;
                if (pathToAdd != null) selPaths.Add(pathToAdd);
            }

            return selPaths;
        }

        private void lvResults_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            theTask.Action = ToolAction.ToolActionLaunch;
            theTask.MatchingItems = GetSelectedItems();

            FindForm().DialogResult = DialogResult.OK;
            FindForm().Close();
        }

        private void OnBookmarksCheckedChanged(object sender, EventArgs e)
        {
            if (!enableEvents) return;

            chkOption1.CheckedChanged -= new EventHandler(OnBookmarksCheckedChanged);
            chkOption2.CheckedChanged -= new EventHandler(OnBookmarksCheckedChanged);

            if (sender == chkOption1 && chkOption1.Checked)
            {
                chkOption2.Checked = false;
            }
            else if (sender == chkOption2 && chkOption2.Checked)
            {
                chkOption1.Checked = false;
            }

            chkOption1.CheckedChanged += new EventHandler(OnBookmarksCheckedChanged);
            chkOption2.CheckedChanged += new EventHandler(OnBookmarksCheckedChanged);

            theTask.Option1 = chkOption1.Checked;
            theTask.Option2 = chkOption2.Checked;
        }

        private bool SearchBookmarksActive()
        {
            string str1 = cmbSearchPattern.Text.ToLowerInvariant();
            string str2 = Translator.Translate("TXT_SEARCH_BOOKMARKS").ToLowerInvariant();
            return str1 == str2;
        }

        private bool SearchMediaFilesActive()
        {
            string str1 = cmbSearchPattern.Text.ToLowerInvariant();
            string str2 = Translator.Translate("TXT_SEARCH_MEDIAFILES").ToLowerInvariant();
            return str1 == str2;
        }

        private void OnResize(object sender, EventArgs e)
        {
            colImage.Width = 20;
            colPath.Width = lvResults.EffectiveWidth - colImage.Width;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OPMFolderBrowserDialog dlg = new OPMFolderBrowserDialog();
            dlg.SelectedPath = theTask.SearchPath;

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                theTask.SearchPath = dlg.SelectedPath;
                txtSearchPath.Text = dlg.SelectedPath;
            }
        }
    }
}
