using OPMedia.Core;
using OPMedia.Runtime.ProTONE.Playlists;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using OPMedia.Core.Logging;

namespace OPMedia.Runtime.ProTONE.OnlineMediaContent
{
    public class LocalDatabaseSearcher : OnlineContentSearcher
    {
        protected override bool HasValidConfig
        {
            get
            {
                return true;
            }
        }

        static object _dbLock = new object();

        public static OnlineMediaData LoadOnlineMediaData()
        {
            lock (_dbLock)
            {
                OnlineMediaData internalDatabase = null;

                try
                {
                    // Fill with Internal stations list from Persistence Service
                    string xml = PersistenceProxy.ReadObject("OnlineMediaData", string.Empty, false);
                    if (!string.IsNullOrEmpty(xml))
                    {
                        using (StringReader sr = new StringReader(xml))
                        using (XmlReader xr = XmlReader.Create(sr))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(OnlineMediaData));
                            internalDatabase = xs.Deserialize(xr) as OnlineMediaData;
                        }
                    }
                }
                catch (Exception ex)
                {
                    internalDatabase = null;
                    Logger.LogException(ex);
                }

                // Try fall back to DefaultOnlineMediaData.xml
                if (internalDatabase == null)
                {
                    using (FileStream fs = new FileStream(@".\DefaultOnlineMediaData.xml", FileMode.Open, FileAccess.Read))
                    using (XmlReader xr = XmlReader.Create(fs))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(OnlineMediaData));
                        internalDatabase = xs.Deserialize(xr) as OnlineMediaData;
                    }
                }

                // Try fallback to empty data
                if (internalDatabase == null)
                    internalDatabase = new OnlineMediaData();

                return internalDatabase;
            }
        }

        public static bool SaveOnlineMediaData(OnlineMediaData data)
        {
            bool isSaved = false;
            lock (_dbLock)
            {
                string xml = null;

                try
                {
                    using (StringWriter sw = new StringWriter())
                    using (XmlWriter xw = XmlWriter.Create(sw))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(OnlineMediaData));
                        xs.Serialize(xw, data);

                        xml = sw.ToString();
                    }

                    if (xml != null)
                    {
                        PersistenceProxy.SaveObject("OnlineMediaData", xml, false);
                        isSaved = true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }

            return isSaved;
        }

        protected override List<OnlineMediaItem> Search(OnlineContentSearchParameters searchParams, ManualResetEvent abortEvent)
        {
            List<OnlineMediaItem> results = new List<OnlineMediaItem>();

            if (abortEvent.WaitOne(5))
                return results;

            OnlineMediaData internalDatabase = LoadOnlineMediaData();

            if (abortEvent.WaitOne(5))
                return results;

            if (internalDatabase != null &&
                internalDatabase.OnlineMediaItems != null &&
                internalDatabase.OnlineMediaItems.Count > 0)
            {
                foreach (var rs in internalDatabase.OnlineMediaItems)
                {
                    if (abortEvent.WaitOne(5))
                        break;

                    var content = rs.Content ?? string.Empty;
                    var genre = rs.Genre ?? string.Empty;
                    var title = rs.Title ?? string.Empty;
                    var url = rs.Url ?? string.Empty;

                    if (content.ToLowerInvariant().Contains(searchParams.SearchText) ||
                        genre.ToLowerInvariant().Contains(searchParams.SearchText) ||
                        title.ToLowerInvariant().Contains(searchParams.SearchText) ||
                        url.ToLowerInvariant().Contains(searchParams.SearchText))
                        results.Add(rs);
                }
            }

            return results;
        }

        protected override List<OnlinePlaylist> GetMyPlaylists(ManualResetEvent abortEvent)
        {
            return null;
        }

        protected override List<OnlineMediaItem> ExpandOnlinePlaylist(OnlinePlaylist p, ManualResetEvent abortEvent)
        {
            return null;
        }

    }
}
