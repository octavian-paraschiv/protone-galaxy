using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OPMedia.UI.Dialogs;
using OPMedia.Addons.Builtin.Translations;
using OPMedia.Core.TranslationSupport;
using OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses;
using OPMedia.Addons.Builtin.Shared.Compression;

namespace OPMedia.Addons.Builtin.Shared.EncoderOptions
{
    public partial class Mp3EncoderOptionsCtl : EncoderConfiguratorCtl
    {
        public Mp3EncoderSettings Settings
        {
            get
            {
                return (base.EncoderSettings as Mp3EncoderSettings);
            }
        }

        public Mp3ConversionOptions Options
        {
            get
            {
                return this.Settings.Options;
            }
        }

        public Mp3EncoderOptionsCtl() 
            : base(new Mp3EncoderSettings())
        {
            InitializeComponent();
            this.Load += new EventHandler(Mp3EncoderOptionsCtl_Load);
        }

        void Mp3EncoderOptionsCtl_Load(object sender, EventArgs e)
        {
            Reload();
        }

        private void UpdateSummary(bool fireSettingsChanged = true)
        {
            string summary = "";
            this.Options.GetConfig(ref summary);
            lblOutputBitrateHint.Text = summary;

            if (fireSettingsChanged)
                FireSettingsChanged();
        }

        private void ChangeBitrateModeFieldsVisibility()
        {
            switch (this.Options.BitrateMode)
            {
                case BitrateMode.CBR:
                    lblBitrate.Visible = cmbBitrate.Visible = true;
                    lblPreset.Visible = cmbPreset.Visible = false;
                    lblVbrQuality.Visible = cgVbrQuality.Visible = false;
                    break;

                case BitrateMode.ABR:
                    lblBitrate.Visible = cmbBitrate.Visible = true;
                    lblPreset.Visible = cmbPreset.Visible = false;
                    lblVbrQuality.Visible = cgVbrQuality.Visible = false;
                    break;

                case BitrateMode.Preset:
                    lblBitrate.Visible = cmbBitrate.Visible = false;
                    lblPreset.Visible = cmbPreset.Visible = true;
                    lblVbrQuality.Visible = cgVbrQuality.Visible = false;
                    break;

                case BitrateMode.VBR:
                    lblBitrate.Visible = cmbBitrate.Visible = false;
                    lblPreset.Visible = cmbPreset.Visible = false;
                    lblVbrQuality.Visible = cgVbrQuality.Visible = true;
                    break;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.Options != null)
            {
                string summary = "";
                string conversionFlags = this.Options.GetConfig(ref summary).ToString();

                // TODO fixme
                //LogFileConsoleDetail dlg = new LogFileConsoleDetail(conversionFlags);
                //dlg.Text = "MP3 Conversion Flags";
                //dlg.ShowDialog();
            }
        }

        internal override void  Reload()
        {
            chkGenerateTag.Text = Translator.Translate(UsedForCdRipper ?
                "TXT_GENERATE_TAG" : "TXT_REUSE_TAG");

            // ------------------------------
            // Channel mode options
            cmbChannelMode.Items.Clear();
            foreach (var x in Enum.GetValues(typeof(ChannelMode)))
            {
                switch ((ChannelMode)x)
                {
                    case ChannelMode.JointStereo:
                    case ChannelMode.Stereo:
                        cmbChannelMode.Items.Add(x);
                        break;

                    case ChannelMode.SingleChannel:
                        {
                            // Mono not available if ripping CD tracks.
                            if (UsedForCdRipper == false)
                                cmbChannelMode.Items.Add(x);
                        }
                        break;
                }

            }
            cmbChannelMode.SelectedIndex = cmbChannelMode.FindStringExact(this.Options.ChannelMode.ToString());
            cmbChannelMode.SelectedIndexChanged += (s, a) =>
            {
                this.Options.ChannelMode =
                    (ChannelMode)Enum.Parse(typeof(ChannelMode), cmbChannelMode.Text);
                UpdateSummary();
            };
            //-------------------------

            // ------------------------
            // Bitrate Mode options
            cmbBitrateMode.Items.Clear();
            foreach (var x in Enum.GetValues(typeof(BitrateMode)))
            {
                cmbBitrateMode.Items.Add(x);
            }
            cmbBitrateMode.SelectedIndex = cmbBitrateMode.FindStringExact(this.Options.BitrateMode.ToString());
            ChangeBitrateModeFieldsVisibility();
            cmbBitrateMode.SelectedIndexChanged += (s, a) =>
            {
                this.Options.BitrateMode = (BitrateMode)cmbBitrateMode.SelectedIndex;
                ChangeBitrateModeFieldsVisibility();
                UpdateSummary();
            };
            // ------------------------

            // ------------------------
            // CBR and ABR bit rate
            cmbBitrate.SelectedIndex = cmbBitrate.FindStringExact(this.Options.Bitrate.ToString());
            cmbBitrate.SelectedIndexChanged += (s, a) =>
            {
                this.Options.Bitrate = int.Parse(cmbBitrate.Text);
                UpdateSummary();
            };
            // ------------------------

            // ------------------------
            // VBR "preset-based"
            cmbPreset.Items.Clear();
            foreach (var x in Enum.GetValues(typeof(Preset)))
            {
                cmbPreset.Items.Add(x);
            }
            cmbPreset.SelectedIndex = cmbPreset.FindStringExact(this.Options.VBRPreset.ToString());
            cmbPreset.SelectedIndexChanged += (s, a) =>
            {
                this.Options.VBRPreset =
                    (Preset)Enum.Parse(typeof(Preset), cmbPreset.Text);
                UpdateSummary();
            };
            // ------------------------

            // ------------------------
            // VBR "quality-based"
            cgVbrQuality.Value = this.Options.VBRQuality;
            cgVbrQuality.PositionChanged += (pos) =>
            {
                this.Options.VBRQuality = (int)Math.Round(cgVbrQuality.Value);
                UpdateSummary();
            };
            // ------------------------


            // ------------------------
            // CopyInputFileMetadata
            chkGenerateTag.Checked = this.Settings.CopyInputFileMetadata;
            chkGenerateTag.CheckedChanged += (s, a) =>
            {
                this.Settings.CopyInputFileMetadata = chkGenerateTag.Checked;
                UpdateSummary();
            };
            // ------------------------

            // ------------------------
            // resample frequency
            cmbFrequency.SelectedIndex = cmbFrequency.FindStringExact(this.Options.ResampleFrequency.ToString());
            cmbFrequency.SelectedIndexChanged += (s, a) =>
            {
                this.Options.ResampleFrequency = int.Parse(cmbFrequency.Text);
                UpdateSummary();
            };
            // ------------------------

            UpdateSummary(false);
        }
    }
}
