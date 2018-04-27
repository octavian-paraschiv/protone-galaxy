using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using OPMedia.UI.Themes;
using System.Drawing.Drawing2D;

namespace OPMedia.UI.Controls
{
    public class GraphPlotter : OPMDoubleBufferedControl
    {
        private List<double[]> _dataSets = new List<double[]>();
        private List<Color> _dataSetsColors = new List<Color>();

        public bool ShowXAxis { get; set; }
        public bool ShowYAxis { get; set; }

        public bool LogarithmicXAxis { get; set; }
        public bool LogarithmicYAxis { get; set; }

        public bool IsHistogram { get; set; }
        public bool ShowDecadicLines { get; set; }

        public double? MinVal { get; set; }
        public double? MaxVal { get; set; }

        public void Reset(bool redraw)
        {
            bool wasEmpty = (_dataSets.Count < 1);

            _dataSets.Clear();
            _dataSetsColors.Clear();

            if (redraw)
            {
                Invalidate();
            }
        }

        public void AddDataRange(double[] data, Color color)
        {
            _dataSets.Add(data);
            _dataSetsColors.Add(color);
            Invalidate();
        }

        public GraphPlotter()
        {
        }

        protected override void OnRenderGraphics(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            ThemeManager.PrepareGraphics(g);

            Rectangle rc = this.ClientRectangle;

            using (Brush b = new SolidBrush(ThemeManager.BackColor))
                g.FillRectangle(b, rc);

            rc.Inflate(-1, -1);
            if (IsHistogram)
                rc.Height -= 15;

            using (Pen p = new Pen(ThemeManager.ForeColor))
            using (Pen p2 = new Pen(ThemeManager.ForeColor))
            {
                for (int i = 0; i < _dataSets.Count; i++)
                {
                    DrawDataSet(g, rc, _dataSets[i], _dataSetsColors[i]);
                }

                p2.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                g.DrawRectangle(p2, rc);

                if (ShowXAxis)
                {
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                    g.DrawLine(p,
                        rc.Left, rc.Top + rc.Height / 2,
                        rc.Right, rc.Top + rc.Height / 2);
                }

                if (ShowYAxis)
                {
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                    g.DrawLine(p,
                        rc.Left + rc.Width / 2, rc.Top,
                        rc.Left + rc.Width / 2, rc.Bottom);
                }

                if (IsHistogram && ShowDecadicLines)
                    DrawDecadicLines(g, rc);
            }
        }

        protected virtual void DrawDecadicLines(Graphics g, Rectangle rc)
        {
        }

        private void DrawDataSet(Graphics g, Rectangle rc, double[] data, Color color)
        {
            double min = MinVal.GetValueOrDefault();
            double max = MaxVal.GetValueOrDefault();

            int dataSetLen = data.Length;

            Point last = new Point(rc.Left, 
                (max == min) ? rc.Bottom - rc.Height / 2 :
                rc.Bottom - (int)((data[0] - min) * rc.Height / (max - min)));

            for (double i = 0; i < data.Length; i++)
            {
                try
                {
                    Point pt = Point.Empty;

                    double val = Math.Min(max, Math.Max(min, data[(int)i]));

                    int y =
                        (max == min) ? rc.Bottom - rc.Height / 2 :
                        rc.Bottom - (int)((val - min) * rc.Height / (max - min));

                    int w = (int)Math.Round((float)rc.Width / (float)dataSetLen);
                    if (w < 1)
                        w = 1;

                    int x = 0;
                    if (LogarithmicXAxis)
                    {
                        double logXDomain = Math.Abs(Math.Log((double)1 / data.Length));
                        x = rc.Left + (int)((logXDomain + Math.Log(i / data.Length)) * rc.Width / logXDomain);
                    }
                    else
                    {
                        x = rc.Left + (int)(i * rc.Width / data.Length);
                    }

                    pt = new Point(x, y);

                    if (IsHistogram)
                    {
                        if (color == Color.Transparent)
                        {
                            DrawCustomHistoBar(g, rc, w, pt);
                        }
                        else
                        {
                            using (Brush b = new SolidBrush(color))
                            {
                                Rectangle rcBar = new Rectangle(pt.X - w, pt.Y, w, rc.Bottom - pt.Y);
                                g.FillRectangle(b, rcBar);
                            }
                        }
                    }
                    else
                    {
                        using (Pen pen = new Pen(color, 1.51f))
                        {
                            g.DrawLine(pen, last, pt);
                        }

                        last = pt;
                    }
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }

            }
        }

        protected virtual void DrawCustomHistoBar(Graphics g, Rectangle rc, int w, Point pt)
        {
            Color clStart1 = ThemeManager.GradientGaugeColor1;
            Color clEnd1 = ThemeManager.GradientGaugeColor2;
            using (Brush b = new LinearGradientBrush(rc, clStart1, clEnd1, -90f))
            {
                Rectangle rcBar = new Rectangle(pt.X - w, pt.Y, w, rc.Bottom - pt.Y);
                g.FillRectangle(b, rcBar);
            }
        }
    }
}
