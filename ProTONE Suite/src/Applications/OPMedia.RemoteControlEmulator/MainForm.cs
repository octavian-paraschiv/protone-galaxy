using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OPMedia.UI.Themes;
using RC = OPMedia.Runtime.ProTONE.RemoteControl;
using OPMedia.Runtime.ProTONE.RemoteControl;
using OPMedia.Runtime.Shortcuts;
using OPMedia.Runtime.ProTONE;
using OPMedia.Core.Logging;
using OPMedia.UI.Controls;
using OPMedia.Runtime.InterProcessCommunication;
using OPMedia.Core;
using OPMedia.UI.Generic;
using OPMedia.RemoteControlEmulator.Properties;
using System.ServiceModel;
using OPMedia.Runtime.ProTONE.Rendering;

namespace OPMedia.RemoteControlEmulator
{
    public partial class MainForm : ToolForm
    {
        public MainForm() : base("RCC Emulator")
        {
            InitializeComponent();

            ImageList il = new ImageList();
            il.ImageSize = new System.Drawing.Size(64, 64);
            il.ColorDepth = ColorDepth.Depth32Bit;
            il.TransparentColor = ThemeManager.TransparentColor;

            //il.Images.Add(Resources.player);
            //il.Images.Add(Resources.catalog);
            //il.Images.Add(Resources.ir_remote);

            btnPlayer.ImageList = btnMediaLib.ImageList = btnRemote.ImageList = il;

            btnPlayer.ImageIndex = 0;
            btnMediaLib.ImageIndex = 1;
            btnRemote.ImageIndex = 2;

            Version v = new Version(SuiteVersion.Version);

            lblDesc.Text = string.Format("{0} {1}.{2}",
                Constants.SuiteName, v.Major, v.Minor);

            this.ShowInTaskbar = true;

            ThemeManager.SetFont(opmGroupBox1, FontSizes.NormalBold);

            this.Load += new EventHandler(MainForm_Load);
        }

        protected static ISignalAnalisys _proxy = null;
        protected static Timer _tmrWCFCheck = null;

        void MainForm_Load(object sender, EventArgs e)
        {
            #region API tester tab

            cmbCommandType.Items.Clear();
            foreach (var item in Enum.GetValues(typeof(RC.CommandType)))
            {
                cmbCommandType.Items.Add(item);
            }

            OPMShortcut[] cmds = new OPMShortcut[]
            {
                OPMShortcut.CmdPlayPause,
                OPMShortcut.CmdStop,
                OPMShortcut.CmdPrev,
                OPMShortcut.CmdNext,
        
                // Full Screen
                OPMShortcut.CmdFullScreen,
        
                // Media seek control
                OPMShortcut.CmdFwd,
                OPMShortcut.CmdRew,
        
                // Volume control
                OPMShortcut.CmdVolUp,
                OPMShortcut.CmdVolDn,
            };

            cmbPlaybackCmd.Items.Clear();
            foreach (var item in cmds)
            {
                cmbPlaybackCmd.Items.Add(item);
            }

            cmbCommandType.SelectedIndex = 0;
            cmbDestination.SelectedIndex = 1;

            txtDestinationName.Text = Environment.MachineName;

            #endregion API tester tab

            #region Simulator tab
            int i = 1000;
            foreach (Control ctl in pnlSimulator.Controls)
            {
                OPMButton btn = (ctl as OPMButton);
                if (btn != null)
                {
                    btn.Click += new EventHandler(OnSimulatorClick);
                    btn.Tag = (i++).ToString();
                }

            }
            #endregion

            #region WCF tab
            WCFOpen();
            _tmrWCFCheck = new Timer();
            _tmrWCFCheck.Interval = 2000;
            _tmrWCFCheck.Tick += new EventHandler(_tmrWCFCheck_Tick);
            _tmrWCFCheck.Start();
            #endregion
        }

        private void WCFOpen()
        {
            var myBinding = new WSHttpBinding();
            myBinding.MaxReceivedMessageSize = int.MaxValue;
            myBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;

            var myEndpoint = new EndpointAddress("http://localhost/ProTONESignalAnalisys.svc");
            var myChannelFactory = new ChannelFactory<ISignalAnalisys>(myBinding, myEndpoint);
            _proxy = myChannelFactory.CreateChannel();        
        }

        protected void WCFAbort()
        {
            ((ICommunicationObject)_proxy).Abort();
            _proxy = null;
        }


        #region API tester tab

        private void cmbCommandType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblPlaybackCmd.Visible = false;
            cmbPlaybackCmd.Visible = false;
          

            RC.CommandType cmdType = (RC.CommandType)cmbCommandType.SelectedIndex;
            if (BasicCommand.RequiresArguments(cmdType))
            {
                switch (cmdType)
                {
                    case RC.CommandType.Playback:
                        lblPlaybackCmd.Visible = true;
                        cmbPlaybackCmd.Visible = true;
                        break;
                }
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            txtResult.Text = string.Empty;

            try
            {
                RC.CommandType cmdType = (RC.CommandType)cmbCommandType.SelectedIndex;
                string[] args = null;

                if (BasicCommand.RequiresArguments(cmdType))
                {
                    switch (cmdType)
                    {
                        case RC.CommandType.Playback:
                            args = new string[] { cmbPlaybackCmd.Text };
                            break;
                    }
                }

                string restlt = string.Empty;

                switch (cmbDestination.SelectedIndex)
                {
                    case 0:
                        RemoteControlHelper.SendPlayerCommand(cmdType, args);
                        txtResult.Text = "[ Player commands do not return results. ]";
                        break;

                    case 1:
                        string dest = txtDestinationName.Text;
                        txtResult.Text = RemoteControlHelper.SendServiceCommand(dest, cmdType, args);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                txtResult.Text = ex.Message;
            }
        }

        #endregion

        #region Simulator tab 
        void OnSimulatorClick(object sender, EventArgs e)
        {
            OPMButton btn = (sender as OPMButton);
            if (btn != null)
            {
                //WmCopyDataSender.SendData("EmulatorInputPin", btn.Tag as string);
                IPCRemoteControlProxy.PostRequest("EmulatorInputPin", btn.Tag as string);
            }

            pnlContent.Select();
            pnlContent.Focus();
        }
        #endregion

        #region WCF tab
        void _tmrWCFCheck_Tick(object sender, EventArgs e)
        {
            //SignalAnalisysData data = null;
            //try
            //{
            //    data = _proxy.GetSignalAnalisysData();
            //}
            //catch(Exception ex)
            //{
            //    Logger.LogException(ex);
            //    WCFAbort();
            //    WCFOpen();
            //}

            //if (data != null)
            //    AddText(data.ToString());
            //else
            //    AddText("can't read WCF data ...");
        }

        private void AddText(string p)
        {
            tbWCFDetails.Text += p + "\r\n";
            tbWCFDetails.Select(tbWCFDetails.Text.Length - 2, 1);
        }
        #endregion
    }
}
