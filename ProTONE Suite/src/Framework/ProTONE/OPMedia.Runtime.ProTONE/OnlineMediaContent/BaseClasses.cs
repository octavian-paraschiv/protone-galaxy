using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using OPMedia.Runtime.ProTONE.Playlists;

namespace OPMedia.Runtime.ProTONE.OnlineMediaContent
{
    [Flags]
    public enum OnlineMediaSource
    {
        None = 0x00,

        Internal = 0x01,
        ShoutCast = 0x02,
        Deezer = 0x04,

        All = 0xFF,
    }

    [DataContract]
    [Serializable]
    [XmlInclude(typeof(RadioStation))]
    [XmlInclude(typeof(DeezerTrackItem))]
    public class OnlineMediaItem
    {
        [DataMember(Order = 0)]
        public string Url { get; set; }

        [DataMember(Order = 1)]
        public string Title { get; set; }

        [DataMember(Order = 2)]
        public string Genre { get; set; }

        [DataMember(Order = 3)]
        public string Type { get; set; }

        [DataMember(Order = 4)]
        public OnlineMediaSource Source { get; set; }

        [DataMember(Order = 5)]
        public int Bitrate { get; set; }

        [DataMember(Order = 6)]
        public string Content { get; set; }

        [DataMember(Order = 7)]
        public string Artist { get; set; }

        [DataMember(Order = 8)]
        public string Album { get; set; }

        [DataMember(Order = 9)]
        public TimeSpan Duration { get; set; }

        public override bool Equals(object obj)
        {
            OnlineMediaItem item = obj as OnlineMediaItem;
            if (item != null)
            {
                return (string.Compare(this.Url, item.Url, true) == 0);
            }

            return false;
        }
    }

    [DataContract]
    [Serializable]
    public class OnlinePlaylist
    {
        [DataMember(Order = 0)]
        public UInt64 Id { get; set; }

        [DataMember(Order = 1)]
        public string Title { get; set; }

        [DataMember(Order = 2)]
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("[Id={0}, Title={1}, Desc={2}]", Id, Title, Description);
        }
    }

}
