using System;
using System.Collections.Generic;
using System.Text;
using OPMedia.Runtime.ProTONE.Playlists;
using System.IO;
using System.Windows.Forms;
using OPMedia.Core;

namespace OPMedia.Runtime.ProTONE.Playlists
{
    public class PersistentPlaylist
    {
        static bool _isLoading = false;

        static PersistentPlaylist()
        {
        }

        public static void Load(ref Playlist playlist)
        {
            string persistedPlaylist = PersistenceProxy.ReadObject(true, "PersistentPlaylist", string.Empty);
            if (string.IsNullOrEmpty(persistedPlaylist) == false)
            {
                try
                {
                    _isLoading = true;
                    playlist.LoadM3UPlaylistFromString(persistedPlaylist);
                }
                finally
                {
                    _isLoading = false;
                }
            }
        }

        public static void Save(Playlist playlist)
        {
            if (_isLoading)
            {
                playlist.AbortLoad();
            }
            else
            {
                string persistedPlaylist = playlist.SaveM3UPlaylistAsString();
                PersistenceProxy.SaveObject(true, "PersistentPlaylist", persistedPlaylist);
            }
        }
    }
}
