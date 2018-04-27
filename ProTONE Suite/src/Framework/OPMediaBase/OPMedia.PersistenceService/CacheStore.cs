using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Core;
using OPMedia.Core.Logging;
using System.Threading;

namespace OPMedia.PersistenceService
{
    public class CacheItem
    {
        // Time-To-Live: 30 seconds
        const int MaxCachedItemTTL = 30 * 1000;

        public string Key { get; private set; }

        object _accessLock = new object();

        public string _value = null;
        public string Value 
        {
            get
            {
                lock (_accessLock)
                {
                    this.TimeStamp = DateTime.Now;
                    return _value;
                }
            }

            set
            {
                lock (_accessLock)
                {
                    this.TimeStamp = DateTime.Now;
                    _value = value;
                }
            }
        }

        public DateTime TimeStamp { get; private set; }

        public bool IsExpired
        {
            get
            {
                TimeSpan ts = DateTime.Now - TimeStamp;
                return (ts.TotalMilliseconds >= MaxCachedItemTTL);
            }
        }

        public CacheItem(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

    public class CacheStore
    {
        // Poll for expired items at each 10 seconds.
        const int ExpiredItemsPollTimer = 10 * 1000;

        const int IterationsUntilGarbageCollection = (10 * 60 * 1000) / ExpiredItemsPollTimer;

        private static CacheStore _instance = new CacheStore();
        public static CacheStore Instance
        {
            get
            {
                return _instance;
            }
        }

        private Dictionary<string, CacheItem> _cache = new Dictionary<string, CacheItem>();

        private System.Timers.Timer _tmrCachePoller = null;
        private object _cachePollerLock = new object();

        private CacheStore()
        {
            _tmrCachePoller = new System.Timers.Timer();
            _tmrCachePoller.Interval = ExpiredItemsPollTimer;
            _tmrCachePoller.Elapsed += new System.Timers.ElapsedEventHandler(_tmrCachePoller_Elapsed);
            _tmrCachePoller.Start();
        }

        int _iter = 0;
        void _tmrCachePoller_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _tmrCachePoller.Stop();
                PollForExpiredCachedItems();
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }
            finally
            {
                try
                {
                    _iter = (_iter + 1) % IterationsUntilGarbageCollection;
                    if (_iter == 0)
                        GC.Collect();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }

                _tmrCachePoller.Start();
            }
        }

        private void PollForExpiredCachedItems()
        {
            lock (_cachePollerLock)
            {
                List<string> keysToDelete = new List<string>();

                foreach (KeyValuePair<string, CacheItem> kvp in _cache)
                {
                    if (kvp.Value.IsExpired)
                    {
                        Logger.LogTrace("PollForExpiredCachedItems: Object with key {0} seems to be expired.", kvp.Key);
                        keysToDelete.Add(kvp.Key);
                    }
                }

                Logger.LogTrace("PollForExpiredCachedItems: A number of {0} objects will be removed from the cache...", keysToDelete.Count);

                foreach (string key in keysToDelete)
                {
                    if (_cache.ContainsKey(key))
                    {
                        Logger.LogTrace("PollForExpiredCachedItems: Object with key {0} was removed from cache because it was expired.", key);
                        _cache.Remove(key);
                    }
                }
            }
        }

        public string ReadObject(string persistenceId, string persistenceContext)
        {
            if (string.IsNullOrEmpty(persistenceContext))
                persistenceContext = "*";

            string key = BuildCacheKey(persistenceId, persistenceContext);

            lock (_cachePollerLock)
            {
                // Is the requested object in the cache ?
                if (_cache.ContainsKey(key))
                {
                    Logger.LogTrace("ReadObject: Object with key {0} was found in cache.", key);

                    // If it is, return it from the cache.
                    return _cache[key].Value;
                }
            }

            Logger.LogTrace("ReadObject: Object with key {0} was not found in cache.", key);

            // If it is not, get it from the persistence DB and also add it in the cache.
            string s = DbStore.ReadObject(persistenceId, persistenceContext);
            if (s != null)
            {
                Logger.LogTrace("ReadObject: Object with key {0} was found in DB.", key);

                CacheItem ci = new CacheItem(key, s);

                lock (_cachePollerLock)
                {
                    Logger.LogTrace("ReadObject: Object with key {0} was added to cache with a TTL of 5 sec", key);
                    _cache.Add(key, ci);
                }

                return ci.Value;
            }

            Logger.LogTrace("ReadObject: Object with key {0} was not found in DB also.", key);

            return null;
        }

        public bool SaveObject(string persistenceId, string persistenceContext, string objectContent)
        {
            bool retVal = false;

            try
            {
                if (string.IsNullOrEmpty(persistenceContext))
                    persistenceContext = "*";

                string key = BuildCacheKey(persistenceId, persistenceContext);
                bool foundInCache = false;

                lock (_cachePollerLock)
                {
                    // Is the requested object in the cache ?
                    if (_cache.ContainsKey(key))
                    {
                        Logger.LogTrace("SaveObject: Object with key {0} was found and updated in cache.", key);

                        // If it is, update it in the cache.
                        _cache[key].Value = objectContent;
                        foundInCache = true;

                        retVal = true;
                    }
                }

                // We need to update in DB anyways and also make sure it is in the cache.
                if (foundInCache == false)
                {
                    CacheItem ci = new CacheItem(key, objectContent);

                    lock (_cachePollerLock)
                    {
                        Logger.LogTrace("SaveObject: Object with key {0} was not found cache => adding it now", key);
                        _cache.Add(key, ci);

                        retVal = true;
                    }
                }

                ThreadPool.QueueUserWorkItem((c) =>
                    {
                        Logger.LogTrace("SaveObject: Object with key {0} is now saved also in DB.", key);
                        DbStore.SaveObject(persistenceId, persistenceContext, objectContent);
                    });
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        public bool DeleteObject(string persistenceId, string persistenceContext)
        {
            bool retVal = false;

            try
            {
                if (string.IsNullOrEmpty(persistenceContext))
                    persistenceContext = "*";

                string key = BuildCacheKey(persistenceId, persistenceContext);

                lock (_cachePollerLock)
                {
                    if (_cache.ContainsKey(key))
                    {
                        Logger.LogTrace("DeleteObject: Object with key {0} was found in cache and removed.", key);
                        _cache.Remove(key);

                        retVal = true;
                    }
                }

                ThreadPool.QueueUserWorkItem((c) =>
                    {
                        Logger.LogTrace("DeleteObject: Object with key {0} is now removed also from the DB.", key);
                        DbStore.DeleteObject(persistenceId, persistenceContext);
                    });
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        private string BuildCacheKey(string persistenceId, string persistenceContext)
        {
            return $"{persistenceId}_{persistenceContext}";
        }

    }

}
