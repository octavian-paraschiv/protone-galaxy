using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using OPMedia.UI.Themes;
using System.ComponentModel;
using OPMedia.Core;
using System.Drawing.Drawing2D;
using OPMedia.UI.Generic;
using System.Windows.Forms.VisualStyles;
using System.Reflection;
using OPMedia.UI.Controls;
using OPMedia.Core.TranslationSupport;
using System.Drawing.Text;

namespace OPMedia.UI.Controls
{
    public class OPMComboBox : ComboBox
    {
        protected bool _isHovered = false;

        #region GUI Properties

        #region Font Size

        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Font Font { get { return base.Font; } }

        FontSizes _fontSizes = FontSizes.Normal;
        [DefaultValue(FontSizes.Normal)]
        public FontSizes FontSize
        {
            get { return _fontSizes; }
            set
            {
                ThemeManager.SetFont(this, value);
                _fontSizes = value;

                Invalidate(true);
            }
        }
        #endregion

        #region Override settings

        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Color ForeColor { get { return base.ForeColor; } }

        protected Color _overrideForeColor = Color.Empty;
        public Color OverrideForeColor
        {
            get { return _overrideForeColor; }
            set { _overrideForeColor = value; Invalidate(true); }
        }

        protected Color GetForeColor()
        {
            if (_overrideForeColor != Color.Empty)
                return _overrideForeColor;

            return ThemeManager.WndTextColor;
        }
        #endregion

        #endregion


        public new object SelectedItem
        {
            get { return base.SelectedItem; }
            set { base.SelectedItem = value; }
        }

        [ReadOnly(true)]
        public new ComboBoxStyle DropDownStyle
        {
            get { return base.DropDownStyle; }
        }


        [ReadOnly(true)]
        public new FlatStyle FlatStyle
        {
            get { return base.FlatStyle; }
        }

        protected bool _disableRoundCorners = false;
        public OPMComboBox(bool disableRoundCorners) : this()
        {
            _disableRoundCorners = disableRoundCorners;
        }

        public OPMComboBox()
            : base()
        {
            base.FlatStyle = FlatStyle.Standard;
            base.DropDownStyle = ComboBoxStyle.DropDownList;
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = true;

            this.MouseEnter += new EventHandler(OnMouseEnter);
            this.MouseLeave += new EventHandler(OnMouseLeave);
            this.Enter += new EventHandler(OnEnter);
            this.Leave += new EventHandler(OnLeave);

            this.DrawItem += new DrawItemEventHandler(OPMComboBox_DrawItem);
            this.MeasureItem += new MeasureItemEventHandler(OPMComboBox_MeasureItem);

            this.FontSize = FontSizes.Normal;
        }

        void OPMComboBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {

        }

        void OnLeave(object sender, EventArgs e)
        {
            //_isHovered = false;
            Invalidate(true);
        }
        void OnEnter(object sender, EventArgs e)
        {
            //_isHovered = false;
            Invalidate(true);
        }

        void OnMouseLeave(object sender, EventArgs e)
        {
            _isHovered = false;
            Invalidate(true);
        }

        void OnMouseEnter(object sender, EventArgs e)
        {
            _isHovered = Enabled;
            Invalidate(true);
        }

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            base.OnSelectedValueChanged(e);
            this.Invalidate(true);
        }
        
