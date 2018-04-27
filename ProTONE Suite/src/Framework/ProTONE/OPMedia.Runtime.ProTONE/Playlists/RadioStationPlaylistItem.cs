using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Core.Utilities;
using OPMedia.Runtime.ProTONE.Rendering;

namespace OPMedia.Runtime.ProTONE.Playlists
{
    public class RadioStationPlaylistItem : PlaylistItem
    {
        protected override bool _IsBookmarkInfoEditable()
        {
            return false;
        }

        protected override bool _IsTrackInfoEditable()
        {
            return false;
        }

        protected override bool _SupportsBookmarkInfo()
        {
            return false;
        }

        protected override bool _SupportsTrackInfo()
        {
            return false;
        }

        RadioStation _rs = null;

        public override string Type
        {
            get
            {
                return "URL";
            }
        }

        public override string SubType
        {
            get
            {
                return "ShoutCast";
            }
        }

        public override string DisplayName
        {
            get
            {
                return string.Format("{0} [{1}]", _rs.Title, _rs.Url);
            }
        }

        public override string Details
        {
            get
            {
                return _rs.Url;
            }
        }

        public override string PersistentPlaylistName
        {
            get
            {
                return _rs.Title;
            }
        }


        public override Dictionary<string, string> MediaInfo
        {
            get
            {
                Dictionary<string, string> info = base.MediaInfo;

                if (MediaRenderer.DefaultInstance.GetRenderFile() == mi.Name &&
                    (MediaRenderer.DefaultInstance.FilterState == Rendering.DS.BaseClasses.FilterState.Running ||
                    MediaRenderer.DefaultInstance.FilterState == Rendering.DS.BaseClasses.FilterState.Paused))
                {
                    foreach (KeyValuePair<string, string> kvp in MediaRenderer.DefaultInstance.StreamData)
                        info.Add(kvp.Key + ":", kvp.Value);
                }

                return info;
            }
        }

        public RadioStationPlaylistItem(RadioStation rs) : 
            base(rs.Url, false, false)
        {
            _rs = rs;
        }
    }
}
