using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using OPMedia.Core;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using OPMedia.Core.Utilities;
using System.Net;
using Newtonsoft.Json.Linq;
using OPMedia.Runtime.ProTONE.Configuration;
using OPMedia.Core.Logging;
using System.Web;
using OPMedia.Core.TranslationSupport;
using System.Threading;
using OPMedia.Runtime.ProTONE.OnlineMediaContent;

namespace OPMedia.Runtime.ProTONE.Playlists
{
    [Serializable]
    public class RadioStation : OnlineMediaItem
    {
        private bool _isFake = false;

        public bool IsFake
        {
            get
            {
                return _isFake;
            }
        }

        public static RadioStation Empty
        {
            get
            {
                RadioStation rs = new RadioStation(true);
                rs.Title = "TXT_NO_STATIONS_LOADED";
                return rs;
            }
        }

        public static RadioStation NotFound
        {
            get
            {
                RadioStation rs = new RadioStation(true);
                rs.Title = "TXT_NO_STATIONS_FOUND";
                return rs;
            }
        }

        public RadioStation(bool isFake)
        {
            this.Source = OnlineMediaSource.Internal;
            _isFake = isFake;
        }

        public RadioStation()
        {
            this.Source = OnlineMediaSource.Internal;
            _isFake = false;
        }

        public RadioStation(OnlineMediaSource source)
        {
            this.Source = source;
            _isFake = false;
        }
    }

    public class SearchParams
    {
        public string Text { get; set; }
        public OnlineMediaSource Source { get; set; }
        public int MaxResults { get; set; }
    }


    [DataContract]
    [Serializable]
    [XmlInclude(typeof(RadioStation))]
    [XmlInclude(typeof(DeezerTrackItem))]
    public class OnlineMediaData
    {
        [DataMember(Order = 0)]
        public List<OnlineMediaItem> OnlineMediaItems { get; set; }

        public OnlineMediaData()
        {
            OnlineMediaItems = new List<OnlineMediaItem>();
        }

        public static OnlineMediaData GetDefault()
        {
            OnlineMediaData rsd = new OnlineMediaData();

            rsd.OnlineMediaItems.Add(RadioStation.Empty);

            return rsd;
        }

        public bool Contains(OnlineMediaItem item)
        {
            return OnlineMediaItems.Contains(item);
        }

        public void SafeAddItem(OnlineMediaItem item)
        {
            if (Contains(item) == false)
                OnlineMediaItems.Add(item);
        }

        public void SafeRemoveItem(OnlineMediaItem item)
        {
            if (Contains(item))
                OnlineMediaItems.Remove(item);
        }
    }
}