        protected override void WndProc(ref Message m)
        {
            IntPtr hDC = IntPtr.Zero;
            switch (m.Msg)
            {
                case (int)Messages.WM_NCPAINT:
                    using (Graphics g = GetPreparedGraphics(ref hDC))
                    {
                        User32.SendMessage(this.Handle, (int)Messages.WM_ERASEBKGND, hDC.ToInt32(), 0);
                        SendPrintClientMsg();	// send to draw client area
                        PaintFlatControlBorder(g);
                        User32.ReleaseDC(m.HWnd, hDC);
                    }
                    m.Result = (IntPtr)1;	// indicate msg has been processed			
                    break;
                    
                case (int)Messages.WM_PAINT:
                    base.WndProc(ref m);
                    // flatten the border area again
                    using (Graphics g = GetPreparedGraphics(ref hDC))
                    {
                        PaintFlatControlBorder(g);
                        User32.ReleaseDC(m.HWnd, hDC);
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private Graphics GetPreparedGraphics(ref IntPtr hDC)
        {
            hDC = User32.GetWindowDC(this.Handle);
            Graphics g = Graphics.FromHdc(hDC);
            ThemeManager.PrepareGraphics(g);

            return g;
        }

        //protected override void OnDrawItem(DrawItemEventArgs e)
        void OPMComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index > Items.Count)
                return;

            DrawItemInternal(sender, e);
        }

        protected virtual void DrawItemInternal(object sender, DrawItemEventArgs e)
        {
            Image img = null;
            ComboBoxItem item = this.Items[e.Index] as ComboBoxItem;
            if (item != null)
            {
                img = item.Image;
            }

            Color cText = Enabled ? GetForeColor() : Color.FromKnownColor(KnownColor.ControlDark);

            bool hot = false;
            if (e.State.HasFlag(DrawItemState.Selected) ||
                e.State.HasFlag(DrawItemState.Checked) ||
                e.State.HasFlag(DrawItemState.Focus))
            {
                cText = ThemeManager.WndValidColor;
                hot = true;
            }
            
            ThemeManager.PrepareGraphics(e.Graphics);

            Rectangle rc1 = e.Bounds;
            Rectangle rc2 = e.Bounds;

            rc1.Inflate(1, 1);
            rc2.Inflate(-1, -1);

            using (Brush b1 = new SolidBrush(ThemeManager.WndValidColor))
            using (Brush b2 = new SolidBrush(ThemeManager.SelectedColor))
            {
                e.Graphics.FillRectangle(b1, rc1);

                if (hot)
                {
                    e.Graphics.FillRectangle(b2, rc2);
                }
            }

            using (Brush b = new SolidBrush(cText))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisWord;
                //sf.FormatFlags = StringFormatFlags.NoWrap;

                string text = Items[e.Index].GetFieldValueAsText(DisplayMember);

                if (img != null)
                {
                    e.Graphics.DrawImage(img, rc2.X + 2, rc2.Top, 16, 16);

                    rc2.X += 20;
                    rc2.Width -= 20;
                }

                e.Graphics.DrawString(text, e.Font, b, rc2);
            }
        }

        private void SendPrintClientMsg()
        {
            // We send this message for the control to redraw the client area
            Graphics gClient = this.CreateGraphics();
            IntPtr ptrClientDC = gClient.GetHdc();
            User32.SendMessage(this.Handle, (int)Messages.WM_PRINTCLIENT, ptrClientDC.ToInt32(), 0);
            gClient.ReleaseHdc(ptrClientDC);
            gClient.Dispose();
        }

