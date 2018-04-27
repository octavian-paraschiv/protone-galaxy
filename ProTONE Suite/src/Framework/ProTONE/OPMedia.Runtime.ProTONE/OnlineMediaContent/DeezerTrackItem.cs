using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OPMedia.Runtime.ProTONE.OnlineMediaContent
{
    [Serializable]
    public class DeezerTrackItem : OnlineMediaItem
    {
        public DeezerTrackItem()
        {
            this.Source = OnlineMediaSource.Deezer;
        }

        public override string ToString()
        {
            //return string.Format("{0} - {1} [{2}]", Artist, Title, Url);
            return string.Format("{0} - {1}", Artist, Title);
        }
    }
}
