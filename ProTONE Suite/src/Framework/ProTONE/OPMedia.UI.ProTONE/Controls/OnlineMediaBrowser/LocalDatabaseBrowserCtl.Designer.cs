namespace OPMedia.UI.ProTONE.Controls.OnlineMediaBrowser
{
    partial class LocalDatabaseBrowserCtl
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
            this.colIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAlbum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colArtist = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colGenre = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colBitrate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lvRadioStations
            // 
            this.lvRadioStations.AllowEditing = false;
            this.lvRadioStations.AlternateRowColors = true;
            this.lvRadioStations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIcon,
            this.colName,
            this.colArtist,
            this.colAlbum,
            this.colURL,
            this.colGenre,
            this.colBitrate,
            this.colType});
            this.lvRadioStations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRadioStations.Location = new System.Drawing.Point(0, 0);
            this.lvRadioStations.Margin = new System.Windows.Forms.Padding(0);
            this.lvRadioStations.MultiSelect = false;
            this.lvRadioStations.Name = "lvRadioStations";
            this.lvRadioStations.OverrideBackColor = System.Drawing.Color.Empty;
            this.lvRadioStations.Size = new System.Drawing.Size(711, 412);
            this.lvRadioStations.TabIndex = 1;
            this.lvRadioStations.UseCompatibleStateImageBehavior = false;
            this.lvRadioStations.View = System.Windows.Forms.View.Details;
            // 
            // colIcon
            // 
            this.colIcon.Text = "";
            this.colIcon.Width = 25;
            // 
            // colName
            // 
            this.colName.Text = "TXT_NAME";
            this.colName.Width = 116;
            // 
            // colAlbum
            // 
            this.colAlbum.Text = "TXT_ALBUM";
            // 
            // colArtist
            // 
            this.colArtist.Text = "TXT_ARTIST";
            // 
            // colURL
            // 
            this.colURL.Text = "TXT_SERVERURL";
            this.colURL.Width = 113;
            // 
            // colGenre
            // 
            this.colGenre.Text = "TXT_GENRE";
            this.colGenre.Width = 105;
            // 
            // colBitrate
            // 
            this.colBitrate.Text = "TXT_BITRATE";
            // 
            // colType
            // 
            this.colType.Text = "TXT_MEDIATYPE";
            // 
            // LocalDatabaseBrowserCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvRadioStations);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "LocalDatabaseBrowserCtl";
            this.Size = new System.Drawing.Size(711, 412);
            this.ResumeLayout(false);

        }

        #endregion

        private UI.Controls.OPMListView lvRadioStations;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colURL;
        private System.Windows.Forms.ColumnHeader colGenre;
        private System.Windows.Forms.ColumnHeader colIcon;
        private System.Windows.Forms.ColumnHeader colAlbum;
        private System.Windows.Forms.ColumnHeader colArtist;
        private System.Windows.Forms.ColumnHeader colBitrate;
        private System.Windows.Forms.ColumnHeader colType;
    }
}
