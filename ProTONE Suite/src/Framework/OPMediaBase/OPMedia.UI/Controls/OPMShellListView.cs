#region Copyright © opmedia research 2006
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written permission of the copyright owner.

// File: 	OPMShellListView.cs
#endregion

#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.IO;

using System.Windows.Forms;
using System.Runtime.InteropServices;
using OPMedia.Runtime;

using System.Threading;
using OPMedia.Core.Configuration;
using OPMedia.Core;
using OPMedia.Core.Logging;
using OPMedia.Core.TranslationSupport;
using OPMedia.UI.Themes;

using OPMedia.UI.Controls;
using OPMedia.Core.GlobalEvents;

using System.Linq;
using System.Globalization;
using OPMedia.Core.Utilities;
using OPMedia.UI.FileTasks;
using OPMedia.Core.NetworkAccess;

#endregion

namespace OPMedia.UI.Controls
{
    #region Delegates
    /// <summary>
    /// Delegate used for the DoubleClickDirectory event.
    /// </summary>
    public delegate void DoubleClickDirectoryEventHandler(object sender, DoubleClickDirectoryEventArgs args);
    /// <summary>
    /// Delegate used for the DoubleClickFile event.
    /// </summary>
    public delegate void DoubleClickFileEventHandler(object sender, DoubleClickFileEventArgs args);
    /// <summary>
    /// Delegate used for the SelectDirectory event.
    /// </summary>
    public delegate void SelectDirectoryEventHandler(object sender, SelectDirectoryEventArgs args);
    /// <summary>
    /// Delegate used for the SelectFile event.
    /// </summary>
    public delegate void SelectFileEventHandler(object sender, SelectFileEventArgs args);
    /// <summary>
    /// Delegate used for the SelectMultipleItems event.
    /// </summary>
    public delegate void SelectMultipleItemsEventHandler(object sender, SelectMultipleItemsEventArgs args);

    public delegate List<string> QueryLinkedFilesHandler(string path, FileTaskType taskType);
    public delegate void ItemRenameHandler(string newPath);

    public delegate bool LaunchMultipleItemsHandler(object sender, System.EventArgs e);

    public delegate string QueryDisplayNameHandler(string fsi);

    #endregion

    #region Main class
    /// <summary>
	/// Component that implements a list with the contents of a
    /// given file system folder.
	/// </summary>
	public class OPMShellListView : OPMListView
    {
        #region Members

        System.Windows.Forms.Timer _delayedExplore = null;
        System.Windows.Forms.Timer _delayedSelectionTimer = null;
        System.Windows.Forms.Timer _delayedRenameTimer = null;

        private string m_strDirPath = Environment.CurrentDirectory;
		private string m_strPrevDirPath = "";
		
        private bool ignoreEvents = false;

        private object syncRoot = new object();

        FileSystemWatcher fsw = null;

        private Stack<string> bckPaths = new Stack<string>();
        private Stack<string> fwdPaths = new Stack<string>();

        ColumnHeader colName = new ColumnHeader();
        ColumnHeader colLastAccess = new ColumnHeader();
        ColumnHeader colSize = new ColumnHeader();
        ColumnHeader colAttr = new ColumnHeader();

        private FileSystemImageListManager m_ilDirListManager = new FileSystemImageListManager(false);

        #endregion

        #region Events

        public event LaunchMultipleItemsHandler LaunchMultipleItems = null;

        /// <summary>
        /// Occurs when a directory is double clicked.
        /// Directory path is part of the event data.
        /// </summary>
        public event DoubleClickDirectoryEventHandler DoubleClickDirectory = null;
        /// <summary>
        /// Occurs when a file is double clicked.
        /// File path is part of the event data.
        /// </summary>
        public event DoubleClickFileEventHandler DoubleClickFile = null;
        /// <summary>
        /// Occurs when a directory is selected.
        /// Directory path is part of the event data.
        /// </summary>
        public event SelectDirectoryEventHandler SelectDirectory = null;
        /// <summary>
        /// Occurs when a file is selected.
        /// File path is part of the event data.
        /// </summary>
        public event SelectFileEventHandler SelectFile = null;
        /// <summary>
        /// Occurs when multiple items are selected.
        /// Items paths are part of the event data.
        /// </summary>
        public event SelectMultipleItemsEventHandler SelectMultipleItems = null;

        public event QueryLinkedFilesHandler QueryLinkedFiles = null;
        public event ItemRenameHandler ItemRenamed = null;

        public event QueryDisplayNameHandler QueryDisplayName = null;

        #endregion

        #region Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DefaultValue(true)]
        public bool ShowAttributes { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DefaultValue(true)]
        public bool ShowLastAccess { get; set; }

