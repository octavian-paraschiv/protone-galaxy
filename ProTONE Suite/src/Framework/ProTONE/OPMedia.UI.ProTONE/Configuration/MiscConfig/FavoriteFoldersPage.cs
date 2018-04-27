using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using System.Linq;
using System.Text;
using System.Windows.Forms;
using OPMedia.UI.Configuration;
using OPMedia.Core.TranslationSupport;

using OPMedia.Core;
using Microsoft.Win32;
using OPMedia.UI.Properties;
using OPMedia.Runtime.ProTONE.Haali;
using OPMedia.Runtime.ProTONE.FfdShowApi;
using OPMedia.Core.Configuration;
using OPMedia.Runtime.ProTONE.Configuration;

namespace OPMedia.UI.ProTONE.Configuration.MiscConfig
{
    public partial class FavoriteFoldersPage : BaseCfgPanel
    {
        public FavoriteFoldersPage()
        {
            InitializeComponent();
            favoriteFoldersControl.ShowOKButton = false;
            favoriteFoldersControl.FavoriteFoldersHiveName = "FavoriteFolders";
            favoriteFoldersControl.OnModified += new EventHandler(favoriteFoldersControl_OnModified);
        }

        void favoriteFoldersControl_OnModified(object sender, EventArgs e)
        {
            Modified = true;
        }

        protected override void SaveInternal()
        {
            favoriteFoldersControl.Save();
        }
    }
}
