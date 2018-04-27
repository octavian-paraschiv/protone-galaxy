using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using OPMedia.Runtime.ProTONE.OnlineMediaContent;
using OPMedia.UI.Controls;
using OPMedia.UI.Menus;
using OPMedia.Core.TranslationSupport;
using OPMedia.UI.Themes;
using OPMedia.Core;
using LocalEventNames = OPMedia.UI.ProTONE.GlobalEvents.EventNames;
using OPMedia.UI.ProTONE.Properties;
using System.Diagnostics;
using OPMedia.Runtime.ProTONE.Configuration;

namespace OPMedia.UI.ProTONE.Controls.OnlineMediaBrowser
{
    public partial class DeezerTrackBrowserCtl : MediaBrowserPage
    {
        public DeezerTrackBrowserCtl()
        {
            InitializeComponent();
            _searchType = OnlineMediaSource.Deezer;

            lvTracks.MultiSelect = true;
            lvTracks.Resize += OnListResize;
            lvTracks.SelectedIndexChanged += OnListSelectedIndexChanged;
            
            OPMContextMenuStrip cms = BuildCommonMenuStrip(true);

            OPMMenuStripSeparator sep = new OPMMenuStripSeparator();
           cms.Items.Add(sep);
            
            OPMToolStripMenuItem tsmi = new OPMToolStripMenuItem();

            Bitmap searchIcon = OPMedia.UI.Properties.Resources.Search16;
            searchIcon.MakeTransparent(Color.Magenta);

            tsmi.Click += new EventHandler(OnMenuClick);
            tsmi.Text = Translator.Translate("TXT_LOOKUP_THIS_ARTIST");
            tsmi.Tag = "LookupDeezerArtist";
            tsmi.Image = searchIcon;
            cms.Items.Add(tsmi);

            tsmi = new OPMToolStripMenuItem();
            tsmi.Click += new EventHandler(OnMenuClick);
            tsmi.Text = Translator.Translate("TXT_LOOKUP_THIS_ALBUM");
            tsmi.Tag = "LookupDeezerAlbum";
            tsmi.Image = searchIcon;
            cms.Items.Add(tsmi);

            tsmi = new OPMToolStripMenuItem();
            tsmi.Click += new EventHandler(OnMenuClick);
            tsmi.Text = Translator.Translate("TXT_LOOKUP_THIS_TRACK");
            tsmi.Tag = "LookupDeezerTrack";
            tsmi.Image = searchIcon;
            cms.Items.Add(tsmi);

            sep = new OPMMenuStripSeparator();
            cms.Items.Add(sep);

            tsmi = new OPMToolStripMenuItem();
            tsmi.Click += new EventHandler(OnMenuClick);
            tsmi.Text = "www.deezer.com";
            tsmi.Tag = "OpenDeezerPage";
            tsmi.Image = searchIcon;
            cms.Items.Add(tsmi);

            tsmi = new OPMToolStripMenuItem();
            tsmi.Click += new EventHandler(OnMenuClick);
            tsmi.Text = Translator.Translate("TXT_LOOKUP_MY_PLAYLISTS");
            tsmi.Tag = "LookupMyPlaylists";
            tsmi.Image = searchIcon;
            cms.Items.Add(tsmi);

            lvTracks.ContextMenuStrip = cms;
        }

        protected override void HandleAction(string act)
        {
            var selItems = this.SelectedItems;
            DeezerTrackItem selItem = null;

            if (selItems != null && selItems.Count > 0)
                selItem = selItems[0] as DeezerTrackItem;

            string search = string.Empty;

            switch (act)
            {
                case "LookupDeezerArtist":
                    search = string.Format("artist:\"{0}\"", selItem.Artist.ToLowerInvariant());
                    break;

                case "LookupDeezerAlbum":
                    search = string.Format("artist:\"{0}\" album:\"{1}\"", selItem.Artist.ToLowerInvariant(), selItem.Album.ToLowerInvariant());
                    break;

                case "LookupDeezerTrack":
                    search = string.Format("artist:\"{0}\" track:\"{1}\"", selItem.Artist.ToLowerInvariant(), selItem.Title.ToLowerInvariant());
                    break;

                case "LookupMyPlaylists":
                    EventDispatch.DispatchEvent(LocalEventNames.StartDeezerSearch, act);
                    return;

                case "OpenDeezerPage":
                    Process.Start("http://www.deezer.com");
                    return;
            }

            if (string.IsNullOrEmpty(search) == false)
                EventDispatch.DispatchEvent(LocalEventNames.StartDeezerSearch, search);
        }
                

        private void OnListSelectedIndexChanged(object sender, EventArgs e)
        {
            base.SelectedItems.Clear();

            foreach (ListViewItem lvi in lvTracks.SelectedItems)
            {
                DeezerTrackItem dti = lvi.Tag as DeezerTrackItem;
                if (dti != null)
                    base.SelectedItems.Add(dti);
            }
        }

        private void OnListResize(object sender, EventArgs e)
        {
            AdjustColumns();
        }

        private void AdjustColumns()
        {
            colURL.Width = 150;
            int w = (lvTracks.EffectiveWidth - colURL.Width) / 3;
            colAlbum.Width = colArtist.Width = colName.Width = w;
        }

        protected override void DisplaySearchResultsInternal()
        {
            lvTracks.Items.Clear();

            if (this.Items != null)
            {
                foreach (var item in this.Items)
                {
                    DeezerTrackItem dzi = item as DeezerTrackItem;
                    if (dzi != null)
                    {
                        ListViewItem lvi = new ListViewItem(new string[]
                        {
                            string.Empty,
                            dzi.Title,
                            dzi.Artist,
                            dzi.Album,
                            dzi.Url
                        });

                        lvi.Tag = dzi;
                        lvTracks.Items.Add(lvi);
                    }
                }

                if (lvTracks.Items.Count > 0)
                {
                    lvTracks.Items[0].Selected = true;
                    lvTracks.Items[0].Focused = true;
                }
            }
        }

        public override bool PreValidateSearch(string search)
        {
            bool valid = base.PreValidateSearch(search);

            if (valid)
            {
                if (search.Contains(":") || search.Contains("\""))
                {
                    // We're trying to make use of advanced search filters.
                    // If the Deezer Searcher can build a non-empty filter from the user input, then we're OK.
                    DeezerJsonFilter filter = DeezerTrackSearcher.BuildFilterFromQuery(search);
                    valid = DeezerJsonFilter.IsNullOrEmpty(filter) == false;
                }
            }

            return valid;
        }

        public override string GetSearchBoxTip()
        {
            return Translator.Translate("TXT_DEEZERFILTER_HINT");
        }

        protected override List<string> GetSearchHistory()
        {
            return ProTONEConfig.Media_Browser_History_Deezer;
        }

        protected override void SetSearchHistory(List<string> history)
        {
            ProTONEConfig.Media_Browser_History_Deezer = history;
        }
    }
}
