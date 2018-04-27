using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Core.Configuration;
using OPMedia.Core;

namespace OPMedia.SkinBuilder.Configuration
{
    public class SkinBuilderConfiguration 
    {
        public static string LastOpenedFolder
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "LastOpenedFolder", string.Empty);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "LastOpenedFolder", value);
            }
        }

        public static bool OpenLastFile
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "OpenLastFile", false);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "OpenLastFile", value);
            }
        }

        public static bool RememberRecentFiles
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "RememberRecentFiles", false);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "RememberRecentFiles", value);
            }
        }

        public static int RecentFilesCount
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "RecentFilesCount", 5);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "RecentFilesCount", value);
            }
        }

        public static string RecentFiles
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "RecentFiles", string.Empty);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "RecentFiles", value);
            }
        }
    }
}
