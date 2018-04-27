using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Core.Configuration;
using System.Windows.Forms;
using OPMedia.Core;

namespace OPMedia.Runtime.Addons.Configuration
{
    public static class AddonAppConfig
    {
        public static int VSplitterDistance
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "VSplitterDistance", 4 * Screen.PrimaryScreen.Bounds.Width / 9);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "VSplitterDistance", value);
            }
        }

        public static int HSplitterDistance
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "HSplitterDistance", Screen.PrimaryScreen.Bounds.Height / 3 - 50);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "HSplitterDistance", value);
            }
        }

        public static string LastNavAddon
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "LastNavAddon", "FileExplorer");
            }

            set
            {
                PersistenceProxy.SaveObject(true, "LastNavAddon", value);
            }
        }

        public static int MaxProcessedEntries
        {
            get { return PersistenceProxy.ReadObject(true, "MaxProcessedEntries", 100); }
            set { PersistenceProxy.SaveObject(true, "MaxProcessedEntries", value); }
        }
    }
}
