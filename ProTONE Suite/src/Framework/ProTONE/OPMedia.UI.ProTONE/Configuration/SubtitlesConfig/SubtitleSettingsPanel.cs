using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Globalization;
using OPMedia.Core.TranslationSupport;
using OPMedia.Core.Configuration;
using OPMedia.Core;

using OPMedia.Runtime;
using OPMedia.UI.Configuration;
using OPMedia.UI.ProTONE.Properties;
using OPMedia.Runtime.ProTONE.SubtitleDownload;
using OPMedia.UI.Controls;
using OPMedia.UI.Themes;
using OPMedia.UI.Generic;

namespace OPMedia.UI.ProTONE.Configuration
{
    public partial class SubtitleSettingsPanel : BaseCfgPanel, IMultiPageCfgPanel
    {
        public override Image Image
        {
            get
            {
                return ImageProcessing.Subtitle;
            }
        }

        public override string GetHelpTopic()
        {
            if (tabSubtitlesOsd.SelectedTab != null)
                return string.Format("{0}/{1}", this.Name, tabSubtitlesOsd.SelectedTab.Name);

            return base.GetHelpTopic();
        }

        public SubtitleSettingsPanel()
            : base()
        {
            this.Title = "TXT_S_SUBTITLESETTINGS";
            InitializeComponent();

            tabSubtitlesOsd.SizeMode = TabSizeMode.Fixed;
            tabSubtitlesOsd.ItemSize = new System.Drawing.Size(130, 28);

            tabSubtitlesOsd.ImageList = new ImageList();
            tabSubtitlesOsd.ImageList.ImageSize = new System.Drawing.Size(24, 24);

            tabSubtitlesOsd.ImageList.Images.Add(ImageProcessing.Subtitle);
            tabSubtitlesOsd.ImageList.Images.Add(Resources.Fonts);

            tpSubtitles.ImageIndex = 0;
            tpOsd.ImageIndex = 1;

            pageSubtitles.ModifiedActive += new EventHandler(OnModifiedActive);
            pageOsd.ModifiedActive += new EventHandler(OnModifiedActive);

            this.HandleCreated += new EventHandler(SubtitleSettingsPanel_HandleCreated);
        }

        void SubtitleSettingsPanel_HandleCreated(object sender, EventArgs e)
        {
            tabSubtitlesOsd.SelectedIndex = 0;
        }

        void OnModifiedActive(object sender, EventArgs e)
        {
            Modified = true;
        }

        protected override void SaveInternal()
        {
            pageSubtitles.Save();
            pageOsd.Save();
            

            Modified = false;
        }

        void IMultiPageCfgPanel.SelectSubPage(string itemName)
        {
            foreach (OPMTabPage tp in tabSubtitlesOsd.TabPages)
            {
                if (tp.Text == itemName)
                {
                    tabSubtitlesOsd.SelectedTab = tp;
                    break;
                }
            }
        }
    }
}
