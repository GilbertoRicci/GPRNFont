using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPRNFont
{
    class GlyphsList
    {
        private Dictionary<char, GlyphData> glyphs;
        private ListView listViewGlyphs;

        public GlyphsList(ListView listView)
        {
            this.glyphs = new Dictionary<char, GlyphData>();
            this.listViewGlyphs = listView;
        }

        private void AddGlyphToListView(char glyph, Image image)
        {
            if (listViewGlyphs.LargeImageList == null)
            {
                listViewGlyphs.LargeImageList = new ImageList();
                listViewGlyphs.LargeImageList.ImageSize = new Size(125, 125);
            }

            listViewGlyphs.LargeImageList.Images.Add(image);

            var key = glyph + "";
            listViewGlyphs.Items.Add(key, key, listViewGlyphs.LargeImageList.Images.Count - 1);
        }

        public void DeleteGlyph(char glyph)
        {
            this.glyphs.Remove(glyph);
            this.listViewGlyphs.Items.RemoveByKey(glyph + "");
        }

        public bool SaveGlyph(GlyphData glyphData, Image image)
        {
            var glyph = glyphData.Glyph;
            DialogResult result = DialogResult.Yes;

            if (glyphs.ContainsKey(glyph))
            {
                result = MessageBox.Show("Glyph '" + glyph + "' is already used. Do you want to override it?", "GPRNFont", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    this.DeleteGlyph(glyph);
            }

            if (result == DialogResult.Yes)
            {
                this.AddGlyphToListView(glyph, image);
                this.glyphs.Add(glyph, glyphData);
            }

            return result == DialogResult.Yes;
        }

        public GlyphData GetGlyphData(char glyph)
        {
            if (this.glyphs.ContainsKey(glyph))
                return this.glyphs[glyph];
            
            return null;
        }

        public void Draw(DrawListViewItemEventArgs e)
        {
            //item background
            if (e.Item.Selected)
            {
                if (listViewGlyphs.Focused)
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                else if (!listViewGlyphs.HideSelection)
                    e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            }
            else
            {
                using (SolidBrush br = new SolidBrush(listViewGlyphs.BackColor))
                {
                    e.Graphics.FillRectangle(br, e.Bounds);
                }
            }

            //item border
            e.Graphics.DrawRectangle(SystemPens.ControlText, e.Bounds);

            //text background
            var textRect = new Rectangle(e.Bounds.X + e.Bounds.Width / 3, 4, e.Bounds.Width / 3, 20);
            using (SolidBrush br = new SolidBrush(listViewGlyphs.BackColor))
            {
                e.Graphics.FillRectangle(br, textRect);
            }

            //text border
            e.Graphics.DrawRectangle(SystemPens.WindowText, textRect);

            //text
            TextRenderer.DrawText(e.Graphics, e.Item.Text, listViewGlyphs.Font, textRect,
                                  SystemColors.WindowText, Color.Empty,
                                  TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            //img background
            var imgBorderRect = new Rectangle(e.Bounds.X + 4, textRect.Y + textRect.Height + 2, e.Bounds.Width - 8, e.Bounds.Height - textRect.Height - 8);
            e.Graphics.FillRectangle(SystemBrushes.ControlDark, imgBorderRect);

            //img border
            e.Graphics.DrawRectangle(SystemPens.ControlText, imgBorderRect);

            //img
            var imgRect = new Rectangle(imgBorderRect.X + 1, imgBorderRect.Y + 1, imgBorderRect.Width - 2, imgBorderRect.Height - 2);
            var img = listViewGlyphs.LargeImageList.Images[e.Item.ImageIndex];
            e.Graphics.DrawImage(img, imgRect, new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
        }

        public void Clear()
        {
            this.listViewGlyphs.Clear();
            this.glyphs.Clear();
        }

        public List<GlyphData> GetGlyphsData()
        {
            return this.glyphs.Values.ToList();
        }

        public int GetLineSpacing()
        {
            return this.glyphs.Values.Max(x => x.Height);
        }
    }
}