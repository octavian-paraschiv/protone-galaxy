using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OPMedia.UI.Configuration;
using OPMedia.Core.Configuration;
using OPMedia.Core.TranslationSupport;
using OPMedia.Addons.Builtin.Properties;

using OPMedia.UI.Controls;
using OPMedia.Runtime.Addons.Configuration;
using OPMedia.Core;
using OPMedia.Runtime.ProTONE.Rendering;
using OPMedia.Core.Utilities;
using OPMedia.Runtime.ProTONE.Configuration;
using OPMedia.Addons.Builtin.Configuration;

namespace OPMedia.Addons.Builtin.FileExplorer
{
    public partial class FileExplorerCfgPanel : BaseCfgPanel
    {
        OPMToolTip _tip = new OPMToolTip();

        public override Image Image
        {
            get
            {
                return Resources.FileExplorer16;
            }
        }

        public FileExplorerCfgPanel()
        {
            InitializeComponent();

            this.Title = "TXT_ADDON_FE_SETTINGS";
            this.HandleCreated += new EventHandler(FileExplorerCfgPanel_HandleCreated);

            connectedFilesConfigCtl1.ModifiedActive += new EventHandler(OnSettingsChanged);
        }

        void FileExplorerCfgPanel_HandleCreated(object sender, EventArgs e)
        {
            Translator.TranslateControl(this, DesignMode);

            nudMaxProcessedFiles.Value = AddonAppConfig.MaxProcessedEntries;
            nudPreviewTimer.Value = BuiltinAddonConfig.FEPreviewTimer;

            Dictionary<string, string> tableLinkedFiles = ProTONEConfig.LinkedFilesTable;
            if (tableLinkedFiles.Count < 1)
            {
                List<string> supChildrenForAudioTypes = new List<string>();
                supChildrenForAudioTypes.Add("BMK");

                List<string> supChildrenForVideoTypes = new List<string>();
                supChildrenForVideoTypes.AddRange(MediaRenderer.GetSupportedFileProvider().SupportedSubtitles);
                supChildrenForVideoTypes.Add("BMK");

                tableLinkedFiles.Add(
                    StringUtils.FromStringArray(MediaRenderer.SupportedAudioTypes.ToArray(), ';'),
                    StringUtils.FromStringArray(supChildrenForAudioTypes.ToArray(), ';'));

                tableLinkedFiles.Add(
                    StringUtils.FromStringArray(MediaRenderer.SupportedVideoTypes.ToArray(), ';'),
                    StringUtils.FromStringArray(supChildrenForVideoTypes.ToArray(), ';'));

                ProTONEConfig.LinkedFilesTable = new Dictionary<string, string>(tableLinkedFiles);
            }
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            Modified = true;
        }

        protected override void SaveInternal()
        {
            connectedFilesConfigCtl1.Save();
            AddonAppConfig.MaxProcessedEntries = (int)nudMaxProcessedFiles.Value;
            BuiltinAddonConfig.FEPreviewTimer = nudPreviewTimer.Value;
            
        }
    }
}