        protected virtual void PaintFlatControlBorder(Graphics g)
        {
            int pw = 1;
            Color c1 = Color.Empty, c2 = Color.Empty, cb = Color.Empty, cText = Color.Empty;

            c1 = Enabled ? ThemeManager.GradientNormalColor1 : ThemeManager.BackColor;
            c2 = Enabled ? ThemeManager.GradientNormalColor2 : ThemeManager.BackColor;
            cb = Enabled ? ThemeManager.BorderColor : ThemeManager.GradientNormalColor2;
            cText = Enabled ? ThemeManager.ForeColor : Color.FromKnownColor(KnownColor.ControlDark);

            if (Enabled && (_isHovered || Focused))
            {
                if (_isHovered && Focused)
                {
                    c1 = ThemeManager.GradientFocusHoverColor1;
                    c2 = ThemeManager.GradientFocusHoverColor2;
                    cb = ThemeManager.FocusBorderColor;
                    //pw = 2;
                }
                else if (Focused)
                {
                    c1 = ThemeManager.GradientFocusColor1;
                    c2 = ThemeManager.GradientFocusColor2;
                    cb = ThemeManager.FocusBorderColor;
                    //pw = 2;
                }
                else
                {
                    c1 = ThemeManager.GradientHoverColor1;
                    c2 = ThemeManager.GradientHoverColor2;
                    cText = ThemeManager.SelectedTextColor;
                }
            }
           
            if (_overrideForeColor != Color.Empty)
            {
                cText = _overrideForeColor;
            }

            Rectangle rc = ClientRectangle;
            rc.Inflate(2, 2);
            using (Brush b = new SolidBrush(ThemeManager.BackColor))
            {
                g.FillRectangle(b, rc);
            }

            rc = ClientRectangle;
            rc.Width -= 1;
            rc.Height -= 1;

            using (Pen p = new Pen(cb, pw))
            using (Brush b = new LinearGradientBrush(rc, c1, c2, 90))
            {
                g.FillRectangle(b, rc);
                g.DrawRectangle(p, rc);
            }

            rc = new Rectangle(ClientRectangle.Left + 2, ClientRectangle.Top + 2,
                ClientRectangle.Width - 6, ClientRectangle.Height - 6);

            Rectangle rcText = new Rectangle(rc.Left, rc.Top, rc.Width - 12, rc.Height);
            Rectangle rcArrow = new Rectangle(rcText.Right, rc.Top, 12, rc.Height);

            Image img = null;
            ComboBoxItem item = this.SelectedItem as ComboBoxItem;
            if (item != null)
            {
                img = item.Image;
            }

            using (Brush b = new SolidBrush(cText))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                //sf.FormatFlags = StringFormatFlags.NoWrap;

                string text = SelectedItem.GetFieldValueAsText(DisplayMember);

                if (img != null)
                {
                    g.DrawImage(img, rcText.X + 2, rcText.Top, 16, 16);

                    rcText.X += 20;
                    rcText.Width -= 20;
                }

                g.DrawString(text, this.Font, b, rcText, sf);
            }

