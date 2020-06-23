using System;
using System.Drawing;

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
    }
}