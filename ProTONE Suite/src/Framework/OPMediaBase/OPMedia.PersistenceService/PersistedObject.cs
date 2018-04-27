using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDB;

namespace OPMedia.SimpleCacheService
{
    public class PersistedObject
    {
        public Int64 _id { get; set; }
        public string PersistenceId { get; set; }
        public string PersistenceContext  { get; set; }
        public string Content { get; set; }
    }
}
