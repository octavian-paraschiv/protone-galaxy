using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using OPMedia.Core;

namespace OPMedia.UI.Generic
{
    [Flags]
    public enum CornersPosition
    {
        None = 0x00,
        LeftTop = 0x01,
        RightTop = 0x02,
        RightBottom = 0x04,
        LeftBottom = 0x08,
        All = 0x0F
    }

    public class ImageProcessing
    {
        public static Image AppIcon
        {
            get
            {
                var img = ImageProvider.GetAppIcon(true).ToBitmap();
                //var img = ImageProvider.ApplicationIconLarge as Bitmap;

                ImageProcessing.GrayToBlack(img); 

                return img;
            }
        }

        public static Image AppIcon16
        {
            get
            {
                return AppIcon.Resize(false);
            }
        }

        public static Image Subtitle
        {
            get
            {
                var img = OPMedia.Core.Properties.Resources.Subtitle.ToBitmap();
                ImageProcessing.GrayToBlack(img);
                return img;
            }
        }

        public static Image Subtitle16
        {
            get
            {
                return Subtitle.Resize(false);
            }
        }


        public static Image AudioFile
        {
            get
            {
                var img = OPMedia.Core.Properties.Resources.AudioFile.ToBitmap();
                ImageProcessing.GrayToBlack(img);
                return img;
            }
        }

        public static Image AudioFile16
        {
            get
            {
                return AudioFile.Resize(false);
            }
        }

        public static Image VideoFile
        {
            get
            {
                var img = OPMedia.Core.Properties.Resources.VideoFile.ToBitmap();
                ImageProcessing.GrayToBlack(img);
                return img;
            }
        }

        public static Image VideoFile16
        {
            get
            {
                return VideoFile.Resize(false);
            }
        }

        public static Image Library
        {
            get
            {
                var img = OPMedia.Core.Properties.Resources.Library.ToBitmap();
                ImageProcessing.GrayToBlack(img);
                return img;
            }
        }

        public static Image Library16
        {
            get
            {
                return Library.Resize(false);
            }
        }

        public static Image Player
        {
            get
            {
                var img = OPMedia.Core.Properties.Resources.player.ToBitmap();
                ImageProcessing.GrayToBlack(img);
                return img;
            }
        }

        public static Image Player16
        {
            get
            {
                return Player.Resize(false);
            }
        }


        public static Image Playlist
        {
            get
            {
                var img = OPMedia.Core.Properties.Resources.Playlist.ToBitmap();
                ImageProcessing.GrayToBlack(img);
                return img;
            }
        }

        public static Image Playlist16
        {
            get
            {
                return Playlist.Resize(false);
            }
        }


        public static Image Bookmark
        {
            get
            {
                var img = OPMedia.Core.Properties.Resources.bookmark.ToBitmap();
                ImageProcessing.GrayToBlack(img);
                return img;
            }
        }

        public static Image Bookmark16
        {
            get
            {
                return Bookmark.Resize(false);
            }
        }

        public static Image CDA
        {
            get
            {
                var img = OPMedia.Core.Properties.Resources.CDA;
                ImageProcessing.GrayToBlack(img);
                return img;
            }
        }

        public static Image CDA16
        {
            get
            {
                return CDA.Resize(false);
            }
        }

        public static Image DVD
        {
            get
            {
                var img = OPMedia.Core.Properties.Resources.DVD;
                ImageProcessing.GrayToBlack(img);
                return img;
            }
        }

        public static Image DVD16
        {
            get
            {
                return CDA.Resize(false);
            }
        }


        public static Bitmap Brightness(Image b, float brightness)
        {
            Bitmap bDest = new Bitmap(b.Width, b.Height, b.PixelFormat);

            float adjustedBrightness = brightness - 1.0f;

            // Create the ImageAttributes object and apply the ColorMatrix
            ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
            ColorMatrix brightnessMatrix = new ColorMatrix(new float[][]{
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 0, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {brightness, brightness, brightness, 0, 1}
            });
            attributes.SetColorMatrix(brightnessMatrix);

            // Use a new Graphics object from the new image.
            using (Graphics g = Graphics.FromImage(bDest))
            {
                // Draw the original image using the ImageAttributes created above.
                g.DrawImage(b,
                            new Rectangle(0, 0, b.Width, b.Height),
                            0, 0, b.Width, b.Height,
                            GraphicsUnit.Pixel,
                            attributes);
            }

            return bDest;
        }

        public static Bitmap ColorShift(Image b, Color cs)
        {
            Bitmap bDest = new Bitmap(b.Width, b.Height, b.PixelFormat);

            // Create the ImageAttributes object and apply the ColorMatrix
            ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
            ColorMatrix shiftMatrix = new ColorMatrix(new float[][]{
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {cs.R/255f, cs.G/255f, cs.B/255f, 0, 1}
            });
            attributes.SetColorMatrix(shiftMatrix);

            // Use a new Graphics object from the new image.
            using (Graphics g = Graphics.FromImage(bDest))
            {
                // Draw the original image using the ImageAttributes created above.
                g.DrawImage(b,
                            new Rectangle(0, 0, b.Width, b.Height),
                            0, 0, b.Width, b.Height,
                            GraphicsUnit.Pixel,
                            attributes);
            }

            return bDest;
        }

