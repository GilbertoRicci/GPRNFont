using System;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace GPRNFont
{
    public class GlyphData
    {
        private decimal imgWidth;
        private decimal imgHeight;
        private char glyph;
        private int x;
        private int y;
        private int w;
        private int h;

        public char Glyph
        {
            get
            {
                return this.glyph;
            }
            set
            {
                this.glyph = value;
                this.Index = (int)this.glyph;
            }
        }
        public int XPosition
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
                this.CalcUnityData();
            }
        }
        public int YPosition
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
                this.CalcUnityData();
            }
        }
        public int Width
        {
            get
            {
                return this.w;
            }
            set
            {
                this.w = value;
                this.CalcUnityData();
            }
        }
        public int Height
        {
            get
            {
                return this.h;
            }
            set
            {
                this.h = value;
                this.CalcUnityData();
            }
        }
        public int Index { get; set; }
        public int Advance { get; set; }
        public decimal UVX { get; set; }
        public decimal UVY { get; set; }
        public decimal UVW { get; set; }
        public decimal UVH { get; set; }
        public int VertX { get; set; }
        public int VertY { get; set; }
        public int VertW { get; set; }
        public int VertH { get; set; }

        public GlyphData(Rectangle selectedArea, Size imageSize, int zoom)
        {
            this.Glyph = '\0';
            this.Index = 0;

            this.imgWidth = imageSize.Width;
            this.imgHeight = imageSize.Height;

            this.SetGlyphRect(selectedArea, zoom);
        }

        private void CalcUnityData()
        {
            this.Advance = this.Width;
            this.UVX = 1 / (this.imgWidth / this.Width) * this.XPosition / this.Width;
            this.UVY = 1 - 1 / (this.imgHeight / this.Height) * ((this.YPosition / this.Height) + 1);
            this.UVW = 1 / (this.imgWidth / this.Width);
            this.UVH = 1 / (this.imgHeight / this.Height);
            this.VertX = 0;
            this.VertY = 0;
            this.VertW = this.Width;
            this.VertH = -this.Width;
        }

        public void SetGlyphRect(Rectangle rect, int zoom)
        {
            double zoomFactor = 100 / (double)zoom;

            this.x = Convert.ToInt32(rect.X * zoomFactor);
            this.y = Convert.ToInt32(rect.Y * zoomFactor);
            this.w = Convert.ToInt32(rect.Width * zoomFactor);
            this.h = Convert.ToInt32(rect.Height * zoomFactor);

            this.CalcUnityData();
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