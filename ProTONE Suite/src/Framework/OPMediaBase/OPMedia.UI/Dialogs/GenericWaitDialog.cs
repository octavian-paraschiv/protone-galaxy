using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OPMedia.UI.Controls;
using OPMedia.Core.TranslationSupport;
using System.IO;
using OPMedia.Core;
using OPMedia.UI.Themes;
using System.Diagnostics;
using System.Threading;

namespace OPMedia.UI.Dialogs
{
    public partial class GenericWaitDialog : ToolForm, IDisposable
    {
        public void SetText(string msg)
        {
            this.lblNotifyText.Text = Translator.Translate(msg);
        }

        public DialogResult ShowDialog(string message, Form parent = null)
        {
            SetText(message);

            if (parent == null)
                return ShowDialog();

            return ShowDialog(parent);
        }

        public GenericWaitDialog()
        {
            InitializeComponent();

            this.TitleBarVisible = false;
        }

        protected override bool AllowCloseOnKeyDown(Keys key)
        {
            return false;
        }
    }

    public class CancellableWaitDialog : GenericWaitDialog
    {
        public bool EscapePressed { get; private set; }

        protected override bool AllowCloseOnKeyDown(Keys key)
        {
            this.EscapePressed = (key == Keys.Escape);
            return this.EscapePressed;
        }
    }
}