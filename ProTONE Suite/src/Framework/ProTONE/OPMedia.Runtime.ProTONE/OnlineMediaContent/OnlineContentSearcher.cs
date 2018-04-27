using OPMedia.Core.Logging;
using OPMedia.Runtime.ProTONE.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPMedia.Runtime.ProTONE.OnlineMediaContent
{
    [Flags]
    public enum OnlineContentSearchFilter
    {
        None = 0x00,

        Title = 0x01,
        Artist = 0x02,
        Album = 0x04,

        Any = 0xFF,
    }

    public class OnlineContentSearchParameters
    {
        public OnlineContentSearchFilter Filter { get; set; }
        public string SearchText { get; set; }

        public override string ToString()
        {
            return string.Format("[SearchText='{0}', Filter={1}]", SearchText ?? "<null>", Filter);
        }

        public OnlineContentSearchParameters()
        {
            Filter = OnlineContentSearchFilter.Any;
            SearchText = string.Empty;
        }
    }

    public abstract class OnlineContentSearcher
    {
        public static bool IsSearchConfigValid(OnlineMediaSource source)
        {
            try
            {
                OnlineContentSearcher searcher = GetSearcher(source);
                if (searcher != null)
                    return searcher.HasValidConfig;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return false;
        }

        public static List<OnlineMediaItem> Search(OnlineMediaSource source, OnlineContentSearchParameters searchParams, ManualResetEvent abortEvent)
        {
            try
            {
                if (searchParams == null)
                    searchParams = new OnlineContentSearchParameters();

                // Nothing means Everything.
                if (searchParams.Filter == OnlineContentSearchFilter.None)
                    searchParams.Filter = OnlineContentSearchFilter.Any;

                searchParams.SearchText = searchParams.SearchText.ToLowerInvariant();

                OnlineContentSearcher searcher = GetSearcher(source);
                if (searcher != null)
                    return searcher.Search(searchParams, abortEvent);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return null;
        }

        public static List<OnlinePlaylist> GetMyPlaylists(OnlineMediaSource source, ManualResetEvent abortEvent)
        {
            try
            {
                OnlineContentSearcher searcher = GetSearcher(source);
                if (searcher != null)
                    return searcher.GetMyPlaylists(abortEvent);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return null;
        }

        public static List<OnlineMediaItem> ExpandOnlinePlaylist(OnlineMediaSource source, OnlinePlaylist p, ManualResetEvent abortEvent)
        {
            try
            {
                OnlineContentSearcher searcher = GetSearcher(source);
                if (searcher != null)
                    return searcher.ExpandOnlinePlaylist(p, abortEvent);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return null;
        }

        private static OnlineContentSearcher GetSearcher(OnlineMediaSource source)
        {
            OnlineContentSearcher searcher = null;

            switch (source)
            {
                case OnlineMediaSource.Deezer:
                    searcher = new DeezerTrackSearcher();
                    break;

                case OnlineMediaSource.ShoutCast:
                    searcher = new ShoutcastDirSearcher();
                    break;

                case OnlineMediaSource.Internal:
                default:
                    searcher = new LocalDatabaseSearcher();
                    break;
            }

            return searcher;
        }

        protected abstract List<OnlineMediaItem> Search(OnlineContentSearchParameters searchParams, ManualResetEvent abortEvent);
        protected abstract List<OnlinePlaylist> GetMyPlaylists(ManualResetEvent abortEvent);
        protected abstract List<OnlineMediaItem> ExpandOnlinePlaylist(OnlinePlaylist p, ManualResetEvent abortEvent);

        protected abstract bool HasValidConfig { get; }
    }
}
