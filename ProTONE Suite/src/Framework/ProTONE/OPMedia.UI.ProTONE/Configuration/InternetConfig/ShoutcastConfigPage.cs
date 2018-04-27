using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OPMedia.UI.Configuration;
using OPMedia.UI.ProTONE.Properties;
using OPMedia.Runtime.ProTONE.OnlineMediaContent;
using OPMedia.Runtime.ProTONE.Configuration;
using OPMedia.Core.Configuration;
using System.Diagnostics;

namespace OPMedia.UI.ProTONE.Configuration.InternetConfig
{
    public partial class ShoutcastConfigPage : BaseCfgPanel
    {
        public override Image Image
        {
            get
            {
                return Resources.Shoutcast16;
            }
        }

        public ShoutcastConfigPage()
        {
            InitializeComponent();
            this.Title = "ShoutCast";

            this.HandleCreated += new EventHandler(ShoutcastConfigPage_HandleCreated);

            txtShoutcastDevId.InnerTextBox.TextChanged += new EventHandler(OnTextChanged);
            txtShoutcastSearchURL.InnerTextBox.TextChanged += new EventHandler(OnTextChanged);
            txtShoutcastTuneinURL.InnerTextBox.TextChanged += new EventHandler(OnTextChanged);
        }

        void OnTextChanged(object sender, EventArgs e)
        {
            base.Modified = true;
        }

        void ShoutcastConfigPage_HandleCreated(object sender, EventArgs e)
        {
            txtShoutcastDevId.Text = ProTONEConfig.ShoutCastApiDevID;
            txtShoutcastSearchURL.Text = ProTONEConfig.ShoutCastSearchBaseURL;
            txtShoutcastTuneinURL.Text = ProTONEConfig.ShoutCastTuneInBaseURL;

            this.Enabled = AppConfig.CurrentUserIsAdministrator;
        }

        protected override void SaveInternal()
        {
            ProTONEConfig.ShoutCastApiDevID = txtShoutcastDevId.Text;
            ProTONEConfig.ShoutCastSearchBaseURL = txtShoutcastSearchURL.Text;
            ProTONEConfig.ShoutCastTuneInBaseURL = txtShoutcastTuneinURL.Text;
        }

        private void opmLinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.shoutcast.com/Developer");
        }
    }
}
