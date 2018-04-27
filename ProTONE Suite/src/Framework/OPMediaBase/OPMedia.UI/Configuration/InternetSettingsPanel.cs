using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using OPMedia.UI.Themes;
using OPMedia.Core.Configuration;
using OPMedia.UI.Properties;

namespace OPMedia.UI.Configuration
{
    public partial class InternetSettingsPanel : MultiPageCfgPanel
    {
        public override Image Image
        {
            get
            {
                return OPMedia.Core.Properties.Resources.Internet;
            }
        }

        public InternetSettingsPanel() : base()
        {
            this.Title = "Internet";
            InitializeComponent();
        }

       
        
    }
}