using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Net;
using OPMedia.Core.Logging;

namespace OPMedia.DeezerInterop.RestApi
{
    public class DeezerRuntime
    {
        private string _apiEndpoint = "http://api.deezer.com/";

        private WebClient _webClient;

        private Tuple<string, string> _applicationCredentials;

        public override bool Equals(object obj)
        {
            bool bEq = false;

            DeezerRuntime dzr = obj as DeezerRuntime;
            if (dzr != null)
            {
                if (dzr._applicationCredentials != null && this._applicationCredentials != null)
                {
                    bEq = (dzr._applicationCredentials.Item1 == this._applicationCredentials.Item1 &&
                        dzr._applicationCredentials.Item2 == this._applicationCredentials.Item2);
                }
            }

            return bEq;
        }

        public override int GetHashCode()
        {
            return (_applicationCredentials?.GetHashCode()).GetValueOrDefault();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DeezerRuntime"/> class.
        /// </summary>
        public DeezerRuntime(string apiEndpoint)
        {
            _apiEndpoint = apiEndpoint;
            _webClient = new WebClient();
            _applicationCredentials = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeezerRuntime"/> class.
        /// Sets the application credentials for use with needed requests.
        /// </summary>
        /// <param name="applicationId">Your Application ID.</param>
        /// <param name="applicationSecretKey">Your Application secret key.</param>
        public DeezerRuntime(string apiEndpoint, string applicationId, string applicationSecretKey)
            : this(apiEndpoint)
        {
            _applicationCredentials = new Tuple<string, string>(applicationId, applicationSecretKey);
        }

        internal void CheckResponseForErrors(string response)
        {
            if (string.IsNullOrEmpty(response))
                    throw new WebException("Empty response received from server.");

            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
            if (jsonResult.ContainsKey("error"))
            {
                ErrorInfo ei = JsonConvert.DeserializeObject<ErrorInfo>(jsonResult["error"].ToString());
                if (ErrorInfo.IsNullOrEmpty(ei) == false)
                    throw new DeezerRuntimeException(ei.ToString());
            }
        }

        internal string ExecuteHttpGet(string method)
        {
            string response = null;
            string url = _apiEndpoint + method;

            try
            {
                response = _webClient.DownloadString(url);
                CheckResponseForErrors(response);
            }
            catch (DeezerRuntimeException)
            {
                // Don't do anything ... mostly this would indicate that the requested objects were not found
                // on the Deezer server. This would be part of normal app operation so no need to log them.
            }
            catch (WebException wex)
            {
                // This catches web-related errors (Not Found, Unaouthorized, timeout ... etc ...)
                // Since these either indicate problems with the server or an internal app bug, we should log them
                Logger.LogWarning($"HTTP GET error: URL={url} => {wex.Message}");
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }

            return response;
        }

        public Artist GetArtist(UInt64 artistId)
        {
            Artist artist = null;

            string response = this.ExecuteHttpGet($"/artist/{artistId}");
            if (string.IsNullOrEmpty(response) == false)
            {
                artist = JsonConvert.DeserializeObject<Artist>(response);
                artist.CurrentRuntime = this;
            }

            return artist;
        }

        public Playlist GetPlaylist(UInt64 playlistId)
        {
            Playlist playlist = null;

            string response = this.ExecuteHttpGet($"/playlist/{playlistId}");
            if (string.IsNullOrEmpty(response) == false)
            {
                // There an issue with the API. We only get the first 400 tracks from the playlist
                // Working on that internally (because the /playlist/:id/tracks only has pages of 50 tracks...
                playlist = JsonConvert.DeserializeObject<Playlist>(response);

                playlist.CurrentRuntime = this;
                playlist.LoadTracks();
            }

            return playlist;
        }

        public List<Track> ExecuteSearch(string query, ManualResetEvent abortEvent)
        {
            List<Track> tracks = new List<Track>();

            if (abortEvent.WaitOne(5))
                return tracks;

            string response = this.ExecuteHttpGet($"/search?limit=10000&q={query}");
            if (string.IsNullOrEmpty(response) == false)
            {
                var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

                List<Track> tracksChunk = JsonConvert.DeserializeObject<List<Track>>(jsonResult["data"].ToString());
                if (tracksChunk != null && tracksChunk.Count > 0)
                    tracks.AddRange(tracksChunk);

                var albums = JsonConvert.DeserializeObject<List<Album>>(jsonResult["data"].ToString());
                if (albums != null)
                {
                    foreach (Album album in albums)
                    {
                        if (abortEvent.WaitOne(5))
                            break;

                        album.CurrentRuntime = this;

                        tracksChunk = album.LoadTracks();

                        if (tracksChunk != null && tracksChunk.Count > 0)
                            tracks.AddRange(tracksChunk);
                    }
                }
            }

            return tracks;
        }


        public List<Playlist> GetMyPlaylists(string accessToken, ManualResetEvent abortEvent)
        {
            List<Playlist> playlists = new List<Playlist>();

            if (abortEvent.WaitOne(5))
                return playlists;

            string response = this.ExecuteHttpGet($"user/me/playlists?access_token={accessToken}");
            if (string.IsNullOrEmpty(response) == false)
            {
                var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

                List<Playlist> playlistsChunk = JsonConvert.DeserializeObject<List<Playlist>>(jsonResult["data"].ToString());
                if (playlistsChunk != null && playlistsChunk.Count > 0)
                    playlists.AddRange(playlistsChunk);
            }

            return playlists;
        }
    }
}
