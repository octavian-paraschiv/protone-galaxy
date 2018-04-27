using OPMedia.Runtime.ProTONE.OnlineMediaContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Core.Utilities;

namespace OPMedia.Runtime.ProTONE.Playlists
{
    public class DeezerTrackPlaylistItem : PlaylistItem
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

        DeezerTrackItem _dti = null;

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
                return "Deezer";
            }
        }

        public override string DisplayName
        {
            get
            {
                return _dti.ToString();
            }
        }

        public override string Details
        {
            get
            {
                return _dti.Url;
            }
        }

        public override string PersistentPlaylistName
        {
            get
            {
                return string.Format("{0}`{1}`{2}", _dti.Artist, _dti.Title, _dti.Album);
            }
        }

        public override TimeSpan Duration
        {
            get
            {
                return _dti.Duration;
            }
        }

        public override Dictionary<string, string> MediaInfo
        {
            get            
            {
                Dictionary<string, string> info = base.MediaInfo;

                if (_dti != null)
                {
                    info.Add(" ", null); // separator

                    // add specific details for DeezerTrackItem
                    info.Add("TXT_ARTIST:", _dti.Artist);
                    info.Add("TXT_TITLE:", _dti.Title);
                    info.Add("TXT_ALBUM:", _dti.Album);
                    info.Add("TXT_DURATION:", _dti.Duration.ToString());
                }

                return info;
            }
        }


        public DeezerTrackPlaylistItem(DeezerTrackItem dti) : 
            base(dti.Url, false, false)
        {
            _dti = dti;
        }
    }
}
