using System;
using System.Drawing;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace GPRNFont
{
    public class GlyphData
    {
        public char Glyph { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void SetGlyphRect(Rectangle rect, int zoom)
        {
            double zoomFactor = 100 / (double)zoom;

            this.XPosition = Convert.ToInt32(rect.X * zoomFactor);
            this.YPosition = Convert.ToInt32(rect.Y * zoomFactor);
            this.Width = Convert.ToInt32(rect.Width * zoomFactor);
            this.Height = Convert.ToInt32(rect.Height * zoomFactor);
        }

        public Rectangle GetGlyphRect(int zoom)
        {
            double zoomFactor = (double)zoom / 100;
            return new Rectangle(Convert.ToInt32(this.XPosition * zoomFactor), Convert.ToInt32(this.YPosition * zoomFactor),
                                    Convert.ToInt32(this.Width * zoomFactor), Convert.ToInt32(this.Height * zoomFactor));
        }

        public Rectangle GetGlyphRect()
        {
            return this.GetGlyphRect(100);
        }

        public int GetIndex()
        {
            return (int)this.Glyph;
        }

        public int GetAdvance()
        {
            return this.Width;
        }

        public decimal GetUVX(int imgWidth)
        {
            return 1M / ((decimal)imgWidth / (decimal)this.Width) * (decimal)this.XPosition / (decimal)this.Width;
        }

        public decimal GetUVY(int imgHeight)
        {
            return 1M - 1M / ((decimal)imgHeight / (decimal)this.Height) * (((decimal)this.YPosition / (decimal)this.Height) + 1M);
        }

        public decimal GetUVW(int imgWidth)
        {
            return 1M / ((decimal)imgWidth / (decimal)this.Width);
        }

        public decimal GetUVH(int imgHeight)
        {
            return 1M / ((decimal)imgHeight / (decimal)this.Height);
        }

        public int GetVertX()
        {
            return 0;
        }

        public int GetVertY()
        {
            return 0;
        }

        public int GetVertW()
        {
            return this.Width;
        }

        public int GetVertH()
        {
            return -this.Height;
        }

        public string GetExportText(int imgWidth, int imgHeight)
        {
            return  "  - serializedVersion: 2" + Environment.NewLine +
                    "    index: " + this.GetIndex() + Environment.NewLine +
                    "    uv:" + Environment.NewLine +
                    "      serializedVersion: 2" + Environment.NewLine +
                    "      x: " + this.GetUVX(imgWidth).ToString("G8", CultureInfo.CreateSpecificCulture("en-GB")) + Environment.NewLine +
                    "      y: " + this.GetUVY(imgHeight).ToString("G8", CultureInfo.CreateSpecificCulture("en-GB")) + Environment.NewLine +
                    "      width: " + this.GetUVW(imgWidth).ToString("G8", CultureInfo.CreateSpecificCulture("en-GB")) + Environment.NewLine +
                    "      height: " + this.GetUVH(imgHeight).ToString("G8", CultureInfo.CreateSpecificCulture("en-GB")) + Environment.NewLine +
                    "    vert:" + Environment.NewLine +
                    "      serializedVersion: 2" + Environment.NewLine +
                    "      x: " + this.GetVertX() + Environment.NewLine +
                    "      y: " + this.GetVertY() + Environment.NewLine +
                    "      width: " + this.GetVertW() + Environment.NewLine +
                    "      height: " + this.GetVertH() + Environment.NewLine +
                    "    advance: " + this.GetAdvance() + Environment.NewLine +
                    "    flipped: 0" + Environment.NewLine;
        }
    }
}