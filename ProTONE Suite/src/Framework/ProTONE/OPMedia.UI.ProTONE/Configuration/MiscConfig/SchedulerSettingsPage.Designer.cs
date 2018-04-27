using OPMedia.UI.Controls;
using System.Windows.Forms;

namespace OPMedia.UI.ProTONE.Configuration.MiscConfig
{
    partial class SchedulerSettingsPage
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
            this.wsScheduledEvtDays = new OPMedia.UI.Controls.WeekdaySelector();
            this.grpPlaylistEvt = new OPMedia.UI.Controls.OPMCustomPanel();
            this.opmLayoutPanel3 = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.cmbPlaylistEvtHandler = new OPMedia.UI.Controls.OPMComboBox();
            this.lblCaution = new OPMedia.UI.Controls.OPMLabel();
            this.chkEnablePlaylistEvt = new OPMedia.UI.Controls.OPMCheckBox();
            this.grpScheduledEvt = new OPMedia.UI.Controls.OPMCustomPanel();
            this.opmLayoutPanel4 = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.cmbScheduledEvtHandler = new OPMedia.UI.Controls.OPMComboBox();
            this.label3 = new OPMedia.UI.Controls.OPMLabel();
            this.label2 = new OPMedia.UI.Controls.OPMLabel();
            this.dtpScheduledEvtTime = new OPMedia.UI.Controls.OPMDateTimePicker();
            this.label4 = new OPMedia.UI.Controls.OPMLabel();
            this.chkEnableScheduledEvt = new OPMedia.UI.Controls.OPMCheckBox();
            this.label5 = new OPMedia.UI.Controls.OPMLabel();
            this.nudSchedulerWaitTimerProceed = new OPMedia.UI.Controls.OPMNumericUpDown();
            this.opmLayoutPanel1 = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.lblSep3 = new OPMedia.UI.Controls.OPMLabel();
            this.pnlProceedTimerOptions = new OPMedia.UI.Controls.OPMTableLayoutPanel();
            this.layoutPanel = new OPMedia.UI.Controls.OPMPanel();
            this.label1 = new OPMedia.UI.Controls.OPMLabel();
            this.grpPlaylistEvt.SuspendLayout();
            this.opmLayoutPanel3.SuspendLayout();
            this.grpScheduledEvt.SuspendLayout();
            this.opmLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSchedulerWaitTimerProceed)).BeginInit();
            this.opmLayoutPanel1.SuspendLayout();
            this.pnlProceedTimerOptions.SuspendLayout();
            this.layoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // wsScheduledEvtDays
            // 
            this.wsScheduledEvtDays.AutoSize = true;
            this.wsScheduledEvtDays.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.opmLayoutPanel4.SetColumnSpan(this.wsScheduledEvtDays, 5);
            this.wsScheduledEvtDays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wsScheduledEvtDays.FontSize = OPMedia.UI.Themes.FontSizes.Normal;
            this.wsScheduledEvtDays.Location = new System.Drawing.Point(0, 24);
            this.wsScheduledEvtDays.Margin = new System.Windows.Forms.Padding(0);
            this.wsScheduledEvtDays.MaximumSize = new System.Drawing.Size(4270, 50);
            this.wsScheduledEvtDays.MinimumSize = new System.Drawing.Size(400, 45);
            this.wsScheduledEvtDays.Name = "wsScheduledEvtDays";
            this.wsScheduledEvtDays.OverrideBackColor = System.Drawing.Color.Empty;
            this.wsScheduledEvtDays.Size = new System.Drawing.Size(450, 50);
            this.wsScheduledEvtDays.TabIndex = 3;
            // 
            // grpPlaylistEvt
            // 
            this.grpPlaylistEvt.AutoScroll = true;
            this.grpPlaylistEvt.AutoSize = true;
            this.grpPlaylistEvt.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpPlaylistEvt.BaseColor = System.Drawing.Color.Empty;
            this.grpPlaylistEvt.BorderWidth = 0;
            this.grpPlaylistEvt.Controls.Add(this.opmLayoutPanel3);
            this.grpPlaylistEvt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPlaylistEvt.FontSize = OPMedia.UI.Themes.FontSizes.Normal;
            this.grpPlaylistEvt.HasBorder = true;
            this.grpPlaylistEvt.Highlight = false;
            this.grpPlaylistEvt.Location = new System.Drawing.Point(0, 20);
            this.grpPlaylistEvt.Margin = new System.Windows.Forms.Padding(0);
            this.grpPlaylistEvt.Name = "grpPlaylistEvt";
            this.grpPlaylistEvt.OverrideBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.grpPlaylistEvt.Size = new System.Drawing.Size(450, 40);
            this.grpPlaylistEvt.TabIndex = 1;
            this.grpPlaylistEvt.TabStop = false;
            // 
            // opmLayoutPanel3
            // 
            this.opmLayoutPanel3.AutoSize = true;
            this.opmLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.opmLayoutPanel3.ColumnCount = 3;
            this.opmLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.opmLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.opmLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.opmLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.opmLayoutPanel3.Controls.Add(this.cmbPlaylistEvtHandler, 1, 0);
            this.opmLayoutPanel3.Controls.Add(this.lblCaution, 0, 1);
            this.opmLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opmLayoutPanel3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.opmLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.opmLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.opmLayoutPanel3.Name = "opmLayoutPanel3";
            this.opmLayoutPanel3.OverrideBackColor = System.Drawing.Color.Empty;
            this.opmLayoutPanel3.RowCount = 2;
            this.opmLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.opmLayoutPanel3.Size = new System.Drawing.Size(450, 40);
            this.opmLayoutPanel3.TabIndex = 0;
            // 
            // cmbPlaylistEvtHandler
            // 
            this.cmbPlaylistEvtHandler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbPlaylistEvtHandler.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbPlaylistEvtHandler.FormattingEnabled = true;
            this.cmbPlaylistEvtHandler.Location = new System.Drawing.Point(156, 0);
            this.cmbPlaylistEvtHandler.Margin = new System.Windows.Forms.Padding(0);
            this.cmbPlaylistEvtHandler.MaximumSize = new System.Drawing.Size(180, 0);
            this.cmbPlaylistEvtHandler.MinimumSize = new System.Drawing.Size(180, 0);
            this.cmbPlaylistEvtHandler.Name = "cmbPlaylistEvtHandler";
            this.cmbPlaylistEvtHandler.OverrideForeColor = System.Drawing.Color.Empty;
            this.cmbPlaylistEvtHandler.Size = new System.Drawing.Size(180, 24);
            this.cmbPlaylistEvtHandler.TabIndex = 1;
            // 
            // lblCaution
            // 
            this.lblCaution.AutoSize = true;
            this.opmLayoutPanel3.SetColumnSpan(this.lblCaution, 3);
            this.lblCaution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCaution.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCaution.FontSize = OPMedia.UI.Themes.FontSizes.Small;
            this.lblCaution.Location = new System.Drawing.Point(0, 27);
            this.lblCaution.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblCaution.Name = "lblCaution";
            this.lblCaution.OverrideBackColor = System.Drawing.Color.Empty;
            this.lblCaution.OverrideForeColor = System.Drawing.Color.Empty;
            this.lblCaution.Size = new System.Drawing.Size(450, 13);
            this.lblCaution.TabIndex = 3;
            this.lblCaution.Text = "TXT_PLAYLISTEVT_CAUTION";
            this.lblCaution.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkEnablePlaylistEvt
            // 
            this.chkEnablePlaylistEvt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkEnablePlaylistEvt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkEnablePlaylistEvt.Location = new System.Drawing.Point(0, 0);
            this.chkEnablePlaylistEvt.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.chkEnablePlaylistEvt.Name = "chkEnablePlaylistEvt";
            this.chkEnablePlaylistEvt.OverrideForeColor = System.Drawing.Color.Empty;
            this.chkEnablePlaylistEvt.Size = new System.Drawing.Size(450, 15);
            this.chkEnablePlaylistEvt.TabIndex = 0;
            this.chkEnablePlaylistEvt.Text = "TXT_ENABLE_PLAYLISTEVT";
            // 
            // grpScheduledEvt
            // 
            this.grpScheduledEvt.AutoScroll = true;
            this.grpScheduledEvt.AutoSize = true;
            this.grpScheduledEvt.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpScheduledEvt.BaseColor = System.Drawing.Color.Empty;
            this.grpScheduledEvt.BorderWidth = 0;
            this.grpScheduledEvt.Controls.Add(this.opmLayoutPanel4);
            this.grpScheduledEvt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpScheduledEvt.FontSize = OPMedia.UI.Themes.FontSizes.Normal;
            this.grpScheduledEvt.HasBorder = true;
            this.grpScheduledEvt.Highlight = false;
            this.grpScheduledEvt.Location = new System.Drawing.Point(0, 90);
            this.grpScheduledEvt.Margin = new System.Windows.Forms.Padding(0);
            this.grpScheduledEvt.Name = "grpScheduledEvt";
            this.grpScheduledEvt.OverrideBackColor = System.Drawing.Color.Empty;
            this.grpScheduledEvt.Size = new System.Drawing.Size(450, 98);
            this.grpScheduledEvt.TabIndex = 3;
            this.grpScheduledEvt.TabStop = false;
            // 
            // opmLayoutPanel4
            // 
            this.opmLayoutPanel4.AutoSize = true;
            this.opmLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.opmLayoutPanel4.ColumnCount = 4;
            this.opmLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.opmLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.opmLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.opmLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.opmLayoutPanel4.Controls.Add(this.cmbScheduledEvtHandler, 1, 2);
            this.opmLayoutPanel4.Controls.Add(this.label3, 0, 0);
            this.opmLayoutPanel4.Controls.Add(this.label2, 0, 2);
            this.opmLayoutPanel4.Controls.Add(this.dtpScheduledEvtTime, 1, 0);
            this.opmLayoutPanel4.Controls.Add(this.wsScheduledEvtDays, 0, 1);
            this.opmLayoutPanel4.Controls.Add(this.label4, 2, 0);
            this.opmLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opmLayoutPanel4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.opmLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.opmLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.opmLayoutPanel4.Name = "opmLayoutPanel4";
            this.opmLayoutPanel4.OverrideBackColor = System.Drawing.Color.Empty;
            this.opmLayoutPanel4.RowCount = 4;
            this.opmLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.opmLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.opmLayoutPanel4.Size = new System.Drawing.Size(450, 98);
            this.opmLayoutPanel4.TabIndex = 0;
            // 
            // cmbScheduledEvtHandler
            // 
            this.cmbScheduledEvtHandler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbScheduledEvtHandler.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbScheduledEvtHandler.FormattingEnabled = true;
            this.cmbScheduledEvtHandler.Location = new System.Drawing.Point(156, 74);
            this.cmbScheduledEvtHandler.Margin = new System.Windows.Forms.Padding(0);
            this.cmbScheduledEvtHandler.MaximumSize = new System.Drawing.Size(180, 0);
            this.cmbScheduledEvtHandler.MinimumSize = new System.Drawing.Size(180, 0);
            this.cmbScheduledEvtHandler.Name = "cmbScheduledEvtHandler";
            this.cmbScheduledEvtHandler.OverrideForeColor = System.Drawing.Color.Empty;
            this.cmbScheduledEvtHandler.Size = new System.Drawing.Size(180, 24);
            this.cmbScheduledEvtHandler.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.label3.MinimumSize = new System.Drawing.Size(151, 24);
            this.label3.Name = "label3";
            this.label3.OverrideBackColor = System.Drawing.Color.Empty;
            this.label3.OverrideForeColor = System.Drawing.Color.Empty;
            this.label3.Size = new System.Drawing.Size(151, 24);
            this.label3.TabIndex = 0;
            this.label3.Text = "TXT_SCHEDULEDEVT_TIME";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Location = new System.Drawing.Point(0, 74);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.label2.MinimumSize = new System.Drawing.Size(151, 24);
            this.label2.Name = "label2";
            this.label2.OverrideBackColor = System.Drawing.Color.Empty;
            this.label2.OverrideForeColor = System.Drawing.Color.Empty;
            this.label2.Size = new System.Drawing.Size(151, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "TXT_SCHEDULEDEVT_DESC";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtpScheduledEvtTime
            // 
            this.dtpScheduledEvtTime.CustomFormat = "HH:mm:ss";
            this.dtpScheduledEvtTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpScheduledEvtTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpScheduledEvtTime.Location = new System.Drawing.Point(156, 0);
            this.dtpScheduledEvtTime.Margin = new System.Windows.Forms.Padding(0);
            this.dtpScheduledEvtTime.Name = "dtpScheduledEvtTime";
            this.dtpScheduledEvtTime.ShowUpDown = true;
            this.dtpScheduledEvtTime.Size = new System.Drawing.Size(180, 23);
            this.dtpScheduledEvtTime.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Location = new System.Drawing.Point(346, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label4.Name = "label4";
            this.label4.OverrideBackColor = System.Drawing.Color.Empty;
            this.label4.OverrideForeColor = System.Drawing.Color.Empty;
            this.label4.Size = new System.Drawing.Size(79, 24);
            this.label4.TabIndex = 2;
            this.label4.Text = "TXT_ONDAYS";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkEnableScheduledEvt
            // 
            this.chkEnableScheduledEvt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkEnableScheduledEvt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkEnableScheduledEvt.Location = new System.Drawing.Point(0, 70);
            this.chkEnableScheduledEvt.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.chkEnableScheduledEvt.Name = "chkEnableScheduledEvt";
            this.chkEnableScheduledEvt.OverrideForeColor = System.Drawing.Color.Empty;
            this.chkEnableScheduledEvt.Size = new System.Drawing.Size(450, 15);
            this.chkEnableScheduledEvt.TabIndex = 0;
            this.chkEnableScheduledEvt.Text = "TXT_ENABLE_SCHEDULEDEVT";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.OverrideBackColor = System.Drawing.Color.Empty;
            this.label5.OverrideForeColor = System.Drawing.Color.Empty;
            this.label5.Size = new System.Drawing.Size(211, 23);
            this.label5.TabIndex = 0;
            this.label5.Text = "TXT_SCHEDULERWAITTIMERPROCEED";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudSchedulerWaitTimerProceed
            // 
            this.nudSchedulerWaitTimerProceed.AutoSize = true;
            this.nudSchedulerWaitTimerProceed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudSchedulerWaitTimerProceed.Location = new System.Drawing.Point(216, 0);
            this.nudSchedulerWaitTimerProceed.Margin = new System.Windows.Forms.Padding(0);
            this.nudSchedulerWaitTimerProceed.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudSchedulerWaitTimerProceed.Name = "nudSchedulerWaitTimerProceed";
            this.nudSchedulerWaitTimerProceed.ReadOnly = true;
            this.nudSchedulerWaitTimerProceed.Size = new System.Drawing.Size(35, 23);
            this.nudSchedulerWaitTimerProceed.TabIndex = 1;
            this.nudSchedulerWaitTimerProceed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudSchedulerWaitTimerProceed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // opmLayoutPanel1
            // 
            this.opmLayoutPanel1.AutoSize = true;
            this.opmLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.opmLayoutPanel1.ColumnCount = 1;
            this.opmLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.opmLayoutPanel1.Controls.Add(this.chkEnablePlaylistEvt, 0, 0);
            this.opmLayoutPanel1.Controls.Add(this.grpPlaylistEvt, 0, 1);
            this.opmLayoutPanel1.Controls.Add(this.chkEnableScheduledEvt, 0, 3);
            this.opmLayoutPanel1.Controls.Add(this.grpScheduledEvt, 0, 4);
            this.opmLayoutPanel1.Controls.Add(this.lblSep3, 0, 5);
            this.opmLayoutPanel1.Controls.Add(this.pnlProceedTimerOptions, 0, 6);
            this.opmLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.opmLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.opmLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.opmLayoutPanel1.Name = "opmLayoutPanel1";
            this.opmLayoutPanel1.OverrideBackColor = System.Drawing.Color.Empty;
            this.opmLayoutPanel1.RowCount = 8;
            this.opmLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.opmLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.opmLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.opmLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.opmLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.opmLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.opmLayoutPanel1.Size = new System.Drawing.Size(450, 221);
            this.opmLayoutPanel1.TabIndex = 6;
            // 
            // lblSep3
            // 
            this.lblSep3.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSep3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblSep3.Location = new System.Drawing.Point(0, 188);
            this.lblSep3.Margin = new System.Windows.Forms.Padding(0);
            this.lblSep3.Name = "lblSep3";
            this.lblSep3.OverrideBackColor = System.Drawing.Color.White;
            this.lblSep3.OverrideForeColor = System.Drawing.Color.Empty;
            this.lblSep3.Size = new System.Drawing.Size(450, 1);
            this.lblSep3.TabIndex = 4;
            this.lblSep3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlProceedTimerOptions
            // 
            this.pnlProceedTimerOptions.AutoSize = true;
            this.pnlProceedTimerOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlProceedTimerOptions.ColumnCount = 3;
            this.pnlProceedTimerOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlProceedTimerOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlProceedTimerOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlProceedTimerOptions.Controls.Add(this.nudSchedulerWaitTimerProceed, 1, 0);
            this.pnlProceedTimerOptions.Controls.Add(this.label5, 0, 0);
            this.pnlProceedTimerOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProceedTimerOptions.Location = new System.Drawing.Point(0, 198);
            this.pnlProceedTimerOptions.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProceedTimerOptions.Name = "pnlProceedTimerOptions";
            this.pnlProceedTimerOptions.OverrideBackColor = System.Drawing.Color.Empty;
            this.pnlProceedTimerOptions.RowCount = 1;
            this.pnlProceedTimerOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlProceedTimerOptions.Size = new System.Drawing.Size(450, 23);
            this.pnlProceedTimerOptions.TabIndex = 5;
            // 
            // layoutPanel
            // 
            this.layoutPanel.AutoScroll = true;
            this.layoutPanel.Controls.Add(this.opmLayoutPanel1);
            this.layoutPanel.Location = new System.Drawing.Point(0, 0);
            this.layoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.layoutPanel.MinimumSize = new System.Drawing.Size(450, 310);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.Size = new System.Drawing.Size(450, 310);
            this.layoutPanel.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.label1.MinimumSize = new System.Drawing.Size(151, 24);
            this.label1.Name = "label1";
            this.label1.OverrideBackColor = System.Drawing.Color.Empty;
            this.label1.OverrideForeColor = System.Drawing.Color.Empty;
            this.label1.Size = new System.Drawing.Size(151, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "TXT_PLAYLISTEVT_DESC";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SchedulerSettingsPage
            // 
            this.Controls.Add(this.layoutPanel);
            this.Name = "SchedulerSettingsPage";
            this.Size = new System.Drawing.Size(1075, 635);
            this.grpPlaylistEvt.ResumeLayout(false);
            this.grpPlaylistEvt.PerformLayout();
            this.opmLayoutPanel3.ResumeLayout(false);
            this.opmLayoutPanel3.PerformLayout();
            this.grpScheduledEvt.ResumeLayout(false);
            this.grpScheduledEvt.PerformLayout();
            this.opmLayoutPanel4.ResumeLayout(false);
            this.opmLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSchedulerWaitTimerProceed)).EndInit();
            this.opmLayoutPanel1.ResumeLayout(false);
            this.opmLayoutPanel1.PerformLayout();
            this.pnlProceedTimerOptions.ResumeLayout(false);
            this.pnlProceedTimerOptions.PerformLayout();
            this.layoutPanel.ResumeLayout(false);
            this.layoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private WeekdaySelector wsScheduledEvtDays;
        private OPMCustomPanel grpPlaylistEvt;
        private OPMCheckBox chkEnablePlaylistEvt;
        private OPMComboBox cmbPlaylistEvtHandler;
        private OPMCustomPanel grpScheduledEvt;
        private OPMComboBox cmbScheduledEvtHandler;
        private OPMLabel label2;
        private OPMLabel label3;
        private OPMLabel label4;
        private OPMDateTimePicker dtpScheduledEvtTime;
        private OPMCheckBox chkEnableScheduledEvt;
        private OPMLabel label5;
        private OPMNumericUpDown nudSchedulerWaitTimerProceed;
        private OPMTableLayoutPanel opmLayoutPanel1;
        private OPMTableLayoutPanel pnlProceedTimerOptions;
        private OPMTableLayoutPanel opmLayoutPanel3;
        private OPMTableLayoutPanel opmLayoutPanel4;
        private OPMLabel lblSep3;
        private OPMLabel lblCaution;
        private OPMPanel layoutPanel;
        private OPMLabel label1;
    }
}
