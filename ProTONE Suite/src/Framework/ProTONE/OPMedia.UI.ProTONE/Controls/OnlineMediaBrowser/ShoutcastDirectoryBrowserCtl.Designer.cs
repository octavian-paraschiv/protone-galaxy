namespace OPMedia.UI.ProTONE.Controls.OnlineMediaBrowser
{
    partial class ShoutcastDirectoryBrowserCtl
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
            this.lvRadioStations = new OPMedia.UI.Controls.OPMListView();
            this.colEmpty = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colContent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colGenre = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colBitrate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMediaType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lvRadioStations
            // 
            this.lvRadioStations.AllowEditing = false;
            this.lvRadioStations.AlternateRowColors = true;
            this.lvRadioStations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colEmpty,
            this.colName,
            this.colSource,
            this.colURL,
            this.colContent,
            this.colGenre,
            this.colBitrate,
            this.colMediaType});
            this.lvRadioStations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRadioStations.Location = new System.Drawing.Point(0, 0);
            this.lvRadioStations.MultiSelect = false;
            this.lvRadioStations.Name = "lvRadioStations";
            this.lvRadioStations.OverrideBackColor = System.Drawing.Color.Empty;
            this.lvRadioStations.Size = new System.Drawing.Size(617, 395);
            this.lvRadioStations.TabIndex = 1;
            this.lvRadioStations.UseCompatibleStateImageBehavior = false;
            this.lvRadioStations.View = System.Windows.Forms.View.Details;
            // 
            // colEmpty
            // 
            this.colEmpty.Text = "";
            this.colEmpty.Width = 0;
            // 
            // colName
            // 
            this.colName.Text = "TXT_NAME";
            this.colName.Width = 116;
            // 
            // colSource
            // 
            this.colSource.Text = "TXT_SOURCE";
            this.colSource.Width = 119;
            // 
            // colURL
            // 
            this.colURL.Text = "TXT_SERVERURL";
            this.colURL.Width = 113;
            // 
            // colContent
            // 
            this.colContent.Text = "TXT_CONTENT";
            // 
            // colGenre
            // 
            this.colGenre.Text = "TXT_GENRE";
            this.colGenre.Width = 105;
            // 
            // colBitrate
            // 
            this.colBitrate.Text = "TXT_BITRATE";
            this.colBitrate.Width = 82;
            // 
            // colMediaType
            // 
            this.colMediaType.Text = "TXT_MEDIATYPE";
            this.colMediaType.Width = 83;
            // 
            // ShoutcastDirectoryBrowserCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvRadioStations);
            this.Name = "ShoutcastDirectoryBrowserCtl";
            this.Size = new System.Drawing.Size(617, 395);
            this.ResumeLayout(false);

        }

        #endregion

        private UI.Controls.OPMListView lvRadioStations;
        private System.Windows.Forms.ColumnHeader colEmpty;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colSource;
        private System.Windows.Forms.ColumnHeader colURL;
        private System.Windows.Forms.ColumnHeader colContent;
        private System.Windows.Forms.ColumnHeader colGenre;
        private System.Windows.Forms.ColumnHeader colBitrate;
        private System.Windows.Forms.ColumnHeader colMediaType;
    }
}
