using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using System.Linq;
using System.Text;
using System.Windows.Forms;
using OPMedia.UI.Configuration;
using OPMedia.Core.Configuration;
using OPMedia.Runtime;

using LocalEventNames = OPMedia.UI.ProTONE.GlobalEvents.EventNames;
using OPMedia.Core;
using OPMedia.Core.Utilities;
using OPMedia.UI.Controls;
using OPMedia.Runtime.ProTONE.Configuration;
using OPMedia.Runtime.ProTONE.Rendering;

namespace OPMedia.UI.ProTONE.Configuration.MiscConfig
{
    public partial class MediaScreensOptionsPage : BaseCfgPanel
    {
        public MediaScreensOptionsPage()
        {
            InitializeComponent();

            chkShowPlaylist.Checked = ProTONEConfig.MediaScreenActive(MediaScreen.Playlist);
            chkShowTrackInfo.Checked = ProTONEConfig.MediaScreenActive(MediaScreen.TrackInfo);
            chkShowSignalAnalisys.Checked = ProTONEConfig.MediaScreenActive(MediaScreen.SignalAnalisys);
            chkShowBookmarkInfo.Checked = ProTONEConfig.MediaScreenActive(MediaScreen.BookmarkInfo);

            chkVuMeter.Checked = ProTONEConfig.SignalAnalisysFunctionActive(SignalAnalisysFunction.VUMeter);
            chkWaveform.Checked = ProTONEConfig.SignalAnalisysFunctionActive(SignalAnalisysFunction.Waveform);
            chkSpectrogram.Checked = ProTONEConfig.SignalAnalisysFunctionActive(SignalAnalisysFunction.Spectrogram);
            chkWCFInterface.Checked = ProTONEConfig.SignalAnalisysFunctionActive(SignalAnalisysFunction.WCFInterface);

            this.chkShowPlaylist.CheckedChanged += new System.EventHandler(this.OnSettingsChanged);
            this.chkShowTrackInfo.CheckedChanged += new System.EventHandler(this.OnSettingsChanged);
            this.chkShowSignalAnalisys.CheckedChanged += new System.EventHandler(this.OnSettingsChanged);
            this.chkShowBookmarkInfo.CheckedChanged += new System.EventHandler(this.OnSettingsChanged);
            
            this.chkVuMeter.CheckedChanged += new System.EventHandler(this.OnSettingsChanged);
            this.chkWaveform.CheckedChanged += new System.EventHandler(this.OnSettingsChanged);
            this.chkSpectrogram.CheckedChanged += new System.EventHandler(this.OnSettingsChanged);
            this.chkWCFInterface.CheckedChanged += new System.EventHandler(this.OnSettingsChanged);
        }

        protected override void SaveInternal()
        {
            MediaScreen mediaScreen = MediaScreen.None;
            SignalAnalisysFunction functions = SignalAnalisysFunction.None;

            if (chkShowPlaylist.Checked) 
                mediaScreen |= MediaScreen.Playlist;
            if (chkShowTrackInfo.Checked)
                mediaScreen |= MediaScreen.TrackInfo;
            if (chkShowBookmarkInfo.Checked)
                mediaScreen |= MediaScreen.BookmarkInfo;

            if (chkShowSignalAnalisys.Checked)
            {
                mediaScreen |= MediaScreen.SignalAnalisys;

                if (chkVuMeter.Checked)
                    functions |= SignalAnalisysFunction.VUMeter;
                if (chkWaveform.Checked)
                    functions |= SignalAnalisysFunction.Waveform;
                if (chkSpectrogram.Checked)
                    functions |= SignalAnalisysFunction.Spectrogram;
            }

            if (chkWCFInterface.Checked)
            {
                functions |= SignalAnalisysFunction.WCFInterface;
                MediaRenderer.DefaultInstance.InitSignalAnalisysWCF();
            }
            else
            {
                MediaRenderer.DefaultInstance.CleanupSignalAnalisysWCF();
            }

            ProTONEConfig.ShowMediaScreens = mediaScreen;
            ProTONEConfig.SignalAnalisysFunctions = functions;
            

            EventDispatch.DispatchEvent(LocalEventNames.UpdateMediaScreens);
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            chkSpectrogram.Enabled = chkShowSignalAnalisys.Checked;
            chkVuMeter.Enabled = chkShowSignalAnalisys.Checked;
            chkWaveform.Enabled = chkShowSignalAnalisys.Checked;

            base.Modified = true;
        }

    }
}
