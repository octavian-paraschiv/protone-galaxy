using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Core;
using LiteDB;
using OP_Logger = OPMedia.Core.Logging.Logger;
using OPMedia.SimpleCacheService;
using OPMedia.Core.Configuration;
using System.IO;

namespace OPMedia.PersistenceService
{
    public class DbStore
    {
        private static string __peristenceDbPath = @"..\\Persistence.db";

        static DbStore()
        {
            if (File.Exists(__peristenceDbPath) == false)
                __peristenceDbPath = "Persistence.db";

            using (var db = new LiteDatabase(__peristenceDbPath))
                db.Shrink();
        }

        public static string ReadObject(string persistenceId, string persistenceContext)
        {
            try
            {
                using (var db = new LiteDatabase(__peristenceDbPath))
                {
                    var obj = GetObjects<PersistedObject>(db);
                    var qry = GetQuery(persistenceId, persistenceContext);

                    var cnt = obj.Find(qry).Select(p => p.Content).FirstOrDefault();
                    return cnt;
                }
            }
            catch (Exception ex)
            {
                OP_Logger.LogException(ex);
            }

            return null;
        }

        public static void SaveObject(string persistenceId, string persistenceContext, string objectContent)
        {
            try
            {
                using (var db = new LiteDatabase(__peristenceDbPath))
                {
                    var obj = GetObjects<PersistedObject>(db);
                    var qry = GetQuery(persistenceId, persistenceContext);

                    var po = obj.Find(qry).FirstOrDefault();

                    if (po == null)
                    {
                        po = new PersistedObject
                        {
                            PersistenceId = persistenceId,
                            PersistenceContext = persistenceContext,
                            Content = objectContent
                        };

                        obj.Insert(po);
                    }
                    else
                    {
                        po.Content = objectContent;
                        obj.Update(po);
                    }
                }
            }
            catch (Exception ex)
            {
                OP_Logger.LogException(ex);
            }
        }

        public static void DeleteObject(string persistenceId, string persistenceContext)
        {
            try
            {
                using (var db = new LiteDatabase(__peristenceDbPath))
                {
                    var obj = GetObjects<PersistedObject>(db);
                    var qry = GetQuery(persistenceId, persistenceContext);

                    obj.Delete(qry);
                }
            }
            catch (Exception ex)
            {
                OP_Logger.LogException(ex);
            }
        }

        private static LiteCollection<T> GetObjects<T>(LiteDatabase db)
        {
            string typeName = typeof(T).Name;
            return db.GetCollection<T>($"{typeName}s");
        }

        private static Query GetQuery(string persistenceId, string persistenceContext)
        {
            if (string.IsNullOrEmpty(persistenceContext))
            {
                return Query.EQ("PersistenceId", persistenceId);
            }
            else
            {
                return Query.And
                (
                    Query.EQ("PersistenceId", persistenceId),
                    Query.EQ("PersistenceContext", persistenceContext)
                );
            }
        }
    }
}
