using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OPMedia.UI.Themes;
using OPMedia.Runtime.ProTONE.Configuration;
using System.Net;
using System.Diagnostics;
using System.Threading;
using OPMedia.Core.Logging;

namespace OPMedia.UI.ProTONE.Configuration.InternetConfig
{
    public partial class DeezerCredentialsForm : ToolForm
    {
        string _redirectUri = "";

        public DeezerCredentialsForm()
        {
            InitializeComponent();
            wbDeezer.Navigated += new WebBrowserNavigatedEventHandler(wbDeezer_Navigated);
        }

        void wbDeezer_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            string url = e.Url.ToString();

            Logger.LogInfo("DeezerCredentialsForm: URL={0}", url);

            if (url.ToLowerInvariant().StartsWith(_redirectUri))
            {
                string fragment = url.Replace(_redirectUri, string.Empty).Trim('#');
                string[] fields = fragment.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (fields != null && fields.Length > 0)
                {
                    foreach (string f in fields)
                    {
                        if (f.StartsWith("access_token"))
                        {
                            wbDeezer.DocumentText = f;

                            string token = f.Replace("access_token=", string.Empty);
                            ProTONEConfig.DeezerUserAccessToken = token;

                            Thread.Sleep(1000);

                            DialogResult = DialogResult.OK;
                            Close();
                            return;
                        }
                    }
                }
            }
        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            _redirectUri = txtAppId.Text.ToLowerInvariant();
            string appId = ProTONEConfig.DeezerApplicationId;

            string url = string.Format("https://connect.deezer.com/oauth/auth.php?app_id={0}&redirect_uri={1}&perms=basic_access,offline_access&response_type=token",
                appId, _redirectUri);

            bool newWindow = chkNewBrowser.Checked;

            wbDeezer.Navigate(url, newWindow);
        }
    }
}
