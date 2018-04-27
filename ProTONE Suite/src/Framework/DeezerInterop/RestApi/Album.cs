using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OPMedia.Core.Logging;

namespace OPMedia.DeezerInterop.RestApi
{

    [DebuggerDisplay("Album({Id},{Title})")]
    public class Album : DeezerEntity
    {
        public UInt64 Id { get; set; }

        public string Title { get; set; }

        [JsonProperty("cover")]
        public Uri AlbumImageUri { get; set; }

        [JsonProperty("genre_id")]
        public MusicGenre Genre { get; set; }

        public int TracksCount { get; set; }

        [JsonProperty("tracks")]
        public List<Track> Tracks { get; set; }

        [JsonProperty("artist")]
        public Artist Artist { get; set; }

        public List<Track> LoadTracks()
        {
            Tracks = new List<Track>();

            try
            {
                string response = CurrentRuntime.ExecuteHttpGet($"/album/{Id}/tracks");
                if (string.IsNullOrEmpty(response) == false)
                {
                    var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

                    Tracks = JsonConvert.DeserializeObject<List<Track>>(jsonResult["data"].ToString());

                    foreach (Track track in Tracks)
                    {
                        track.CurrentRuntime = CurrentRuntime;
                        track.Album = this;
                    }
                }
            }
            catch
            {
            }

            TracksCount = Tracks.Count;
            return Tracks;

        }

        public override string ToString()
        {
            return $"[ID={Id}, Artist={Artist}, Title={Title}, Genre={Genre}]"; 
        }
    }
}
