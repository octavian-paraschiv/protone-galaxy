using OPMedia.Runtime.ProTONE.OnlineMediaContent;
using OPMedia.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OPMedia.Core.TranslationSupport;
using OPMedia.UI.ProTONE.Properties;
using OPMedia.Core;
using LocalEventNames = OPMedia.UI.ProTONE.GlobalEvents.EventNames;
using OPMedia.UI.Generic;
using OPMedia.UI.ProTONE.Dialogs;
using System.Windows.Forms;
using OPMedia.Core.Logging;

namespace OPMedia.UI.ProTONE.Controls.OnlineMediaBrowser
{
    public class MediaBrowserPage : OPMBaseControl
    {
        class MediaBrowserSearchParams
        {
            public OnlineContentSearchParameters SearchParams { get; set; }
            public ManualResetEvent AbortEvent { get; set; }
        }

        public List<OnlineMediaItem> SelectedItems { get; protected set; }
        public List<OnlineMediaItem> Items { get; protected set; }

        public event EventHandler SearchCompleted;

        private BackgroundWorker _bwSearch = new BackgroundWorker();

        protected OnlineMediaSource _searchType = OnlineMediaSource.Internal;

        protected OnlineContentSearchFilter SearchFilter { get; set; }

        public new OPMContextMenuStrip ContextMenuStrip { get; set; }

        public List<string> SearchHistory
        {
            get { return GetSearchHistory(); }
            set { SetSearchHistory(value); }
        }

        public MediaBrowserPage() : base()
        {
            this.SelectedItems = new List<OnlineMediaItem>();
            this.Items = new List<OnlineMediaItem>();

            this.SearchFilter = OnlineContentSearchFilter.Any;

            _bwSearch.WorkerSupportsCancellation = false;
            _bwSearch.WorkerReportsProgress = false;
            _bwSearch.DoWork += new DoWorkEventHandler(OnBackgroundSearch);
            _bwSearch.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnBackgroundSearchCompleted);
        }

        public void DisplaySearchResults()
        {
            DisplaySearchResultsInternal();
        }

        public bool UpdateSearchHistory(string lastSearchText)
        {
            if (Items == null || Items.Count < 1)
                // Unsuccesful search
                return false;

            return UpdateSearchHistoryInternal(lastSearchText);
        }

        public virtual bool PreValidateSearch(string search)
        {
            return string.IsNullOrEmpty(search) == false;
        }

        public void StartCancellableSearch(string search, ManualResetEvent abortEvent)
        {
            MediaBrowserSearchParams sp = new MediaBrowserSearchParams
            {
                AbortEvent = abortEvent,
                SearchParams = new OnlineContentSearchParameters
                {
                    Filter = this.SearchFilter,
                    SearchText = search
                }
            };
            _bwSearch.RunWorkerAsync(sp);
        }

        void OnBackgroundSearch(object sender, DoWorkEventArgs e)
        {
            MediaBrowserSearchParams sp = e.Argument as MediaBrowserSearchParams;
            if (sp != null)
                SearchInternal(sp.SearchParams, sp.AbortEvent);
        }

