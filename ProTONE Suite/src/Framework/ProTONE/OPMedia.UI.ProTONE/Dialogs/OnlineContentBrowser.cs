using OPMedia.Core;
using OPMedia.Runtime.ProTONE.OnlineMediaContent;
using OPMedia.UI.Dialogs;
using OPMedia.UI.ProTONE.Controls.OnlineMediaBrowser;
using OPMedia.UI.Themes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OPMedia.UI.ProTONE.Properties;
using OPMedia.Runtime.ProTONE.Configuration;
using OPMedia.Core.GlobalEvents;
using LocalEventNames = OPMedia.UI.ProTONE.GlobalEvents.EventNames;
using OPMedia.UI.Controls;

namespace OPMedia.UI.ProTONE.Dialogs
{
    public partial class OnlineContentBrowser : ThemeForm
    {
        CancellableWaitDialog _waitDialog = null;
        ManualResetEvent _searchCancelled = new ManualResetEvent(false);

        ImageList _ilImages = null;

        OPMToolTip _tt = null;

        public OnlineContentBrowser() : base("TXT_SELECT_ONLINE_MEDIA")
        {
            InitializeComponent();

            this.ShowInTaskbar = true;

            _ilImages = new ImageList();
            
            _ilImages.TransparentColor = Color.White;

            _ilImages.ColorDepth = ColorDepth.Depth32Bit;
            _ilImages.ImageSize = new Size(32, 32);

            _ilImages.Images.Add(OPMedia.UI.Properties.Resources.Favorites);
            _ilImages.Images.Add(Resources.Shoutcast);
            _ilImages.Images.Add(Resources.deezer);

            tabContentBrowser.ImageList = _ilImages;

            if (OnlineContentSearcher.IsSearchConfigValid(OnlineMediaSource.Internal) == false)
                tabContentBrowser.TabPages.Remove(tpLocalDatabase);
            else
                tpLocalDatabase.ImageIndex = 0;

            if (OnlineContentSearcher.IsSearchConfigValid(OnlineMediaSource.ShoutCast) == false)
                tabContentBrowser.TabPages.Remove(tpShoutcastDir);
            else
                tpShoutcastDir.ImageIndex = 1;

            if (OnlineContentSearcher.IsSearchConfigValid(OnlineMediaSource.Deezer) == false)
                tabContentBrowser.TabPages.Remove(tpDeezerContent);
            else
                tpDeezerContent.ImageIndex = 2;

            this.Shown += OnlineContentBrowser_Shown;

            tabContentBrowser.SelectedIndexChanged += new EventHandler(tabContentBrowser_SelectedIndexChanged);

            cmbSearch.TextChanged += new EventHandler(OnSearchTextChanged);

            _tt = new OPMToolTip();

            OnSearchTextChanged(null, null);
        }

        void OnSearchTextChanged(object sender, EventArgs e)
        {
            bool validText = false;

            MediaBrowserPage selectedPage = GetSelectedPage();
            if (selectedPage != null)
            {
                _tt.SetSimpleToolTip(cmbSearch, selectedPage.GetSearchBoxTip());
                validText = selectedPage.PreValidateSearch(cmbSearch.Text);
            }

            btnSearch.Enabled = validText;
            cmbSearch.BackColor = validText ? ThemeManager.WndValidColor : ThemeManager.ColorValidationFailed;
        }

        void tabContentBrowser_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSearchHistoryDropDown();

            cmbSearch.Select();
            cmbSearch.Focus();

            OnSearchTextChanged(null, null);
        }

        private void UpdateSearchHistoryDropDown()
        {
            string txt = cmbSearch.Text;

            try
            {
                cmbSearch.TextChanged -= new EventHandler(OnSearchTextChanged);

                MediaBrowserPage selectedPage = GetSelectedPage();
                if (selectedPage != null)
                {
                    cmbSearch.Items.Clear();
                    List<string> history = selectedPage.SearchHistory;
                    if (history != null && history.Count > 0)
                    {
                        history.ForEach((s) =>
                            {
                                // Don't add "LookupMyPlaylists" in the drop down history
                                // even when it comes from the persistence service
                                if (string.Compare(s, "LookupMyPlaylists", true) != 0)
                                    cmbSearch.Items.Add(s);

                            });
                    }

                    cmbSearch.SelectedIndex = -1;
                }
            }
            finally
            {
                cmbSearch.Text = txt;
                cmbSearch.TextChanged += new EventHandler(OnSearchTextChanged);
            }
        }

