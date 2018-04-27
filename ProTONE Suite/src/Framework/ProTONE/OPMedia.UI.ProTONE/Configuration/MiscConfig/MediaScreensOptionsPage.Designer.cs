using OPMedia.UI.Controls;

namespace OPMedia.UI.ProTONE.Configuration.MiscConfig
{
    partial class MediaScreensOptionsPage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.chkShowPlaylist = new OPMedia.UI.Controls.OPMCheckBox();
            this.chkShowTrackInfo = new OPMedia.UI.Controls.OPMCheckBox();
            this.chkShowSignalAnalisys = new OPMedia.UI.Controls.OPMCheckBox();
            this.chkShowBookmarkInfo = new OPMedia.UI.Controls.OPMCheckBox();
            this.pnlSignalAnalisysOptions = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.chkVuMeter = new OPMedia.UI.Controls.OPMCheckBox();
            this.chkWaveform = new OPMedia.UI.Controls.OPMCheckBox();
            this.chkSpectrogram = new OPMedia.UI.Controls.OPMCheckBox();
            this.chkWCFInterface = new OPMedia.UI.Controls.OPMCheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlSignalAnalisysOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chkShowPlaylist, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkShowTrackInfo, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.chkShowSignalAnalisys, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.chkShowBookmarkInfo, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.pnlSignalAnalisysOptions, 1, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.OverrideBackColor = System.Drawing.Color.Empty;
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(522, 330);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // chkShowPlaylist
            // 
            this.chkShowPlaylist.AutoSize = true;
            this.chkShowPlaylist.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.tableLayoutPanel1.SetColumnSpan(this.chkShowPlaylist, 2);
            this.chkShowPlaylist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkShowPlaylist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkShowPlaylist.Location = new System.Drawing.Point(3, 3);
            this.chkShowPlaylist.Margin = new System.Windows.Forms.Padding(3, 3, 3, 15);
            this.chkShowPlaylist.Name = "chkShowPlaylist";
            this.chkShowPlaylist.OverrideForeColor = System.Drawing.Color.Empty;
            this.chkShowPlaylist.Size = new System.Drawing.Size(516, 19);
            this.chkShowPlaylist.TabIndex = 0;
            this.chkShowPlaylist.Text = "TXT_SHOW_PLAYLIST";
            // 
            // chkShowTrackInfo
            // 
            this.chkShowTrackInfo.AutoSize = true;
            this.chkShowTrackInfo.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.tableLayoutPanel1.SetColumnSpan(this.chkShowTrackInfo, 2);
            this.chkShowTrackInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkShowTrackInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkShowTrackInfo.Location = new System.Drawing.Point(3, 40);
            this.chkShowTrackInfo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 15);
            this.chkShowTrackInfo.Name = "chkShowTrackInfo";
            this.chkShowTrackInfo.OverrideForeColor = System.Drawing.Color.Empty;
            this.chkShowTrackInfo.Size = new System.Drawing.Size(516, 19);
            this.chkShowTrackInfo.TabIndex = 1;
            this.chkShowTrackInfo.Text = "TXT_SHOW_TRACKINFO";
            // 
            // chkShowSignalAnalisys
            // 
            this.chkShowSignalAnalisys.AutoSize = true;
            this.chkShowSignalAnalisys.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.tableLayoutPanel1.SetColumnSpan(this.chkShowSignalAnalisys, 2);
            this.chkShowSignalAnalisys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkShowSignalAnalisys.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkShowSignalAnalisys.Location = new System.Drawing.Point(3, 77);
            this.chkShowSignalAnalisys.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.chkShowSignalAnalisys.Name = "chkShowSignalAnalisys";
            this.chkShowSignalAnalisys.OverrideForeColor = System.Drawing.Color.Empty;
            this.chkShowSignalAnalisys.Size = new System.Drawing.Size(516, 19);
            this.chkShowSignalAnalisys.TabIndex = 2;
            this.chkShowSignalAnalisys.Text = "TXT_SHOW_SIGNALANALISYS";
            // 
            // chkShowBookmarkInfo
            // 
            this.chkShowBookmarkInfo.AutoSize = true;
            this.chkShowBookmarkInfo.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.tableLayoutPanel1.SetColumnSpan(this.chkShowBookmarkInfo, 2);
            this.chkShowBookmarkInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkShowBookmarkInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkShowBookmarkInfo.Location = new System.Drawing.Point(3, 158);
            this.chkShowBookmarkInfo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 15);
            this.chkShowBookmarkInfo.Name = "chkShowBookmarkInfo";
            this.chkShowBookmarkInfo.OverrideForeColor = System.Drawing.Color.Empty;
            this.chkShowBookmarkInfo.Size = new System.Drawing.Size(516, 19);
            this.chkShowBookmarkInfo.TabIndex = 3;
            this.chkShowBookmarkInfo.Text = "TXT_SHOW_BOOKMARKINFO";
            // 
            // pnlSignalAnalisysOptions
            // 
            this.pnlSignalAnalisysOptions.AutoSize = true;
            this.pnlSignalAnalisysOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSignalAnalisysOptions.ColumnCount = 4;
            this.pnlSignalAnalisysOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSignalAnalisysOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSignalAnalisysOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSignalAnalisysOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlSignalAnalisysOptions.Controls.Add(this.chkVuMeter, 0, 0);
            this.pnlSignalAnalisysOptions.Controls.Add(this.chkWaveform, 1, 0);
            this.pnlSignalAnalisysOptions.Controls.Add(this.chkSpectrogram, 2, 0);
            this.pnlSignalAnalisysOptions.Controls.Add(this.chkWCFInterface, 0, 1);
            this.pnlSignalAnalisysOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSignalAnalisysOptions.Location = new System.Drawing.Point(23, 102);
            this.pnlSignalAnalisysOptions.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSignalAnalisysOptions.Name = "pnlSignalAnalisysOptions";
            this.pnlSignalAnalisysOptions.OverrideBackColor = System.Drawing.Color.Empty;
            this.pnlSignalAnalisysOptions.RowCount = 1;
            this.pnlSignalAnalisysOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlSignalAnalisysOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlSignalAnalisysOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlSignalAnalisysOptions.Size = new System.Drawing.Size(499, 53);
            this.pnlSignalAnalisysOptions.TabIndex = 8;
            // 
            // chkVuMeter
            // 
            this.chkVuMeter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkVuMeter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkVuMeter.Location = new System.Drawing.Point(0, 0);
            this.chkVuMeter.Margin = new System.Windows.Forms.Padding(0);
            this.chkVuMeter.Name = "chkVuMeter";
            this.chkVuMeter.OverrideForeColor = System.Drawing.Color.Empty;
            this.chkVuMeter.Size = new System.Drawing.Size(141, 19);
            this.chkVuMeter.TabIndex = 0;
            this.chkVuMeter.Text = "TXT_SHOW_VUMETER";
            this.chkVuMeter.UseVisualStyleBackColor = true;
            // 
            // chkWaveform
            // 
            this.chkWaveform.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkWaveform.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkWaveform.Location = new System.Drawing.Point(141, 0);
            this.chkWaveform.Margin = new System.Windows.Forms.Padding(0);
            this.chkWaveform.Name = "chkWaveform";
            this.chkWaveform.OverrideForeColor = System.Drawing.Color.Empty;
            this.chkWaveform.Size = new System.Drawing.Size(153, 19);
            this.chkWaveform.TabIndex = 1;
            this.chkWaveform.Text = "TXT_SHOW_WAVEFORM";
            this.chkWaveform.UseVisualStyleBackColor = true;
            // 
            // chkSpectrogram
            // 
            this.chkSpectrogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkSpectrogram.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkSpectrogram.Location = new System.Drawing.Point(294, 0);
            this.chkSpectrogram.Margin = new System.Windows.Forms.Padding(0);
            this.chkSpectrogram.Name = "chkSpectrogram";
            this.chkSpectrogram.OverrideForeColor = System.Drawing.Color.Empty;
            this.chkSpectrogram.Size = new System.Drawing.Size(173, 19);
            this.chkSpectrogram.TabIndex = 2;
            this.chkSpectrogram.Text = "TXT_SHOW_SPECTROGRAM";
            this.chkSpectrogram.UseVisualStyleBackColor = true;
            // 
            // chkWCFInterface
            // 
            this.pnlSignalAnalisysOptions.SetColumnSpan(this.chkWCFInterface, 4);
            this.chkWCFInterface.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkWCFInterface.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkWCFInterface.Location = new System.Drawing.Point(0, 19);
            this.chkWCFInterface.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.chkWCFInterface.Name = "chkWCFInterface";
            this.chkWCFInterface.OverrideForeColor = System.Drawing.Color.Empty;
            this.chkWCFInterface.Size = new System.Drawing.Size(499, 19);
            this.chkWCFInterface.TabIndex = 3;
            this.chkWCFInterface.Text = "TXT_ENABLE_WCF_INTERFACE";
            this.chkWCFInterface.UseVisualStyleBackColor = true;
            // 
            // MediaScreensOptionsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MediaScreensOptionsPage";
            this.Size = new System.Drawing.Size(522, 330);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pnlSignalAnalisysOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OPMTableLayoutPanel tableLayoutPanel1;
        private OPMCheckBox chkShowPlaylist;
        private OPMCheckBox chkShowTrackInfo;
        private OPMCheckBox chkShowSignalAnalisys;
        private OPMCheckBox chkShowBookmarkInfo;
        private OPMTableLayoutPanel pnlSignalAnalisysOptions;
        private OPMCheckBox chkVuMeter;
        private OPMCheckBox chkWaveform;
        private OPMCheckBox chkSpectrogram;
        private OPMCheckBox chkWCFInterface;
    }
}