            using (GraphicsPath gp = ImageProcessing.GenerateCenteredArrow(rcArrow))
            using (Brush b = new SolidBrush(cText))
            using (Pen p = new Pen(b, 1))
            {
                g.FillPath(b, gp);
                g.DrawPath(p, gp);
            }
        }

        public void AddUniqueItem(object item)
        {
            if (!this.Items.Contains(item))
            {
                this.Items.Add(item);
            }
        }
    }

    public class YesNoComboBox : OPMComboBox
    {
        public new bool SelectedValue
        {
            get
            {
                return (SelectedIndex != 0);
            }

            set
            {
                SelectedIndex = value ? 1 : 0;
            }
        }

        public YesNoComboBox()
            : base()
        {
            // Keep this order !!
            Items.Add(Translator.Translate("TXT_NO"));
            Items.Add(Translator.Translate("TXT_YES"));
        }
    }

    public class ComboBoxItem
    {
        public Image Image { get; protected set; }

        public ComboBoxItem(Image img)
        {
            this.Image = img;
        }
    }


    public class FontComboBox : OPMComboBox
    {
        public FontComboBox()
            : base()
        {
            PopulateInstalledFonts();
        }
        
        private void PopulateInstalledFonts()
        {
            FontFamily[] fontFamilies = new InstalledFontCollection().Families;
            foreach (FontFamily ff in fontFamilies)
            {
                this.AddUniqueItem(ff);
            }
        }

        protected override void DrawItemInternal(object sender, DrawItemEventArgs e)
        {
            FontFamily ff = (FontFamily)this.Items[e.Index];
            string text = ff.Name;

            Color cText = Enabled ? base.GetForeColor() : Color.FromKnownColor(KnownColor.ControlDark);

            bool hot = false;
            if (e.State.HasFlag(DrawItemState.Selected) ||
                e.State.HasFlag(DrawItemState.Checked) ||
                e.State.HasFlag(DrawItemState.Focus))
            {
                cText = ThemeManager.WndValidColor;
                hot = true;
            }

            ThemeManager.PrepareGraphics(e.Graphics);

            Rectangle rc1 = e.Bounds;
            Rectangle rc2 = e.Bounds;

            rc1.Inflate(1, 1);
            rc2.Inflate(-1, -1);

            using (Brush b1 = new SolidBrush(ThemeManager.WndValidColor))
            using (Brush b2 = new SolidBrush(ThemeManager.SelectedColor))
            {
                e.Graphics.FillRectangle(b1, rc1);

                if (hot)
                {
                    e.Graphics.FillRectangle(b2, rc2);
                }
            }

            using (Brush b = new SolidBrush(cText))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisWord;
                //sf.FormatFlags = StringFormatFlags.NoWrap;

                Font f = e.Font;
                Font fDraw = new System.Drawing.Font(ff, f.SizeInPoints, f.Style, GraphicsUnit.Point);

                e.Graphics.DrawString(text, fDraw, b, rc2);
            }
        }

        protected override void PaintFlatControlBorder(Graphics g)
        {
            int pw = 1;
            Color c1 = Color.Empty, c2 = Color.Empty, cb = Color.Empty, cText = Color.Empty;

            c1 = Enabled ? ThemeManager.GradientNormalColor1 : ThemeManager.BackColor;
            c2 = Enabled ? ThemeManager.GradientNormalColor2 : ThemeManager.BackColor;
            cb = Enabled ? ThemeManager.BorderColor : ThemeManager.GradientNormalColor2;
            cText = Enabled ? ThemeManager.ForeColor : Color.FromKnownColor(KnownColor.ControlDark);

            if (Enabled && (_isHovered || Focused))
            {
                if (_isHovered && Focused)
                {
                    c1 = ThemeManager.GradientFocusHoverColor1;
                    c2 = ThemeManager.GradientFocusHoverColor2;
                    cb = ThemeManager.FocusBorderColor;
                    //pw = 2;
                }
                else if (Focused)
                {
                    c1 = ThemeManager.GradientFocusColor1;
                    c2 = ThemeManager.GradientFocusColor2;
                    cb = ThemeManager.FocusBorderColor;
                    //pw = 2;
                }
                else
                {
                    c1 = ThemeManager.GradientHoverColor1;
                    c2 = ThemeManager.GradientHoverColor2;
                    cText = ThemeManager.SelectedTextColor;
                }
            }

            if (_overrideForeColor != Color.Empty)
            {
                cText = _overrideForeColor;
            }

            Rectangle rc = ClientRectangle;
            rc.Inflate(2, 2);
            using (Brush b = new SolidBrush(ThemeManager.BackColor))
            {
                g.FillRectangle(b, rc);
            }

            rc = ClientRectangle;
            rc.Width -= 1;
            rc.Height -= 1;

            using (Pen p = new Pen(cb, pw))
            using (Brush b = new LinearGradientBrush(rc, c1, c2, 90))
            {
                g.FillRectangle(b, rc);
                g.DrawRectangle(p, rc);
            }

            rc = new Rectangle(ClientRectangle.Left + 2, ClientRectangle.Top + 2,
                ClientRectangle.Width - 6, ClientRectangle.Height - 6);

            Rectangle rcText = new Rectangle(rc.Left, rc.Top, rc.Width - 12, rc.Height);
            Rectangle rcArrow = new Rectangle(rcText.Right, rc.Top, 12, rc.Height);

            using (Brush b = new SolidBrush(cText))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                //sf.FormatFlags = StringFormatFlags.NoWrap;

                FontFamily ff = SelectedItem as FontFamily;
                if (ff != null)
                {
                    string text = ff.Name;
                    Font fDraw = new System.Drawing.Font(ff, this.Font.SizeInPoints, this.Font.Style, GraphicsUnit.Point);
                    g.DrawString(text, fDraw, b, rcText, sf);
                }
            }

            using (GraphicsPath gp = ImageProcessing.GenerateCenteredArrow(rcArrow))
            using (Brush b = new SolidBrush(cText))
            using (Pen p = new Pen(b, 1))
            {
                g.FillPath(b, gp);
                g.DrawPath(p, gp);
            }
        }
    }


    public class ColorComboBox : OPMComboBox
    {
        public ColorComboBox() : base()
        {
            PopulateKnownColors();
        }
        
        private void PopulateKnownColors()
        {
            KnownColor[] knownColors = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            foreach (KnownColor kc in knownColors)
            {
                Color c = Color.FromName(kc.ToString());
                this.AddUniqueItem(c);
            }
        }

        protected override void DrawItemInternal(object sender, DrawItemEventArgs e)
        {
            Color c = (Color)this.Items[e.Index];

            ThemeManager.PrepareGraphics(e.Graphics);

            Rectangle rc1 = e.Bounds;
            Rectangle rc2 = e.Bounds;

            rc1.Inflate(1, 1);
            rc2.Inflate(-1, -1);

            using (Brush b = new SolidBrush(c))
            {
                e.Graphics.FillRectangle(b, rc1);
            }

            Color cText = ColorHelper.GetContrastingColor(c);

            using (Brush b = new SolidBrush(cText))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisWord;
                //sf.FormatFlags = StringFormatFlags.NoWrap;

                e.Graphics.DrawString(c.Name, e.Font, b, rc2);
            }
        }

        protected override void PaintFlatControlBorder(Graphics g)
        {
            int pw = 1;
            Color c1 = Color.Empty, c2 = Color.Empty, cb = Color.Empty, cText = Color.Empty;

            c1 = Enabled ? ThemeManager.GradientNormalColor1 : ThemeManager.BackColor;
            c2 = Enabled ? ThemeManager.GradientNormalColor2 : ThemeManager.BackColor;
            cb = Enabled ? ThemeManager.BorderColor : ThemeManager.GradientNormalColor2;
            cText = Enabled ? ThemeManager.ForeColor : Color.FromKnownColor(KnownColor.ControlDark);

            if (Enabled && (_isHovered || Focused))
            {
                if (_isHovered && Focused)
                {
                    c1 = ThemeManager.GradientFocusHoverColor1;
                    c2 = ThemeManager.GradientFocusHoverColor2;
                    cb = ThemeManager.FocusBorderColor;
                    //pw = 2;
                }
                else if (Focused)
                {
                    c1 = ThemeManager.GradientFocusColor1;
                    c2 = ThemeManager.GradientFocusColor2;
                    cb = ThemeManager.FocusBorderColor;
                    //pw = 2;
                }
                else
                {
                    c1 = ThemeManager.GradientHoverColor1;
                    c2 = ThemeManager.GradientHoverColor2;
                    cText = ThemeManager.SelectedTextColor;
                }
            }

            if (_overrideForeColor != Color.Empty)
            {
                cText = _overrideForeColor;
            }

            Color c = Color.Empty;
            if (this.SelectedIndex > 0)
            {
                c = (Color)this.Items[this.SelectedIndex];
            }

            Color cString = ColorHelper.GetContrastingColor(c);
            Rectangle rc = ClientRectangle;
            rc.Inflate(2, 2);

            using (Brush b = new SolidBrush(ThemeManager.BackColor))
            {
                g.FillRectangle(b, rc);
            }

            rc = ClientRectangle;
            rc.Width -= 1;
            rc.Height -= 1;

            using (Pen p = new Pen(cb, pw))
            using (Brush b = new LinearGradientBrush(rc, c1, c2, 90))
            {
                g.FillRectangle(b, rc);
                g.DrawRectangle(p, rc);
            }

            rc = new Rectangle(ClientRectangle.Left + 2, ClientRectangle.Top + 2,
                ClientRectangle.Width - 6, ClientRectangle.Height - 6);

            Rectangle rcText = new Rectangle(rc.Left, rc.Top, rc.Width - 12, rc.Height);
            Rectangle rcArrow = new Rectangle(rcText.Right, rc.Top, 12, rc.Height);

            using (Brush b = new SolidBrush(cString))
            using (Brush b2 = new SolidBrush(c))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;

                g.FillRectangle(b2, rcText);
                g.DrawString((c == Color.Empty) ? "" : c.Name, this.Font, b, rcText, sf);
            }

            using (GraphicsPath gp = ImageProcessing.GenerateCenteredArrow(rcArrow))
            using (Brush b = new SolidBrush(cText))
            using (Pen p = new Pen(b, 1))
            {
                g.FillPath(b, gp);
                g.DrawPath(p, gp);
            }
        }
    }
}