        [ReadOnly(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ImageList SmallImageList { get { return base.SmallImageList; } }

        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private new ColumnHeaderCollection Columns
        {
            get
            {
                return base.Columns;
            }
        }

        public List<string> SelectedPaths
        {
            get
            {
                List<string> selectedItems = new List<string>();

                foreach (ListViewItem item in SelectedItems)
                {
                    string fsi = item.Tag as string;
                    if (fsi != null && string.Compare(fsi, ParentFolderTarget, true) != 0)
                    {
                        if (PathUtils.Exists(fsi))
                            selectedItems.Add(fsi);
                    }
                }

                return selectedItems;
            }
        }

        /// <summary>
        /// Gets/sets the current folder path.
        /// Note: setting the property toggles exploring for 
        /// files and folder on the new path. Wrong path specified
        /// results in an Exception being thrown.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public string Path
        {
            get
            {
                return m_strDirPath;
            }

            set
            {
                try
                {
                    ignoreEvents = true;

                    m_strPrevDirPath = m_strDirPath;

                    if (value == PathUtils.DirectorySeparator)
                    {
                        if (PathUtils.IsRootPath(m_strDirPath))
                        {
                            return;
                        }
                        else
                        {
                            m_strDirPath = System.IO.Path.GetPathRoot(m_strDirPath);
                        }
                    }
                    else if (value == PathUtils.ParentDir)
                    {
                        m_strDirPath = System.IO.Path.GetDirectoryName(m_strDirPath);
                    }
                    else if (value == PathUtils.CurrentDir)
                    {
                        m_strDirPath = Environment.CurrentDirectory;
                    }
                    else
                    {
                        m_strDirPath = value;
                    }

                    if (fsw != null)
                    {
                        fsw.Created -= new FileSystemEventHandler(OnFolderContentsUpdated);
                        fsw.Deleted -= new FileSystemEventHandler(OnFolderContentsUpdated);
                        fsw.Renamed -= new RenamedEventHandler(fsw_Renamed);

                        fsw.Dispose();
                        fsw = null;
                    }

                    if (Directory.Exists(m_strDirPath))
                    {
                        fsw = new FileSystemWatcher(m_strDirPath);
                        fsw.EnableRaisingEvents = true;
                        fsw.Created += new FileSystemEventHandler(OnFolderContentsUpdated);
                        fsw.Deleted += new FileSystemEventHandler(OnFolderContentsUpdated);
                        fsw.Renamed += new RenamedEventHandler(fsw_Renamed);
                        //fsw.Changed += new FileSystemEventHandler(OnFolderContentsUpdated);
                    }

                    bckPaths.Push(m_strPrevDirPath);

                    Explore(false);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.Message);

                    this.Items.Clear();
                    CreateMessageRow(Translator.Translate("TXT_FOLDERNOTACCESSIBLE"));
                }
                finally
                {
                    ignoreEvents = false;
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public string SearchPattern { get; set; }

        public void EnableAutoRefresh(bool enable)
        {
            if (fsw != null)
            {
                fsw.EnableRaisingEvents = enable;
            }
        }

        void fsw_Renamed(object sender, RenamedEventArgs e)
        {
            OnFolderContentsUpdated(sender, e);
        }

        void OnFolderContentsUpdated(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath != null)
            {
                string folder = System.IO.Path.GetDirectoryName(e.FullPath);
                if (string.Compare(folder, AppConfig.InstallationPath, true) == 0)
                {
                    // Ignore any changes related to DB3 journal files located in the installation path.
                    if (e.FullPath.ToLowerInvariant().EndsWith("db3-journal"))
                        return;
                }
            }

            if (InvokeRequired)
            {
                Invoke(new FileSystemEventHandler(OnFolderContentsUpdated), sender, e);
                return;
            }

            Logger.LogTrace("OnFolderContentsUpdated: " + e.ToString());
            _delayedExplore.Stop();

            try
            {
                if (sender is FileSystemWatcher)
                {
                    _delayedExplore.Start();
                }
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        void _delayedRenameTimer_Tick(object sender, EventArgs e)
        {
            _delayedRenameTimer.Stop();
            Rename();
        }

        void _delayedExplore_Tick(object sender, EventArgs e)
        {
            _delayedExplore.Stop();

            try
            {
                Explore(_pathToSelect == null);
            }
            finally
            {
                ignoreEvents = false;
            }
        }
        #endregion

        #region Methods
        public new void Clear()
        {
            this.Items.Clear();
        }
        #endregion

        #region Construction
        /// <summary>
        /// Default contructor.
        /// </summary>
        public OPMShellListView() : base()
		{
            this.SearchPattern = null;

            base.SmallImageList = m_ilDirListManager.ImageList;

            colName.Text = "TXT_FILENAME";
            this.Columns.Add(colName);

            colSize.Text = "TXT_FILESIZE";
            this.Columns.Add(colSize);

            colLastAccess.Text = "TXT_LASTCHANGEDATE";
            this.Columns.Add(colLastAccess);
            this.ShowLastAccess = true;

            colAttr.Text = "TXT_ATTRIBUTES";
            this.Columns.Add(colAttr);
            this.ShowAttributes = true;

            int i = 0;
            colName.DisplayIndex = i++;
            colSize.DisplayIndex = i++;
            colLastAccess.DisplayIndex = i++;
            colAttr.DisplayIndex = i++;

            //this.ListViewItemSorter = new Sorter(colName.Index);
            
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.DoubleClick += new EventHandler(this.OnDoubleClick);
            
            //this.KeyDown += new KeyEventHandler(this.OnKeyDown);
            this.PreviewKeyDown += new PreviewKeyDownEventHandler(OnPreviewKeyDown);

            this.SelectedIndexChanged += new EventHandler(this.OnSelectedItemChanged);

            this.HandleCreated += new EventHandler(OPMShellListView_HandleCreated);
            this.HandleDestroyed += new EventHandler(OPMShellListView_HandleDestroyed);

            this.LabelEdit = true;
            this.BeforeLabelEdit += new LabelEditEventHandler(OnCellBeginEdit);
            this.AfterLabelEdit += new LabelEditEventHandler(OnCellEndEdit);

            this.Resize += new EventHandler(OPMShellListView_Resize);

            _delayedSelectionTimer = new System.Windows.Forms.Timer();
            _delayedSelectionTimer.Interval = 100;
            _delayedSelectionTimer.Tick += new EventHandler(_delayedSelectionTimer_Tick);
            _delayedSelectionTimer.Enabled = false;

            _delayedExplore = new System.Windows.Forms.Timer();
            _delayedExplore.Interval = 200;
            _delayedExplore.Tick += new EventHandler(_delayedExplore_Tick);
            _delayedExplore.Enabled = false;

            _delayedRenameTimer = new System.Windows.Forms.Timer();
            _delayedRenameTimer.Interval = 500;
            _delayedRenameTimer.Tick += _delayedRenameTimer_Tick;
            _delayedRenameTimer.Enabled = false;


            ThemeManager.SetFont(this, FontSizes.Normal);
        }

        void OPMShellListView_Resize(object sender, EventArgs e)
        {
            colSize.Width = 75;
            colAttr.Width = 65;
            colLastAccess.Width = 160;
            colName.Width = this.EffectiveWidth - colSize.Width - 
                (ShowLastAccess ? colLastAccess.Width : 0) - 
                (ShowAttributes ? colAttr.Width : 0);
        }

        #endregion

        #region Items rename

        public void Rename()
        {
            if (this.SelectedItems != null && this.SelectedItems.Count == 1)
            {
                ListViewItem lvi = this.SelectedItems[0];
                lvi.BeginEdit();
            }
        }

        public bool IsInEditMode { get; private set; }

        void OnCellBeginEdit(object sender, LabelEditEventArgs e)
        {
            e.CancelEdit = true;

            if (this.SelectedItems != null && this.SelectedItems.Count == 1)
            {
                string initialName = this.SelectedItems[0].Text;
                e.CancelEdit = (initialName == PathUtils.ParentDir);
            }

            this.IsInEditMode = !e.CancelEdit;
        }

        void OnCellEndEdit(object sender, LabelEditEventArgs e)
        {
            string oldPath = "";
            string finalPath = "";

            try
            {
                string newName = e.Label;
                if (e.CancelEdit || string.IsNullOrEmpty(newName))
                    return;

                string fsi = this.Items[e.Item].Tag as string;
                if (fsi == null)
                    return;

                e.CancelEdit = true;
                string initialName = string.Empty;
                ListViewItem item = null;

                if (this.SelectedItems != null && this.SelectedItems.Count == 1)
                {
                    item = this.SelectedItems[0];
                    initialName = this.SelectedItems[0].Text;
                    e.CancelEdit = (initialName == PathUtils.ParentDir);
                }

                oldPath = fsi;
                finalPath = System.IO.Path.Combine(m_strDirPath, newName);

                if (String.Equals(finalPath, fsi, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (Directory.Exists(fsi))
                {
                    // Renaming a folder
                    Directory.Move(fsi, finalPath);
                }
                else if (File.Exists(fsi))
                {
                    List<String> linkedFiles = null;

                    if (QueryLinkedFiles != null)
                    {
                        linkedFiles = QueryLinkedFiles(oldPath, FileTaskType.Move);
                    }

                    if (linkedFiles != null)
                    {
                        string oldFileName = System.IO.Path.GetFileNameWithoutExtension(oldPath);
                        string newFileName = System.IO.Path.GetFileNameWithoutExtension(finalPath);

                        foreach (string linkedFile in linkedFiles)
                        {
                            string linkedFileName = System.IO.Path.GetFileName(linkedFile).Replace(oldFileName, newFileName);
                            string linkedFileFinalPath = System.IO.Path.Combine(m_strDirPath, linkedFileName);

                            if (File.Exists(linkedFile))
                                File.Move(linkedFile, linkedFileFinalPath);
                        }
                    }

                    File.Move(oldPath, finalPath);
                }

                item.Selected = item.Focused = true;

                if (ItemRenamed != null)
                {
                    ItemRenamed(finalPath);
                }

                _pathToSelect = finalPath;
            }
            catch (Exception ex)
            {
                ErrorDispatcher.DispatchError(ex.Message, false);
                _pathToSelect = oldPath;
            }
            finally
            {
                this.IsInEditMode = false;
            }
        }
        #endregion

        #region Event handlers
        void OPMShellListView_HandleCreated(object sender, EventArgs e)
        {
            bckPaths.Clear();
            fwdPaths.Clear();

            EventDispatch.RegisterHandler(this);

            if (ShowAttributes == false)
                this.Columns.Remove(colAttr);

            if (ShowLastAccess == false)
                this.Columns.Remove(colLastAccess);
        }

        void OPMShellListView_HandleDestroyed(object sender, EventArgs e)
        {
            EventDispatch.UnregisterHandler(this);
        }

        /// <summary>
        /// Occurs when a key was pressed.
        /// </summary>
		//private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
            if (!this.IsInEditMode)
            {
	            if (e.Modifiers == Keys.None)
	            {
				    if (e.KeyCode == Keys.Back && !IsInDriveRoot)
				    {
					    this.Path = PathUtils.ParentDir;
					    //e.Handled = true;
				    }
	                else if (e.KeyCode == Keys.Enter)
	                {
	                    OnDoubleClick(sender, e);
	                }
	                else if (e.KeyCode == Keys.F2)
	                {
	                    Rename();
	                }
	                //else if (e.KeyCode == Keys.Escape)
	                //{
	                //    CancelRename();
	                //}
                }
            }
		}

        void _delayedSelectionTimer_Tick(object sender, EventArgs e)
        {
            _delayedSelectionTimer.Stop();

            if (this.SelectedItems == null || this.SelectedItems.Count <= 0)
            {
                OnSelectFile(string.Empty);
                return;
            }

            if (this.SelectedItems.Count > 1)
            {
                OnSelectMultipleItems();
            }
            else
            {
                ListViewItem item = this.SelectedItems[0];

                string fsi = item.Tag as string;
                if (fsi != null)
                {
                    if (File.Exists(fsi))
                    {
                        OnSelectFile(fsi);
                    }
                    else if (Directory.Exists(fsi))
                    {
                        OnSelectDirectory(fsi);
                    }
                }
            }
        }

        void OnSelectedItemChanged(object sender, EventArgs e)
        {
            if (ignoreEvents)
                return;

            _delayedSelectionTimer.Stop();
            _delayedSelectionTimer.Start();
        }

        /// <summary>
        /// Occurs when a file item was double clicked.
        /// </summary>
        private void OnDoubleClickFile(string strPath)
        {
            if (DoubleClickFile != null)
            {
                DoubleClickFileEventArgs eventArgs = new DoubleClickFileEventArgs();
                if (File.Exists(strPath) || Directory.Exists(strPath))
                {
                    eventArgs.m_strPath = strPath;
                }
                DoubleClickFile(this, eventArgs);
            }
        }

        /// <summary>
        /// Occurs when a folder item was double clicked.
        /// </summary>
        private void OnDoubleClickDirectory(string strPath)
        {
            if (DoubleClickDirectory != null)
            {
                DoubleClickDirectoryEventArgs eventArgs = new DoubleClickDirectoryEventArgs();
                if (File.Exists(strPath) || Directory.Exists(strPath))
                {
                    eventArgs.m_strPath = strPath;
                }
                DoubleClickDirectory(this, eventArgs);
            }
        }

        /// <summary>
        /// Occurs when a file item was selected.
        /// </summary>
        private void OnSelectFile(string strPath)
        {
            if (SelectFile != null)
            {
                SelectFileEventArgs eventArgs = new SelectFileEventArgs();
                if (File.Exists(strPath) || Directory.Exists(strPath))
                {
                    eventArgs.m_strPath = strPath;
                }
                SelectFile(this, eventArgs);
            }
        }

        /// <summary>
        /// Occurs when a folder item was selected.
        /// </summary>
        private void OnSelectDirectory(string strPath)
        {
            if (SelectDirectory != null)
            {
                SelectDirectoryEventArgs eventArgs = new SelectDirectoryEventArgs();
                if (File.Exists(strPath) || Directory.Exists(strPath))
                {
                    eventArgs.m_strPath = strPath;                    
                }

                SelectDirectory(this, eventArgs);
            }
        }

        /// <summary>
        /// Occurs when multiple items were selected.
        /// </summary>
		private void OnSelectMultipleItems()
		{
            if (SelectMultipleItems != null)
            {
                SelectMultipleItemsEventArgs eventArgs = new SelectMultipleItemsEventArgs();
                eventArgs.m_strPaths = new List<string>(SelectedPaths);

                SelectMultipleItems(this, eventArgs);
            }
		}
        #endregion

        #region Implementation
        [EventSink(EventNames.PerformTranslation)]
        public void PerformTranslation()
        {
            colName.Text = Translator.Translate("TXT_FILENAME");
            colLastAccess.Text = Translator.Translate("TXT_LASTCHANGEDATE");
            colSize.Text = Translator.Translate("TXT_FILESIZE");
            colAttr.Text = Translator.Translate("TXT_ATTRIBUTES");
        }

        [EventSink(EventNames.RefreshItems)]
        public void RefreshItems(string[] items)
        {
            if (items != null)
            {
                foreach (string item in items)
                {
                    RefreshItem(item);
                }
            }
        }

        protected override void OnThemeUpdatedInternal()
        {
            base.OnThemeUpdatedInternal();
            foreach (ListViewItem item in Items)
            {
                string fsi = item.Tag as string;
                bool isFile = (string.IsNullOrEmpty(fsi) == false && File.Exists(fsi));
                item.BackColor = ThemeManager.BackColor;
                item.ForeColor = isFile ? ThemeManager.ForeColor : ThemeManager.HighlightColor;
                item.Font = isFile ? ThemeManager.NormalFont : ThemeManager.NormalBoldFont;
            }
        }

        public void RefreshItem(string item)
        {
            foreach (ListViewItem row in this.Items)
            {
                string fsi = (row.Tag as string);
                if (fsi != null && string.Compare(fsi, item, true) == 0)
                {
                    row.SubItems[colAttr.Index].Text = BuildAttributes(fsi);
                    break;
                }
            }
        }

        public void RefreshList(bool keepSelection)
        {
            try
            {
                Explore(keepSelection);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.Message);

                this.Items.Clear();
                CreateMessageRow(Translator.Translate("TXT_FOLDERNOTACCESSIBLE"));
            }
            finally
            {
                ignoreEvents = false;
            }
        }

        private void ClearCurrentContents()
        {
            Clear();
            m_ilDirListManager.Clear();
        }

        /// <summary>
        /// Explore the current folder.
        /// </summary>
        private delegate void ExploreHandler(bool keepSelection);
        private void Explore(bool keepSelection)
		{
            if (InvokeRequired)
            {
                Invoke(new ExploreHandler(Explore), keepSelection);
                return;
            }

            List<string> selection = new List<string>();
            if (keepSelection)
            {
                selection.AddRange(SelectedPaths);
            }

            int selIdx = 0;
            try
            {
                selIdx = this.SelectedIndices[0];
            }
            catch
            {
                selIdx = 0;
            }

            lock (syncRoot)
            {
                try
                {
                    ignoreEvents = true;

                    bool isUncPathRoot = false;

                    CursorHelper.ShowWaitCursor(this, true);

                    // Check if current path is valid.
                    if (!Directory.Exists(m_strDirPath))
                    {
                        m_strDirPath = FindFirstUsablePath(m_strDirPath, ref isUncPathRoot);
                    }

                    AppConfig.LastExploredFolder = m_strDirPath;

                    if (!isUncPathRoot && PathUtils.IsRootPath(m_strDirPath))
                    {
                        m_strDirPath = m_strDirPath.TrimEnd(string.Copy(PathUtils.DirectorySeparator).ToCharArray()) + PathUtils.DirectorySeparator;
                    }

                    ShareCollection shares = null;
                    List<string> dirs = null;
                    List<string> files = null;

                    if (isUncPathRoot)
                    {
                        try
                        {
                            shares = ShareCollection.GetShares(m_strDirPath);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            dirs = PathUtils.EnumDirectories(m_strDirPath);
                        }
                        catch { }

                        try
                        {
                            files = PathUtils.EnumFilesUsingMultiFilter(m_strDirPath, this.SearchPattern);
                        }
                        catch { }
                    }

                    ClearCurrentContents();

                    CreateParentFolderRow();

                    if (isUncPathRoot && shares != null)
                    {
                        foreach(Share share in shares)
                        {
                            if (!share.IsFileSystem)
                                continue;

                            if (Directory.Exists(share.Root.FullName) == false)
                                continue;

                            CreateNewRow(share.Root.FullName);
                        }
                    }
                    else
                    {
                        if (dirs != null)
                        {
                            foreach (string dir in dirs)
                            {
                                if (Directory.Exists(dir) == false)
                                    continue;

                                CreateNewRow(dir);
                                int tt = 0;
                            }
                        }

                        if (files != null)
                        {
                            foreach (string file in files)
                            {
                                if (File.Exists(file) == false)
                                    continue;

                                CreateNewRow(file);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                finally
                {
                    CursorHelper.ShowWaitCursor(this, false);
                    ignoreEvents = true;
                }

                if (this.Items.Count <= 0)
                {
                    CreateMessageRow(string.Empty);
                    CreateMessageRow(Translator.Translate("TXT_FOLDERNOTACCESSIBLE"));
                }
            }

            this.Sort();

            //ClearSelection();
            this.SelectedItems.Clear();

            bool selectedSomething = false;
            if (keepSelection || _pathToSelect != null)
            {
                if ((selection != null && selection.Count > 0) || _pathToSelect != null)
                {
                    bool focusSet = false;

                    foreach (ListViewItem item in Items)
                    {
                        string path = item.Tag as string;
                        if (selection.Contains(path) || (string.Compare(_pathToSelect, path, true) == 0))
                        {
                            if (!focusSet)
                            {
                                item.Focused = true;
                                focusSet = true;
                            }

                            item.Selected = true;
                            EnsureVisible(item.Index);
                            selectedSomething = true;
                        }
                    }
                }
            }
            else if (m_strPrevDirPath != null && m_strPrevDirPath.Length > 0)
            {
                int lastVisibleIndex = 0;
                foreach (ListViewItem item in this.Items)
                {
                    string path = item.Tag as string;
                    if (string.IsNullOrEmpty(path) == false)
                    {
                        item.Selected = false;

                        if (string.Compare(path, m_strPrevDirPath, false) == 0)
                        {
                            item.Selected = true;
                            item.Focused = true;

                            lastVisibleIndex = item.Index;
                           
                            break;
                        }
                    }
                }

                EnsureVisible(lastVisibleIndex);
                selectedSomething = true;
            }

            if (!selectedSomething)
            {
                if (selIdx < 0)
                    selIdx = 0;
                if (selIdx >= this.Items.Count)
                    selIdx = this.Items.Count - 1;

                this.Items[selIdx].Selected = true;
                this.Items[selIdx].Focused = true;
                EnsureVisible(selIdx);
            }

            this.Select();
            this.Focus();
		}

        public void ExploreBack()
        {
            string s = null;

            try
            {
                s = bckPaths.Pop();
            }
            catch
            {
                s = null;
            }

            if (!string.IsNullOrEmpty(s))
            {
                fwdPaths.Push(m_strDirPath);
                m_strDirPath = s;

                try
                {
                    Explore(false);
                }
                finally
                {
                    ignoreEvents = false;
                }

            }
        }

        public void ExploreForward()
        {
            string s = null;

            try
            {
                s = fwdPaths.Pop();
            }
            catch
            {
                s = null;
            }

            if (!string.IsNullOrEmpty(s))
            {
                bckPaths.Push(m_strDirPath);
                m_strDirPath = s;

                try
                {
                    Explore(false);
                }
                finally
                { 
                    ignoreEvents = false;
                }
            }

        }



        public string ExploreBackTarget
        {
            get
            {
                return (bckPaths.Count > 0) ? bckPaths.Peek() : string.Empty;
            }
        }

        public string ExploreForwardTarget
        {
            get
            {
                return (fwdPaths.Count > 0) ? fwdPaths.Peek() : string.Empty;
            }
        }

        public string ParentFolderTarget
        {
            get
            {
                if (!IsInDriveRoot)
                {
                    string parent = System.IO.Path.GetDirectoryName(m_strDirPath);
                    return parent;
                }

                return string.Empty;
            }
        }

        public bool IsInDriveRoot
        {
            get
            {
                return PathUtils.IsRootPath(m_strDirPath);
            }
        }

        /// <summary>
        /// Occurs when the item was double-clicked.
        /// </summary>
		private void OnDoubleClick(object sender, System.EventArgs e)
		{
            if (ignoreEvents ||
                this.SelectedItems == null ||
                this.SelectedItems.Count < 1) 
                return;

            if (LaunchMultipleItems == null || !LaunchMultipleItems(sender, e))
            {
                string fsi = this.SelectedItems[0].Tag as string;
                if (fsi != null)
                {
                    if (Directory.Exists(fsi))
                    {
                        this.Path = fsi;
                        OnDoubleClickDirectory(fsi);
                    }
                    else if (File.Exists(fsi))
                    {
                        OnDoubleClickFile(fsi);
                    }
                }
            }
		}

        /// <summary>
        /// Converts the given path (that can contain invalid parts)
        /// to a physically existing, valid path.
        /// </summary>
        /// <param name="path">the given path</param>
        /// <returns>a physically existing, valid path</returns>
        private string FindFirstUsablePath(string path, ref bool isUNCPathRoot)
        {
            string usablePath = path;

            if (!Directory.Exists(path) && path.StartsWith("\\\\"))
            {
                isUNCPathRoot = true;
            }
            else
            {
                isUNCPathRoot = false;

                if (Directory.Exists(path) == false)
                {
                    string parent = System.IO.Path.GetDirectoryName(path);
                    usablePath = FindFirstUsablePath(parent, ref isUNCPathRoot);

                    if (string.IsNullOrEmpty(usablePath))
                        usablePath = Environment.SystemDirectory;
                }
            }

            return usablePath;
        }
        #endregion

        public void JumpToItem(string itemPath, bool launchItem)
        {
            try
            {
                string pathToOpen = System.IO.Path.GetDirectoryName(itemPath);
                //string itemName = System.IO.Path.GetFileName(itemPath);

                if (Directory.Exists(pathToOpen))
                {
                    Path = pathToOpen;
                }

                ignoreEvents = true;

                foreach (ListViewItem item in this.Items)
                {
                    string fsi = item.Tag as string;
                    if (fsi != null && string.Compare(fsi, itemPath, true) == 0)
                    {
                        this.Select();
                        this.Focus();
                        this.SelectedItems.Clear();

                        item.Selected = true;
                        item.EnsureVisible();

                        OnSelectFile(itemPath);

                        if (launchItem)
                        {
                            OnDoubleClickFile(itemPath);
                        }

                        break;
                    }
                }
            }
            finally
            {
                ignoreEvents = false;
            }
        }

        public void OpenItem(string itemPath)
        {
            if (Directory.Exists(itemPath))
            {
                Path = itemPath;
            }
            else if (File.Exists(itemPath))
            {
                JumpToItem(itemPath, true);
            }
        }

        public void CreateMessageRow(string msg)
        {
            ListViewItem item = new ListViewItem(msg);
            item.Tag = null;
            this.Items.Add(item);
        }

        public void CreateParentFolderRow()
        {
            if (!PathUtils.IsRootPath(Path))
            {
                string parent = System.IO.Path.GetDirectoryName(Path);
                if (parent != null)
                {
                    List<string> data = new List<string>();

                    data.Add(PathUtils.ParentDir);
                    
                    data.Add("[ DIR ]");

                    if (ShowLastAccess)
                        data.Add(BuildLastAccessTime(parent));

                    if (ShowAttributes)
                        data.Add(BuildAttributes(parent));

                    ListViewItem item = new ListViewItem(data.ToArray());
                    item.ImageKey = m_ilDirListManager.GetImageKey(parent);
                    item.Tag = parent;
                    item.BackColor = ThemeManager.BackColor;
                    item.ForeColor = ThemeManager.ForeColor;
                    item.Font = ThemeManager.NormalBoldFont;

                    this.Items.Add(item);
                }
            }
        }

        public void CreateNewRow(string fsi)
        {
            string strLen = "[ DIR ]";
            Color c = ThemeManager.SelectedColor;

            bool isFile = false;
            if (File.Exists(fsi))
            {
                long len = new FileInfo(fsi).Length;
                float lenKB = (float)len / (float)(1024);
                float lenMB = (float)len / (float)(1024 * 1024);
                float lenGB = (float)len / (float)(1024 * 1024 * 1024);

                if (lenGB > 2)
                    strLen = $"{lenGB:0.00} GB";
                else if (lenMB > 2)
                    strLen = $"{lenMB:0.00} MB";
                else if (lenKB > 2)
                    strLen = $"{lenKB:0.00} KB";
                else
                    strLen = $"{len} B";

                isFile = true;
            }

            List<string> data = new List<string>();

            data.Add(BuildDisplayName(fsi));

            data.Add(strLen);

            if (ShowLastAccess)
                data.Add(BuildLastAccessTime(fsi));

            if (ShowAttributes)
                data.Add(BuildAttributes(fsi));

            ListViewItem item = new ListViewItem(data.ToArray());
            item.BackColor = ThemeManager.BackColor;
            item.ForeColor = isFile ? ThemeManager.ForeColor : ThemeManager.HighlightColor;
            item.ImageKey = m_ilDirListManager.GetImageKey(fsi);
            item.Tag = fsi;
            item.Font = isFile ? ThemeManager.NormalFont : ThemeManager.NormalBoldFont;

            this.Items.Add(item);
            
        }

        private string BuildDisplayName(string fsi)
        {
            if (Directory.Exists(fsi))
                return PathUtils.GetDirectoryTitle(fsi);

            if (File.Exists(fsi))
            {
                if (QueryDisplayName != null)
                    return QueryDisplayName(fsi);

                return System.IO.Path.GetFileName(fsi);
            }

            return string.Empty;
        }

        private string BuildLastAccessTime(string fsi)
        {
            try
            {
                DateTime dt = File.GetLastWriteTime(fsi);
                return StringUtils.BuildTimeString(dt);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string BuildAttributes(string fsi)
        {
            string strAttr = "";

            if (PathUtils.ObjectHasAttribute(fsi, FileAttributes.ReadOnly))
                strAttr += "R";
            else
                strAttr += "-";
            if (PathUtils.ObjectHasAttribute(fsi, FileAttributes.Archive))
                strAttr += "A";
            else
                strAttr += "-";
            if (PathUtils.ObjectHasAttribute(fsi, FileAttributes.Hidden))
                strAttr += "H";
            else
                strAttr += "-";
            if (PathUtils.ObjectHasAttribute(fsi, FileAttributes.System))
                strAttr += "S";
            else
                strAttr += "-";

            return strAttr;
        }

        string _pathToSelect = null;

        public void CreateNewFolder()
        {
            _pathToSelect = null;

            try
            {
                if (Directory.Exists(Path))
                {
                    string newName = StringUtils.GetRandomNewFolderName();

                    string newPath = System.IO.Path.Combine(Path, newName);
                    Directory.CreateDirectory(newPath);
                    if (Directory.Exists(newPath))
                    {
                        _pathToSelect = newPath;
                        _delayedRenameTimer.Start();
                    }
                }
            }
            catch
            {
                _pathToSelect = null;
            }
        }
    }

    #endregion

    #region Helper classes
    public class CursorHelper
    {
        public static void ShowWaitCursor(Control ctl, bool show)
        {
            ctl.Cursor = (show) ? Cursors.WaitCursor : Cursors.Default;
            //Application.DoEvents();
        }
    }
    
    /// <summary>
    /// Class used to compare list entries
    /// </summary>
	public class Sorter : IComparer
	{
        int _col = 0;

        public Sorter(int column)
		{
            _col = column;
		}

		public int Compare(object x, object y)
		{
            try
            {
                ListViewItem row1 = x as ListViewItem;
                ListViewItem row2 = y as ListViewItem;

                if (row1 == null && row2 == null)
                    return 0;
                else if (row1 == null)
                    return 1;
                else if (row2 == null)
                    return -1;

                string fsi1 = row1.Tag as string;
                string fsi2 = row2.Tag as string;

                if (fsi1 == null && fsi2 == null)
                    return 0;
                else if (fsi1 == null)
                    return 1;
                else if (fsi2 == null)
                    return -1;

                bool isDir1 = Directory.Exists(fsi1);
                bool isDir2 = Directory.Exists(fsi2);

                if (isDir1 != isDir2)
                {
                    if (isDir1)
                        return -1;
                    else if (isDir2)
                        return 1;
                }

                return String.Compare(fsi1, fsi2, true);
            }
            catch
            {
                return 0;
            }

		}
	}

    #region Event args classes
	public class DoubleClickServerEventArgs : EventArgs
	{
		public string m_strPath = "";
	}
	
    public class DoubleClickDirectoryEventArgs : EventArgs
	{
		public string m_strPath = "";
	}
	
    public class DoubleClickFileEventArgs : EventArgs
	{
		public string m_strPath = "";
	}
	
    public class SelectServerEventArgs : EventArgs
	{
		public string m_strPath = "";
	}
	
    public class SelectDirectoryEventArgs : EventArgs
	{
		public string m_strPath = "";
	}
	
    public class SelectFileEventArgs : EventArgs
	{
		public string m_strPath = "";
	}

    public class SelectMultipleItemsEventArgs : EventArgs
    {
        public List<string> m_strPaths;
    }
    #endregion

    #endregion
}
