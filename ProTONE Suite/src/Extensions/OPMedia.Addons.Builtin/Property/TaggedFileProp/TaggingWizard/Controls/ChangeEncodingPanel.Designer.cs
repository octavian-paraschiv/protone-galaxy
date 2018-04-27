namespace OPMedia.Addons.Builtin.TaggedFileProp.TaggingWizard
{
    partial class ChangeEncodingPanel
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
            this.encoderOptionsCtl = new OPMedia.Addons.Builtin.Shared.EncoderOptions.EncoderOptionsCtl();
            this.SuspendLayout();
            // 
            // encoderOptionsCtl
            // 
            this.encoderOptionsCtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.encoderOptionsCtl.Location = new System.Drawing.Point(0, 0);
            this.encoderOptionsCtl.Name = "encoderOptionsCtl";
            this.encoderOptionsCtl.Size = new System.Drawing.Size(509, 280);
            this.encoderOptionsCtl.TabIndex = 0;
            // 
            // ChangeEncodingPanel
            // 
            this.Controls.Add(this.encoderOptionsCtl);
            this.Name = "ChangeEncodingPanel";
            this.Size = new System.Drawing.Size(509, 280);
            this.ResumeLayout(false);

        }

        #endregion

        private Shared.EncoderOptions.EncoderOptionsCtl encoderOptionsCtl;
    }
}
