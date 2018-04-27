using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OPMedia.Core;
using OPMedia.Core.TranslationSupport;
using OPMedia.UI.Themes;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Timers;
using System.Diagnostics;
using OPMedia.Core.Logging;
using System.Threading;

namespace OPMedia.UI.Controls
{
    public partial class TrayNotificationBox : Form
    {
        private static int __count = 0;

        OPMToolTipData data = null;

        Font _fVal = ThemeManager.SmallFont;
        Font _fKey = new Font(ThemeManager.SmallFont, FontStyle.Bold);
        Font _fTitle = ThemeManager.LargeFont;
        static Font _def = new Font("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.World);

        System.Windows.Forms.Timer _tmrHide = null;
        public int HideDelay { get; set; }

        public void Show(string title, Dictionary<string, string> values = null, Image img = null)
        {
            __count++;

            AssignData(title, values, img);
            
            User32.ShowWindow(Handle, ShowWindowStyles.SW_SHOWNOACTIVATE);
            User32.SetWindowOnTop(Handle, false);

            _tmrHide.Interval = HideDelay;
            _tmrHide.Start();
        }

        int showLocation = 0, startLocation = 0;

        public TrayNotificationBox()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.DoubleBuffered = true;

            this.Opacity = 1;

            HideDelay = 6000;
            this.ShowInTaskbar = false;
            this.Paint += new PaintEventHandler(TrayNotificationBox_Paint);
            this.Click += new EventHandler(TrayNotificationBox_Click);

            _tmrHide = new System.Windows.Forms.Timer();
            _tmrHide.Tick += new EventHandler(_tmrHide_Tick);
        }

        void _tmrHide_Tick(object sender, EventArgs e)
        {
            _tmrHide.Stop();
            _tmrHide.Tick -= new EventHandler(_tmrHide_Tick);

            __count--;
            this.Close();
        }

        void TrayNotificationBox_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("(TEST) TrayNotificationBox_Click");
            _tmrHide_Tick(sender, e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TrayNotificationBox
            // 
            this.ClientSize = new System.Drawing.Size(172, 153);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TrayNotificationBox";
            this.ResumeLayout(false);
        }


        private void AssignData(string title, Dictionary<string, string> values, Image img)
        {
            data = new OPMToolTipData { Values = values, TitleImage = img, Title = title };
            base.Size = CalculateSize(data);
            base.Location = CalculateLocation();
        }

        private Point CalculateLocation()
        {
            int x = 0, y = 0;

            Point mousePosition = MousePosition;

            x = Screen.FromPoint(mousePosition).WorkingArea.Right - base.Size.Width - 1;
            y = Screen.FromPoint(mousePosition).WorkingArea.Bottom - base.Size.Height - 1;

            return new Point(x, y);
        }

        private Size CalculateSize(OPMToolTipData data)
        {
            int x = 0, y = 0;

            if (data != null)
            {
                int titleOffsetX = 5;
                int titleOffsetY = 5;

                using (Graphics g = CreateGraphics())
                {
                    SizeF sizeTitle = g.MeasureString(data.Title, _fTitle);

                    if (data.TitleImage != null)
                    {
                        x += 5 + data.TitleImage.Width;
                        y += 5 + data.TitleImage.Height;

                        titleOffsetX += 5 + data.TitleImage.Width;
                        titleOffsetY = (int)(data.TitleImage.Height / 2 - sizeTitle.Height / 2);

                        y += 5;
                    }
                    else
                    {
                        y += (int)sizeTitle.Height + 10;
                    }

                    if (data.Values != null)
                    {
                        foreach (KeyValuePair<string, string> val in data.Values)
                        {
                            string key = Translator.Translate(val.Key.Trim());
                            string keyMeasure = string.IsNullOrEmpty(key) ? " " : key;
                            string valMeasure = string.IsNullOrEmpty(val.Value) ? " " : val.Value;

                            SizeF szKey = g.MeasureString(keyMeasure, _fKey);
                            SizeF szVal = g.MeasureString(valMeasure, _fVal);

                            y += (int)szKey.Height + 2;
                            x = (int)Math.Max(x, szKey.Width + szVal.Width + 10);
                        }
                    }

                    x = (int)Math.Max(x, titleOffsetX + sizeTitle.Width);
                }
            }

            x += 10;
            y += 10;

            return new Size(x, y);
        }

        void TrayNotificationBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.CompositingQuality = CompositingQuality.GammaCorrected;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (LinearGradientBrush b = new LinearGradientBrush(ClientRectangle,
                ControlPaint.Light(ThemeManager.GradientNormalColor1),
                    ControlPaint.Light(ThemeManager.GradientNormalColor2), 90))
            {
                e.Graphics.FillRectangle(b, ClientRectangle);
            }

            using (Pen p = new Pen(ThemeManager.BorderColor))
            {
                Rectangle rc = new Rectangle(ClientRectangle.Location,
                    new Size(ClientRectangle.Width - 1, ClientRectangle.Height - 1));

                e.Graphics.DrawRectangle(p, rc);
            }

            DrawContents(e.Graphics);
        }

        private void DrawContents(Graphics g)
        {
            if (data != null)
            {
                int titleOffsetX = 5;
                int titleOffsetY = 5;
                int textOffsetY = 5;

                SizeF sizeTitle = g.MeasureString(data.Title, _fTitle);

                if (data.TitleImage != null)
                {
                    g.DrawImage(data.TitleImage, 5, 5);
                    titleOffsetX += 5 + data.TitleImage.Width;
                    titleOffsetY = (int)(data.TitleImage.Height / 2 - sizeTitle.Height / 4);

                    textOffsetY += data.TitleImage.Height + 5;
                }
                else
                {
                    textOffsetY += (int)sizeTitle.Height + 5;
                }

                using (Brush b = new SolidBrush(ThemeManager.ForeColor))
                {
                    g.DrawString(data.Title, _fTitle, b, titleOffsetX, titleOffsetY);

                    int x = 5;
                    int y = textOffsetY;

                    if (data.Values != null)
                    {
                        foreach (KeyValuePair<string, string> val in data.Values)
                        {
                            string key = Translator.Translate(val.Key.Trim());
                            string keyMeasure = string.IsNullOrEmpty(key) ? " " : key;
                            string valMeasure = string.IsNullOrEmpty(val.Value) ? " " : val.Value;

                            SizeF szKey = g.MeasureString(keyMeasure, _fKey);
                            SizeF szVal = g.MeasureString(valMeasure, _fVal);

                            if (!string.IsNullOrEmpty(val.Key))
                            {
                                g.DrawString(key, _fKey, b, x, y);
                            }

                            if (!string.IsNullOrEmpty(val.Value))
                            {
                                g.DrawString(val.Value, _fVal, b, x + szKey.Width, y);
                            }

                            y += (int)Math.Max(szKey.Height, szVal.Height) + 2;
                        }
                    }
                }
            }
        }
    }
}
