using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Runtime.Addons;
using OPMedia.Runtime.Addons.Configuration;
using OPMedia.Core.Configuration;
using OPMedia.Runtime.ProTONE.Configuration;
using System.ComponentModel;
using OPMedia.Core;

namespace OPMedia.Addons.Builtin.Configuration
{
    public static class BuiltinAddonConfig
    {
        public static string SearchPaths
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "SearchPaths", string.Empty);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "SearchPaths", value);
            }
        }

        public static string SearchTexts
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "SearchTexts", string.Empty);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "SearchTexts", value);
            }
        }

        public static string SearchPatterns
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "SearchPatterns", string.Empty);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "SearchPatterns", value);
            }
        }


        public static string SearchTextsMC
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "SearchTextsMC", string.Empty);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "SearchTextsMC", value);
            }
        }

        public static string SearchPatternsMC
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "SearchPatternsMC", string.Empty);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "SearchPatternsMC", value);
            }
        }

        public static int SplitterDistanceMC
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "SplitterDistanceMC", 200);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "SplitterDistanceMC", value);
            }
        }

        public static decimal FEPreviewTimer
        {
            get
            {
                try
                {
                    return (decimal)new DecimalConverter().ConvertFromInvariantString(
                    PersistenceProxy.ReadObject(true, "FEPreviewTimer",
                        new DecimalConverter().ConvertToInvariantString(0.5M)));
                }
                catch { }

                return 0.5M;
            }

            set
            {
                PersistenceProxy.SaveObject(true, "FEPreviewTimer",
                    new DecimalConverter().ConvertToInvariantString(value));
            }
        }

        public static string MCLastOpenedFolder
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "MCLastOpenedFolder", string.Empty);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "MCLastOpenedFolder", value);
            }
        }

        public static bool MCOpenLastCatalog
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "MCOpenLastCatalog", false);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "MCOpenLastCatalog", value);
            }
        }

        public static bool MCRememberRecentFiles
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "MCRememberRecentFiles", false);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "MCRememberRecentFiles", value);
            }
        }

        public static int MCRecentFilesCount
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "MCRecentFilesCount", 5);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "MCRecentFilesCount", value);
            }
        }

        public static string MCRecentFiles
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "MCRecentFiles", string.Empty);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "MCRecentFiles", value);
            }
        }

    }
}