        void OnBackgroundSearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (SearchCompleted != null)
                SearchCompleted(this, EventArgs.Empty);
        }

        protected virtual void SearchInternal(OnlineContentSearchParameters searchParams, ManualResetEvent abortEvent)
        {
            this.Items.Clear();
            this.SelectedItems.Clear();

            if (searchParams.SearchText == "LookupMyPlaylists")
            {
                var playlists = OnlineContentSearcher.GetMyPlaylists(_searchType, abortEvent);
                if (playlists != null && playlists.Count > 0)
                {
                    OnlinePlaylist p = null;

                    MainThread.Send((c) =>
                        {
                            SelectOnlinePlaylistDlg dlg = new SelectOnlinePlaylistDlg(playlists);
                            if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
                                p = dlg.SelectedItem;
                        });

                    if (p != null)
                    {
                        var results = OnlineContentSearcher.ExpandOnlinePlaylist(_searchType, p, abortEvent);
                        if (results != null && results.Count > 0)
                            this.Items.AddRange(results);
                    }
                }
            }
            else
            {
                var results = OnlineContentSearcher.Search(_searchType, searchParams, abortEvent);
                if (results != null && results.Count > 0)
                    this.Items.AddRange(results);
            }
        }

        protected OPMContextMenuStrip BuildCommonMenuStrip(bool addToFav)
        {
            OPMContextMenuStrip cms = new OPMContextMenuStrip();

            OPMToolStripMenuItem tsmi = new OPMToolStripMenuItem();

            OPMMenuStripSeparator sep = new OPMMenuStripSeparator();

            tsmi.Click += new EventHandler(OnMenuClick);
            tsmi.Text = Translator.Translate("TXT_PLAY");
            tsmi.Tag = MediaBrowserAction.Play;
            tsmi.Image = ImageProcessing.Player16;
            cms.Items.Add(tsmi);

            tsmi = new OPMToolStripMenuItem();
            tsmi.Click += new EventHandler(OnMenuClick);
            tsmi.Text = Translator.Translate("TXT_ENQUEUE");
            tsmi.Tag = MediaBrowserAction.Enqueue;
            tsmi.Image = ImageProcessing.Player16;
            cms.Items.Add(tsmi);

            cms.Items.Add(sep);

            if (addToFav)
            {
                tsmi = new OPMToolStripMenuItem();
                tsmi.Click += new EventHandler(OnMenuClick);
                tsmi.Text = Translator.Translate("TXT_ADD_FAV_LIST");
                tsmi.Tag = MediaBrowserAction.AddFav;
                tsmi.Image = OPMedia.UI.Properties.Resources.Favorites16;
                cms.Items.Add(tsmi);
            }
            else
            {
                tsmi = new OPMToolStripMenuItem();
                tsmi.Click += new EventHandler(OnMenuClick);
                tsmi.Text = Translator.Translate("TXT_DEL_FAV_LIST");
                tsmi.Tag = MediaBrowserAction.DelFav;
                tsmi.Image = OPMedia.UI.Properties.Resources.Delete16;
                cms.Items.Add(tsmi);
            }



            return cms;
        }

        protected virtual void DisplaySearchResultsInternal()
        {
        }

        protected virtual bool UpdateSearchHistoryInternal(string lastSearchText)
        {
            try
            {
                List<string> history = new List<string>(SearchHistory);

                // Check whether it is already in the search history.
                // Ignore letter case.
                foreach (string s in history)
                {
                    if (string.Compare(s, lastSearchText, true) == 0)
                        return false;
                }

                if (history.Count >= 20)
                    history.RemoveAt(0);

                history.Add(lastSearchText);

                SearchHistory = history;

                return true;
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
        }

        
        protected void OnMenuClick(object sender, EventArgs e)
        {
            OPMToolStripMenuItem tsmi = sender as OPMToolStripMenuItem;
            if (tsmi != null)
            {
                string act = tsmi.Tag as string;
                if (string.IsNullOrEmpty(act) == false)
                {
                    var selItems = this.SelectedItems;
                    if (selItems != null && selItems.Count > 0)
                    {
                        switch (act)
                        {
                            case MediaBrowserAction.Play:
                                EventDispatch.DispatchEvent(LocalEventNames.LoadOnlineContent, selItems, false);
                                return;

                            case MediaBrowserAction.Enqueue:
                                EventDispatch.DispatchEvent(LocalEventNames.LoadOnlineContent, selItems, true);
                                return;

                            case MediaBrowserAction.AddFav:
                                EventDispatch.DispatchEvent(LocalEventNames.ManageOnlineContent, selItems, true);
                                return;

                            case MediaBrowserAction.DelFav:
                                EventDispatch.DispatchEvent(LocalEventNames.ManageOnlineContent, selItems, false);
                                return;
                        }
                    }

                    HandleAction(act);
                }
            }
        }

        protected virtual void HandleAction(string act)
        {
        }

        public virtual string GetSearchBoxTip()
        {
            return null;
        }

        protected virtual List<string> GetSearchHistory()
        {
            return null;
        }

        protected virtual void SetSearchHistory(List<string> history)
        {
        }
    }

    public class MediaBrowserAction
    {
        public const string Play = "Play";
        public const string Enqueue = "Enqueue";
        public const string AddFav = "AddFav";
        public const string DelFav = "DelFav";
    }
}
