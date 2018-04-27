using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace OPMedia.UI.Themes
{
    public class SmoothGraphics : IDisposable
    {
        Graphics _g = null;
        Graphics _g2 = null;

        Rectangle _rc = Rectangle.Empty;
        Image _i = null;

        public Graphics Graphics
        {
            get
            {
                return _g2;
            }
        }

        public static SmoothGraphics New(Graphics g, Rectangle rc)
        {
            if (g != null)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.CompositingMode = CompositingMode.SourceOver;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                return new SmoothGraphics(g, rc);
            }

            return null;
        }

        private SmoothGraphics(Graphics g, Rectangle rc)
        {
            _g = g;
            _rc = rc;

            _i = new Bitmap(rc.Width, rc.Height);
            _g2 = Graphics.FromImage(_i);

            _g2.SmoothingMode = SmoothingMode.AntiAlias;
            _g2.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            _g2.CompositingMode = CompositingMode.SourceOver;
            _g2.CompositingQuality = CompositingQuality.HighQuality;
            _g2.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        public void Dispose()
        {
            try
            {
                if (_g != null && _i != null && _rc != Rectangle.Empty)
                    _g.DrawImage(_i, _rc);

                _g2 = null;
                _i = null;
                _g = null;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

        }
    }
}
