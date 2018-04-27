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
using OPMedia.UI.Themes;
using OPMedia.Runtime.ProTONE.Configuration;

namespace OPMedia.UI.ProTONE.Controls.OnlineMediaBrowser
{
    public partial class ShoutcastDirectoryBrowserCtl : MediaBrowserPage
    {
        public ShoutcastDirectoryBrowserCtl()
        {
            InitializeComponent();
            _searchType = OnlineMediaSource.ShoutCast;

            lvRadioStations.MultiSelect = true;
            lvRadioStations.Resize += lvRadioStations_Resize;
            lvRadioStations.SelectedIndexChanged += LvRadioStations_SelectedIndexChanged;
            lvRadioStations.ContextMenuStrip = BuildCommonMenuStrip(true);
        }


        private void LvRadioStations_SelectedIndexChanged(object sender, EventArgs e)
        {
            base.SelectedItems.Clear();

            foreach (ListViewItem lvi in lvRadioStations.SelectedItems)
            {
                RadioStation rs = lvi.Tag as RadioStation;
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

            colSource.Width = colGenre.Width = colMediaType.Width = 70;
            //colContent.Width = 100;
            colBitrate.Width = 50;

            int w = colSource.Width + colGenre.Width + colMediaType.Width + colBitrate.Width;
            colContent.Width = colURL.Width = colName.Width = (lvRadioStations.EffectiveWidth - w) / 3;
        }

        protected override void DisplaySearchResultsInternal()
        {
            lvRadioStations.Items.Clear();

            if (this.Items != null)
            {
                foreach (var item in this.Items)
                {
                    RadioStation rs = item as RadioStation;
                    if (rs != null)
                    {
                        string title = rs.Title ?? string.Empty;

                        ListViewItem lvi = new ListViewItem(new string[] { "",

                        title.ToUpperInvariant().StartsWith("TXT_") ?
                            Translator.Translate(title) : title,

                        rs.IsFake ? "" : rs.Source.ToString(),
                        rs.Url, rs.Content, rs.Genre,
                        rs.IsFake ? "" : rs.Bitrate.ToString(),
                        rs.Type });

                        lvi.Tag = rs;
                        lvRadioStations.Items.Add(lvi);
                    }
                }

                if (lvRadioStations.Items.Count > 0)
                {
                    lvRadioStations.Items[0].Selected = true;
                    lvRadioStations.Items[0].Focused = true;
                }
            }
        }

        public override bool PreValidateSearch(string search)
        {
            bool valid = base.PreValidateSearch(search);

            if (valid)
            {
                search = search.ToLowerInvariant();

                if (search.StartsWith("now:"))
                {
                    // We're trying to make use of the "now playing" advanced search filter.
                    // If the search string after "now:" is non empty then we're OK.
                    search = search.Replace("now:", "");
                    valid = string.IsNullOrEmpty(search) == false;
                }
            }

            return valid;
        }

        public override string GetSearchBoxTip()
        {
            return Translator.Translate("TXT_SHOUTCASTFILTER_HINT");
        }

        protected override List<string> GetSearchHistory()
        {
            return ProTONEConfig.Media_Browser_History_Shoutcast;
        }

        protected override void SetSearchHistory(List<string> history)
        {
            ProTONEConfig.Media_Browser_History_Shoutcast = history;
        }
    }
}
