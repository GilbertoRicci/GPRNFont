using System;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace GPRNFont
{
    public class GlyphData
    {
        private decimal imgWidth;
        private decimal imgHeight;
        private GlyphBasicData glyphBasicData;

        public GlyphBasicData BasicData
        {
            get
            {
                return this.glyphBasicData;
            }
        }

        public char Glyph
        {
            get
            {
                return this.glyphBasicData.Glyph;
            }
            set
            {
                this.glyphBasicData.Glyph = value;
            }
        }

        public int XPosition
        {
            get
            {
                return this.glyphBasicData.XPosition;
            }
            set
            {
                this.glyphBasicData.XPosition = value;
            }
        }

        public int YPosition
        {
            get
            {
                return this.glyphBasicData.YPosition;
            }
            set
            {
                this.glyphBasicData.YPosition = value;
            }
        }

        public int Width
        {
            get
            {
                return this.glyphBasicData.Width;
            }
            set
            {
                this.glyphBasicData.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return this.glyphBasicData.Height;
            }
            set
            {
                this.glyphBasicData.Height = value;
            }
        }

        public int Index
        {
            get
            {
                return (int)this.Glyph;
            }
        }

        public int Advance
        {
            get
            {
                return this.Width;
            }
        }

        public decimal UVX
        {
            get
            {
                return 1 / (this.imgWidth / this.Width) * this.XPosition / this.Width;
            }
        }

        public decimal UVY
        {
            get
            {
                return 1 - 1 / (this.imgHeight / this.Height) * ((this.YPosition / this.Height) + 1);
            }
        }

        public decimal UVW
        {
            get
            {
                return 1 / (this.imgWidth / this.Width);
            }
        }

        public decimal UVH
        {
            get
            {
                return 1 / (this.imgHeight / this.Height);
            }
        }

        public int VertX
        {
            get
            {
                return 0;
            }
        }

        public int VertY
        {
            get
            {
                return 0;
            }
        }

        public int VertW
        {
            get
            {
                return this.Width;
            }
        }

        public int VertH
        {
            get
            {
                return -this.Width;
            }
        }

        public GlyphData(Rectangle selectedArea, Size imageSize, int zoom)
        {
            this.glyphBasicData = new GlyphBasicData();

            this.Glyph = '\0';

            this.imgWidth = imageSize.Width;
            this.imgHeight = imageSize.Height;

            this.SetGlyphRect(selectedArea, zoom);
        }

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