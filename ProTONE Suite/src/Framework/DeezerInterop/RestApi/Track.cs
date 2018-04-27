﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace OPMedia.DeezerInterop.RestApi
{
    public class Track : DeezerEntity
    {
        public UInt64 Id { get; set; }
        public string Title { get; set; }

        [JsonProperty("duration")]
        private double InternalDuration
        {
            get
            {
                return Duration.TotalSeconds;
            }
            set
            {
                Duration = TimeSpan.FromSeconds(value);
            }
        }

        [JsonIgnore]
        public TimeSpan Duration { get; set; }
        public Uri Preview { get; set; }
        public Artist Artist { get; set; }
        public Album Album { get; set; }

        public override string ToString()
        {
            return $"[ID={Id}, Artist={Artist}, Album={Album}, Title={Title}, Duration={(int)this.Duration.TotalSeconds}]";
        }

    }
}