        private void OnlineContentBrowser_Shown(object sender, EventArgs e)
        {
            cmbSearch.Select();
            cmbSearch.Focus();
        }

        private MediaBrowserPage GetSelectedPage()
        {
            MediaBrowserPage selectedPage = null;

            if (tabContentBrowser.SelectedTab != null &&
                tabContentBrowser.SelectedTab.Controls != null &&
                tabContentBrowser.SelectedTab.Controls.Count > 0)
            {
                selectedPage = tabContentBrowser.SelectedTab.Controls[0] as MediaBrowserPage;
            }

            return selectedPage;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            MediaBrowserPage selectedPage = GetSelectedPage();
            if (selectedPage != null)
            {
                string search = cmbSearch.Text;

                if (selectedPage.PreValidateSearch(search))
                {
                    selectedPage.SearchCompleted -= SelectedPage_SearchCompleted;
                    selectedPage.SearchCompleted += SelectedPage_SearchCompleted;

                    _searchCancelled.Reset();

                    selectedPage.StartCancellableSearch(cmbSearch.Text, _searchCancelled);

                    ShowWaitDialog("Please wait for the search task to finish. You can press ESC to cancel the search.");
                }
            }
        }

        private void SelectedPage_SearchCompleted(object sender, EventArgs e)
        {
            MediaBrowserPage senderPage = sender as MediaBrowserPage;
            MediaBrowserPage selectedPage = GetSelectedPage();

            if (senderPage != null && senderPage == selectedPage)
            {
                senderPage.DisplaySearchResults();

                string s = cmbSearch.Text;

                // Don't add "LookupMyPlaylists" in the search history 
                if (string.Compare(s, "LookupMyPlaylists", true) != 0)
                {
                    if (senderPage.UpdateSearchHistory(s))
                        UpdateSearchHistoryDropDown();
                }
            }

            CloseWaitDialog();
        }

        private void ShowWaitDialog(string message)
        {
            CloseWaitDialog();

            _waitDialog = new CancellableWaitDialog();
            _waitDialog.FormClosed += new FormClosedEventHandler(_waitDialog_FormClosed);
            _waitDialog.ShowDialog(message, this);
        }

        void _waitDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_waitDialog != null && _waitDialog.EscapePressed)
            {
                _waitDialog.SetText("Please wait while cancelling the search task ...");
                _searchCancelled.Set();
            }
        }

        private void CloseWaitDialog()
        {
            if (_waitDialog != null)
            {
                _waitDialog.Close();
                _waitDialog = null;
            }
        }

        internal void RestorePosition()
        {
            this.Size = ProTONEConfig.OnlineContentBrowser_WindowSize;
            this.Location = ProTONEConfig.OnlineContentBrowser_WindowLocation;
        }

        internal void SavePosition()
        {
            ProTONEConfig.OnlineContentBrowser_WindowLocation = this.Location;
            ProTONEConfig.OnlineContentBrowser_WindowSize = this.Size;
        }

        [EventSink(LocalEventNames.StartDeezerSearch)]
        public void StartDeezerSearch(string search)
        {
            if (string.IsNullOrEmpty(search) == false)
                cmbSearch.Text = search;

            btnSearch_Click(this, EventArgs.Empty);
        }

        [EventSink(LocalEventNames.ManageOnlineContent)]
        public void ManageOnlineContent(List<OnlineMediaItem> onlineContent, bool doAdd)
        {
            var data = LocalDatabaseSearcher.LoadOnlineMediaData();

            foreach(OnlineMediaItem item in onlineContent)
            {
                if (doAdd)
                    data.SafeAddItem(item);
                else
                    data.SafeRemoveItem(item);
            }

            bool isSaved = LocalDatabaseSearcher.SaveOnlineMediaData(data);
            if (isSaved && !doAdd)
            {
                LocalDatabaseBrowserCtl localDbBrowser = GetSelectedPage() as LocalDatabaseBrowserCtl;
                if (localDbBrowser != null)
                    StartDeezerSearch(null);
            }
        }
    }

    

}
