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
using OPMedia.Runtime.ProTONE.Playlists;
using OPMedia.Core.TranslationSupport;
using OPMedia.UI.Controls;
using OPMedia.UI.Menus;
using System.Linq;
using OPMedia.UI.ProTONE.Properties;
using OPMedia.Runtime.ProTONE.Configuration;

namespace OPMedia.UI.ProTONE.Controls.OnlineMediaBrowser
{
    public partial class LocalDatabaseBrowserCtl : MediaBrowserPage
    {
        ImageList _ilImages = null;

        public LocalDatabaseBrowserCtl()
        {
            InitializeComponent();
            _searchType = OnlineMediaSource.Internal;

            lvRadioStations.MultiSelect = true;
            lvRadioStations.Resize += lvRadioStations_Resize;
            lvRadioStations.SelectedIndexChanged += LvRadioStations_SelectedIndexChanged;
            lvRadioStations.ContextMenuStrip = BuildCommonMenuStrip(false);

            _ilImages = new ImageList();
            _ilImages.TransparentColor = Color.White;
            _ilImages.ColorDepth = ColorDepth.Depth32Bit;
            _ilImages.ImageSize = new Size(16, 16);

            _ilImages.Images.Add(Resources.Shoutcast);
            _ilImages.Images.Add(Resources.deezer);

            lvRadioStations.SmallImageList = _ilImages;
        }

        private void LvRadioStations_SelectedIndexChanged(object sender, EventArgs e)
        {
            base.SelectedItems.Clear();

            foreach (ListViewItem lvi in lvRadioStations.SelectedItems)
            {
                OnlineMediaItem rs = lvi.Tag as OnlineMediaItem;
                if (rs != null)
                    base.SelectedItems.Add(rs);
            }
        }

        private void lvRadioStations_Resize(object sender, EventArgs e)
        {
            AdjustColumns();
        }

        private void AdjustColumns()
        {
            colBitrate.Width = 50;
            colGenre.Width = colType.Width = 100;
            colIcon.Width = 20;

            int w = lvRadioStations.EffectiveWidth - (colGenre.Width + colType.Width + colBitrate.Width + colIcon.Width);

            colName.Width = colAlbum.Width = colArtist.Width = colURL.Width = w / 4;
        }

        protected override void DisplaySearchResultsInternal()
        {
            lvRadioStations.Items.Clear();

            if (this.Items != null)
            {
                foreach (var item in this.Items)
                {
                    string title = item.Title ?? string.Empty;

                    int ilIndex = -1;
                    if (item.Source == OnlineMediaSource.ShoutCast)
                        ilIndex = 0;
                    else if (item.Source == OnlineMediaSource.Deezer)
                        ilIndex = 1;

                    ListViewItem lvi = new ListViewItem(new string[] 
                    {
                        // Icon
                        string.Empty,

                        // Name
                        title.ToUpperInvariant().StartsWith("TXT_") ?
                            Translator.Translate(title) : title,

                        // Artist
                        item.Artist ?? string.Empty,

                        // Album
                        item.Album ?? string.Empty,

                        // URL
                        item.Url ?? string.Empty,

                        // Genre
                        item.Genre ?? string.Empty,

                        // Bitrate
                        (item.Bitrate > 0) ? item.Bitrate.ToString() : string.Empty,

                        // Type
                        item.Type ?? string.Empty,
                    });

                    lvi.ImageIndex = ilIndex;

                    lvi.Tag = item;
                    lvRadioStations.Items.Add(lvi);
                }
            }

            if (lvRadioStations.Items.Count > 0)
            {
                lvRadioStations.Items[0].Selected = true;
                lvRadioStations.Items[0].Focused = true;
            }
        }

        protected override List<string> GetSearchHistory()
        {
            return ProTONEConfig.Media_Browser_History_Local;
        }

        protected override void SetSearchHistory(List<string> history)
        {
            ProTONEConfig.Media_Browser_History_Local = history;
        }
    }
}