        public static Bitmap Inversion(Image b)
        {
            Bitmap bDest = new Bitmap(b.Width, b.Height, b.PixelFormat);

            // Create the ImageAttributes object and apply the ColorMatrix
            ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
            ColorMatrix inversionMatrix = new ColorMatrix(new float[][]{
                new float[] {-1, 0, 0, 0, 0},
                new float[] {0, -1, 0, 0, 0},
                new float[] {0, 0, -1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {.99f, .99f, .99f, 0, 1}
            });
            attributes.SetColorMatrix(inversionMatrix);

            // Use a new Graphics object from the new image.
            using (Graphics g = Graphics.FromImage(bDest))
            {
                // Draw the original image using the ImageAttributes created above.
                g.DrawImage(b,
                            new Rectangle(0, 0, b.Width, b.Height),
                            0, 0, b.Width, b.Height,
                            GraphicsUnit.Pixel,
                            attributes);
            }

            return bDest;
        }

        public static Bitmap Grayscale(Image b, float brightnessAdjust = 1)
        {
            Bitmap bDest = new Bitmap(b.Width, b.Height, b.PixelFormat);

            // Create the ImageAttributes object and apply the ColorMatrix
            ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
            ColorMatrix grayscaleMatrix = new ColorMatrix(new float[][]{
                new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                new float[] {     0,      0,      0, 1, 0},
                new float[] {     0,      0,      0, 0, 1}
            });
            attributes.SetColorMatrix(grayscaleMatrix);

            // Use a new Graphics object from the new image.
            using (Graphics g = Graphics.FromImage(bDest))
            {
                // Draw the original image using the ImageAttributes created above.
                g.DrawImage(b,
                            new Rectangle(0, 0, b.Width, b.Height),
                            0, 0, b.Width, b.Height,
                            GraphicsUnit.Pixel,
                            attributes);
            }

            if (brightnessAdjust != 1.0f)
                return Brightness(bDest, brightnessAdjust);

            return bDest;
        }

        public static GraphicsPath GenerateCenteredArrow(Rectangle rcArrow)
        {
            GraphicsPath gp = new GraphicsPath();
            List<Point> pts = new List<Point>();

            pts.Add(new Point(rcArrow.Left + rcArrow.Width / 3 - 1, rcArrow.Top + rcArrow.Height / 3 + 1));
            pts.Add(new Point(rcArrow.Right - rcArrow.Width / 3 + 1, rcArrow.Top + rcArrow.Height / 3 + 1));
            pts.Add(new Point(rcArrow.Right - rcArrow.Width / 2, rcArrow.Bottom - rcArrow.Height / 3 - 1));

            gp.AddLine(pts[0], pts[1]);
            gp.AddLine(pts[1], pts[2]);
            gp.AddLine(pts[2], pts[0]);

            // This will automatically add the missing lines
            gp.CloseAllFigures();
            gp.Flatten();

            return gp;
        }

        public static void AddGridToBitmap(Bitmap bmp, int hLineCount, int vLineCount, Color gridColor)
        {
            if (bmp != null)
            {
                const int MinGridResolution = 5;

                int sizeX = bmp.Width;
                int sizeY = bmp.Height;

                if (sizeX / vLineCount < MinGridResolution)
                {
                    vLineCount = sizeX / MinGridResolution;
                }
                if (sizeY / hLineCount < MinGridResolution)
                {
                    hLineCount = sizeY / MinGridResolution;
                }

                // VLines
                for (int i = 0; i < sizeX; i += (sizeX / vLineCount))
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        bmp.SetPixel(i, j, gridColor);
                    }
                }

                // HLines
                for (int j = 0; j < sizeY; j += (sizeY / hLineCount))
                {
                    for (int i = 0; i < sizeX; i++)
                    {
                        bmp.SetPixel(i, j, gridColor);
                    }
                }
            }
        }

        public static void GrayToBlack(Bitmap bmp)
        {
            for (int i = 125; i <= 128; i++)
                ImageProcessing.ReplaceColor(bmp, Color.FromArgb(i, i, i), Color.Black);
        }

        public static void ReplaceColor(Bitmap bmp, Color oldColor, Color newColor)
        {
            int sizeX = bmp.Width;
            int sizeY = bmp.Height;

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    Color c = bmp.GetPixel(i, j);
                    if (c.ToArgb() == oldColor.ToArgb())
                    {
                        bmp.SetPixel(i, j, newColor);
                    }
                }
            }
        }

        public static void DrawStringToBitmap(Bitmap bmp, int x, int y, Font fontText, Color clText, string text)
        {
            if (bmp != null)
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CompositingMode = CompositingMode.SourceCopy;

                    using (Brush b = new SolidBrush(clText))
                    {
                        g.DrawString(text, fontText, b, x, y);
                    }
                }
            }
        }
    }

}
