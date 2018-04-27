using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OPMedia.Core.TranslationSupport;

namespace OPMedia.Addons.Builtin.Shared.EncoderOptions
{
    public partial class EncoderOptionsCtl : UserControl
    {
        public EncoderSettings EncoderSettings
        {
            get
            {
                EncoderSettings retVal = null;
                if (cmbOutputFormat.SelectedIndex >= 0)
                {
                    EncoderConfiguratorCtl panel = panels[cmbOutputFormat.SelectedIndex];
                    if (panel != null)
                    {
                        retVal = panel.EncoderSettings;
                    }
                }

                return retVal;
            }
        }

        List<EncoderConfiguratorCtl> panels = new List<EncoderConfiguratorCtl>();

        public event EventHandler SettingsChanged = null;
        public void FireSettingsChanged(object sender, EventArgs e)
        {
            if (SettingsChanged != null)
                SettingsChanged(sender, e);
        }

        public void DisplaySettings(bool usedForCdRipper)
        {
            InternalDisplaySettings(usedForCdRipper);
        }

        public EncoderOptionsCtl()
        {
            InitializeComponent();

            cmbOutputFormat.Items.Clear();
            AddPanel(new Mp3EncoderOptionsCtl());
            AddPanel(new WavEncoderOptionsCtl());

            cmbOutputFormat.SelectedIndex = 0;
        }

        private void InternalDisplaySettings(bool usedForCdRipper)
        {
            foreach (EncoderConfiguratorCtl ctl in panels)
            {
                ctl.UsedForCdRipper = usedForCdRipper;
                ctl.Reload();
            }

            ShowPanel(cmbOutputFormat.SelectedIndex);
        }

        private void AddPanel(EncoderConfiguratorCtl panel)
        {
            cmbOutputFormat.Items.Add(panel.EncoderSettings.FormatType);
            panels.Add(panel);
            panel.Visible = false;
            panel.Dock = DockStyle.Fill;
            panel.SettingsChanged += new EventHandler(panel_SettingsChanged);
        }

        void panel_SettingsChanged(object sender, EventArgs e)
        {
            FireSettingsChanged(sender, e);
        }

        private void OnSelectOutputFormat(object sender, EventArgs e)
        {
            ShowPanel(cmbOutputFormat.SelectedIndex);
            FireSettingsChanged(sender, e);
        }

        private void ShowPanel(int index)
        {
            try
            {
                this.SuspendLayout();
                pnlEncoderOptions.SuspendLayout();
                pnlEncoderOptions.Controls.Clear();

                EncoderConfiguratorCtl panel = panels[index];
                if (panel != null)
                {
                    Translator.TranslateControl(panel, false);
                    panel.Visible = true;
                    panel.Dock = DockStyle.Fill;
                    pnlEncoderOptions.Controls.Add(panel);
                }
            }
            finally
            {
                pnlEncoderOptions.ResumeLayout();
                this.ResumeLayout();
            }
        }
    }
}
