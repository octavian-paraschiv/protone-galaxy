using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Runtime.ProTONE.Playlists;
using System.Drawing;
using OPMedia.Runtime.ProTONE.OnlineMediaContent;
using OPMedia.UI.ProTONE.Properties;
using OPMedia.Core;

namespace OPMedia.UI.ProTONE
{
    public static class PlaylistItemExtensions
    {
        public static Image GetImageEx(this PlaylistItem pli, bool large)
        {
            Image img = null; 
            
            if (pli != null)
            {
                if (pli is RadioStationPlaylistItem)
                {
                    Bitmap bmp = Resources.Shoutcast;
                    bmp.MakeTransparent(Color.White);
                    img = bmp.Resize(large);
                }
                else if (pli is DeezerTrackPlaylistItem)
                {
                    Bitmap bmp = Resources.deezer;
                    bmp.MakeTransparent(Color.White);
                    img = bmp.Resize(large);
                }
                else
                    img = pli.GetImage(true);
            }

            return img;
        }
    }
}
