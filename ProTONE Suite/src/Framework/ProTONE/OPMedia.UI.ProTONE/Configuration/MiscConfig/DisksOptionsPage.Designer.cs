using OPMedia.UI.Controls;

namespace OPMedia.UI.ProTONE.Configuration.MiscConfig
{
    partial class DisksOptionsPage
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
            this.opmTableLayoutPanel1 = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.dvdGroupBox = new OPMedia.UI.Controls.OPMGroupBox();
            this.opmTableLayoutPanel2 = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.cbDisableDVDMenu = new OPMedia.UI.Controls.OPMCheckBox();
            this.opmGroupBox1 = new OPMedia.UI.Controls.OPMGroupBox();
            this.opmTableLayoutPanel3 = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.opmLabel1 = new OPMedia.UI.Controls.OPMLabel();
            this.cmbAudioCdInfoSource = new OPMedia.UI.Controls.OPMComboBox();
            this.lblCddbServerName = new OPMedia.UI.Controls.OPMLabel();
            this.lblCddbServerPort = new OPMedia.UI.Controls.OPMLabel();
            this.txtCddbServerName = new OPMedia.UI.Controls.OPMTextBox();
            this.txtCddbServerPort = new OPMedia.UI.Controls.OPMNumericTextBox();
            this.opmTableLayoutPanel1.SuspendLayout();
            this.dvdGroupBox.SuspendLayout();
            this.opmTableLayoutPanel2.SuspendLayout();
            this.opmGroupBox1.SuspendLayout();
            this.opmTableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // opmTableLayoutPanel1
            // 
            this.opmTableLayoutPanel1.ColumnCount = 1;
            this.opmTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.opmTableLayoutPanel1.Controls.Add(this.dvdGroupBox, 0, 1);
            this.opmTableLayoutPanel1.Controls.Add(this.opmGroupBox1, 0, 0);
            this.opmTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opmTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.opmTableLayoutPanel1.Name = "opmTableLayoutPanel1";
            this.opmTableLayoutPanel1.OverrideBackColor = System.Drawing.Color.Empty;
            this.opmTableLayoutPanel1.RowCount = 3;
            this.opmTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.opmTableLayoutPanel1.Size = new System.Drawing.Size(545, 376);
            this.opmTableLayoutPanel1.TabIndex = 0;
            // 
            // dvdGroupBox
            // 
            this.dvdGroupBox.AutoSize = true;
            this.dvdGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dvdGroupBox.Controls.Add(this.opmTableLayoutPanel2);
            this.dvdGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dvdGroupBox.Location = new System.Drawing.Point(3, 119);
            this.dvdGroupBox.Name = "dvdGroupBox";
            this.dvdGroupBox.Size = new System.Drawing.Size(539, 53);
            this.dvdGroupBox.TabIndex = 5;
            this.dvdGroupBox.TabStop = false;
            this.dvdGroupBox.Text = "DVD";
            // 
            // opmTableLayoutPanel2
            // 
            this.opmTableLayoutPanel2.AutoSize = true;
            this.opmTableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.opmTableLayoutPanel2.ColumnCount = 1;
            this.opmTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.opmTableLayoutPanel2.Controls.Add(this.cbDisableDVDMenu, 0, 0);
            this.opmTableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opmTableLayoutPanel2.Location = new System.Drawing.Point(3, 19);
            this.opmTableLayoutPanel2.Name = "opmTableLayoutPanel2";
            this.opmTableLayoutPanel2.OverrideBackColor = System.Drawing.Color.Empty;
            this.opmTableLayoutPanel2.RowCount = 1;
            this.opmTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmTableLayoutPanel2.Size = new System.Drawing.Size(533, 31);
            this.opmTableLayoutPanel2.TabIndex = 0;
            // 
            // cbDisableDVDMenu
            // 
            this.cbDisableDVDMenu.AccessibleName = "cbDisableDVDMenu";
            this.cbDisableDVDMenu.AutoSize = true;
            this.cbDisableDVDMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbDisableDVDMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbDisableDVDMenu.Location = new System.Drawing.Point(0, 0);
            this.cbDisableDVDMenu.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.cbDisableDVDMenu.Name = "cbDisableDVDMenu";
            this.cbDisableDVDMenu.OverrideForeColor = System.Drawing.Color.Empty;
            this.cbDisableDVDMenu.Size = new System.Drawing.Size(533, 19);
            this.cbDisableDVDMenu.TabIndex = 3;
            this.cbDisableDVDMenu.Text = "TXT_SKIP_DVD_MENU";
            // 
            // opmGroupBox1
            // 
            this.opmGroupBox1.AutoSize = true;
            this.opmGroupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.opmGroupBox1.Controls.Add(this.opmTableLayoutPanel3);
            this.opmGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opmGroupBox1.Location = new System.Drawing.Point(3, 3);
            this.opmGroupBox1.Name = "opmGroupBox1";
            this.opmGroupBox1.Size = new System.Drawing.Size(539, 110);
            this.opmGroupBox1.TabIndex = 4;
            this.opmGroupBox1.TabStop = false;
            this.opmGroupBox1.Text = "CD";
            // 
            // opmTableLayoutPanel3
            // 
            this.opmTableLayoutPanel3.AutoSize = true;
            this.opmTableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.opmTableLayoutPanel3.ColumnCount = 2;
            this.opmTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.opmTableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.opmTableLayoutPanel3.Controls.Add(this.opmLabel1, 0, 0);
            this.opmTableLayoutPanel3.Controls.Add(this.cmbAudioCdInfoSource, 1, 0);
            this.opmTableLayoutPanel3.Controls.Add(this.lblCddbServerName, 0, 1);
            this.opmTableLayoutPanel3.Controls.Add(this.lblCddbServerPort, 0, 2);
            this.opmTableLayoutPanel3.Controls.Add(this.txtCddbServerName, 1, 1);
            this.opmTableLayoutPanel3.Controls.Add(this.txtCddbServerPort, 1, 2);
            this.opmTableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opmTableLayoutPanel3.Location = new System.Drawing.Point(3, 19);
            this.opmTableLayoutPanel3.Name = "opmTableLayoutPanel3";
            this.opmTableLayoutPanel3.OverrideBackColor = System.Drawing.Color.Empty;
            this.opmTableLayoutPanel3.RowCount = 4;
            this.opmTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmTableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmTableLayoutPanel3.Size = new System.Drawing.Size(533, 88);
            this.opmTableLayoutPanel3.TabIndex = 0;
            // 
            // opmLabel1
            // 
            this.opmLabel1.AutoSize = true;
            this.opmLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opmLabel1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.opmLabel1.Location = new System.Drawing.Point(3, 0);
            this.opmLabel1.Name = "opmLabel1";
            this.opmLabel1.OverrideBackColor = System.Drawing.Color.Empty;
            this.opmLabel1.OverrideForeColor = System.Drawing.Color.Empty;
            this.opmLabel1.Size = new System.Drawing.Size(161, 30);
            this.opmLabel1.TabIndex = 0;
            this.opmLabel1.Text = "TXT_AUDIOCDINFO_SOURCE";
            this.opmLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbAudioCdInfoSource
            // 
            this.cmbAudioCdInfoSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbAudioCdInfoSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbAudioCdInfoSource.FormattingEnabled = true;
            this.cmbAudioCdInfoSource.Location = new System.Drawing.Point(170, 3);
            this.cmbAudioCdInfoSource.Name = "cmbAudioCdInfoSource";
            this.cmbAudioCdInfoSource.OverrideForeColor = System.Drawing.Color.Empty;
            this.cmbAudioCdInfoSource.Size = new System.Drawing.Size(360, 24);
            this.cmbAudioCdInfoSource.TabIndex = 1;
            // 
            // lblCddbServerName
            // 
            this.lblCddbServerName.AutoSize = true;
            this.lblCddbServerName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCddbServerName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCddbServerName.Location = new System.Drawing.Point(3, 30);
            this.lblCddbServerName.Name = "lblCddbServerName";
            this.lblCddbServerName.OverrideBackColor = System.Drawing.Color.Empty;
            this.lblCddbServerName.OverrideForeColor = System.Drawing.Color.Empty;
            this.lblCddbServerName.Size = new System.Drawing.Size(161, 26);
            this.lblCddbServerName.TabIndex = 2;
            this.lblCddbServerName.Text = "TXT_CDDB_SERVERNAME";
            this.lblCddbServerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCddbServerPort
            // 
            this.lblCddbServerPort.AutoSize = true;
            this.lblCddbServerPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCddbServerPort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCddbServerPort.Location = new System.Drawing.Point(3, 56);
            this.lblCddbServerPort.Name = "lblCddbServerPort";
            this.lblCddbServerPort.OverrideBackColor = System.Drawing.Color.Empty;
            this.lblCddbServerPort.OverrideForeColor = System.Drawing.Color.Empty;
            this.lblCddbServerPort.Size = new System.Drawing.Size(161, 32);
            this.lblCddbServerPort.TabIndex = 3;
            this.lblCddbServerPort.Text = "TXT_CDDB_SERVERPORT";
            this.lblCddbServerPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCddbServerName
            // 
            this.txtCddbServerName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.txtCddbServerName.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtCddbServerName.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtCddbServerName.FontSize = OPMedia.UI.Themes.FontSizes.Normal;
            this.txtCddbServerName.Location = new System.Drawing.Point(167, 36);
            this.txtCddbServerName.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.txtCddbServerName.MaximumSize = new System.Drawing.Size(2000, 20);
            this.txtCddbServerName.MaxLength = 32767;
            this.txtCddbServerName.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtCddbServerName.Multiline = false;
            this.txtCddbServerName.Name = "txtCddbServerName";
            this.txtCddbServerName.OverrideBackColor = System.Drawing.Color.Empty;
            this.txtCddbServerName.OverrideForeColor = System.Drawing.Color.Empty;
            this.txtCddbServerName.Padding = new System.Windows.Forms.Padding(3);
            this.txtCddbServerName.PasswordChar = '\0';
            this.txtCddbServerName.ReadOnly = false;
            this.txtCddbServerName.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtCddbServerName.ShortcutsEnabled = true;
            this.txtCddbServerName.Size = new System.Drawing.Size(366, 20);
            this.txtCddbServerName.TabIndex = 4;
            this.txtCddbServerName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtCddbServerName.UseSystemPasswordChar = false;
            this.txtCddbServerName.WordWrap = true;
            // 
            // txtCddbServerPort
            // 
            this.txtCddbServerPort.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.txtCddbServerPort.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCddbServerPort.FontSize = OPMedia.UI.Themes.FontSizes.Normal;
            this.txtCddbServerPort.Location = new System.Drawing.Point(167, 62);
            this.txtCddbServerPort.Margin = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.txtCddbServerPort.MaximumSize = new System.Drawing.Size(2000, 20);
            this.txtCddbServerPort.MaxLength = 5;
            this.txtCddbServerPort.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtCddbServerPort.Multiline = false;
            this.txtCddbServerPort.Name = "txtCddbServerPort";
            this.txtCddbServerPort.NumBase = OPMedia.UI.Controls.NumberingBase.Base10;
            this.txtCddbServerPort.OverrideBackColor = System.Drawing.Color.Empty;
            this.txtCddbServerPort.OverrideForeColor = System.Drawing.Color.Empty;
            this.txtCddbServerPort.Padding = new System.Windows.Forms.Padding(3);
            this.txtCddbServerPort.PasswordChar = '\0';
            this.txtCddbServerPort.ReadOnly = false;
            this.txtCddbServerPort.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtCddbServerPort.ShortcutsEnabled = true;
            this.txtCddbServerPort.Size = new System.Drawing.Size(117, 20);
            this.txtCddbServerPort.TabIndex = 6;
            this.txtCddbServerPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCddbServerPort.UseSystemPasswordChar = false;
            this.txtCddbServerPort.WordWrap = true;
            // 
            // DisksOptionsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.opmTableLayoutPanel1);
            this.Name = "DisksOptionsPage";
            this.Size = new System.Drawing.Size(545, 376);
            this.opmTableLayoutPanel1.ResumeLayout(false);
            this.opmTableLayoutPanel1.PerformLayout();
            this.dvdGroupBox.ResumeLayout(false);
            this.dvdGroupBox.PerformLayout();
            this.opmTableLayoutPanel2.ResumeLayout(false);
            this.opmTableLayoutPanel2.PerformLayout();
            this.opmGroupBox1.ResumeLayout(false);
            this.opmGroupBox1.PerformLayout();
            this.opmTableLayoutPanel3.ResumeLayout(false);
            this.opmTableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private OPMTableLayoutPanel opmTableLayoutPanel1;
        private OPMCheckBox cbDisableDVDMenu;
        private OPMGroupBox dvdGroupBox;
        private OPMTableLayoutPanel opmTableLayoutPanel2;
        private OPMGroupBox opmGroupBox1;
        private OPMTableLayoutPanel opmTableLayoutPanel3;
        private OPMLabel opmLabel1;
        private OPMComboBox cmbAudioCdInfoSource;
        private OPMLabel lblCddbServerName;
        private OPMLabel lblCddbServerPort;
        private OPMTextBox txtCddbServerName;
        private OPMNumericTextBox txtCddbServerPort;

    }
}
