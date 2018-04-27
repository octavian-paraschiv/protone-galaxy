
using System.Windows.Forms;
using System.Drawing;
using OPMedia.UI.Themes;
using OPMedia.Core;
using System.Collections.Generic;

using OPMedia.UI.Controls;
using System;
using OPMedia.Core.GlobalEvents;
using OPMedia.Core.TranslationSupport;
using OPMedia.Core.Logging;

namespace OPMedia.UI.ProTONE
{
    public class TrayNotificationTarget
    {
        private NotifyIcon _notifyIcon = null;
        private bool _tipVisible = false;
        private string _dispMessage = string.Empty;

        private TrayNotificationBox _box = new TrayNotificationBox();

        ~TrayNotificationTarget()
        {
            EventDispatch.UnregisterHandler(this);
        }

        public TrayNotificationTarget(NotifyIcon notifyIcon, Form hostForm)
        {
            _notifyIcon = notifyIcon;
            EventDispatch.RegisterHandler(this);
        }

        private Image GetTrayToolTipIcon(int icon)
        {
            Image dispIcon = null;

            switch (icon)
            {
                case (int)ToolTipIcon.Warning:
                case (int)MessageBoxIcon.Warning:
                    dispIcon = ImageProvider.GetUser32Icon(User32Icon.Warning, true);
                    break;

                case (int)ToolTipIcon.Error:
                case (int)MessageBoxIcon.Error:
                    dispIcon = ImageProvider.GetUser32Icon(User32Icon.Error, true);
                    break;

                case (int)ToolTipIcon.Info:
                case (int)MessageBoxIcon.Information:
                    dispIcon = ImageProvider.GetUser32Icon(User32Icon.Information, true);
                    break;

                default:
                    dispIcon = ImageProvider.ApplicationIconLarge;
                    break;
            }

            return dispIcon;
        }

        [EventSink(EventNames.ShowTrayMessage)]
        public void ShowTrayMessage(string message, string title, int icon)
        {
            string dispMsg = string.Format("{0}_{1}_{2}", message, title, icon);
            Image dispIcon = GetTrayToolTipIcon(icon);

            if (dispMsg != _dispMessage)
            {
                _dispMessage = dispMsg;

                Dictionary<string, string> d = null;
                if (message != null)
                {
                    d = new Dictionary<string, string>();
                    d.Add(message, string.Empty);
                }

                TrayNotificationBox box = new TrayNotificationBox();
                box.HideDelay = 5000;
                box.Show(title, d, dispIcon);
            }
        }
    }
}
