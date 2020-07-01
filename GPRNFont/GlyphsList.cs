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
            var key = glyph + "";

            if (listViewGlyphs.LargeImageList == null)
            {
                listViewGlyphs.LargeImageList = new ImageList();
                listViewGlyphs.LargeImageList.ImageSize = new Size(125, 125);
            }

            listViewGlyphs.LargeImageList.Images.Add(key, image);
            listViewGlyphs.Items.Add(key, key, key);
        }

        public GlyphData GetSelectedGlyphData()
        {
            var selectedItems = this.listViewGlyphs.SelectedItems;
            if (selectedItems.Count == 1)
            {
                var selectedText = selectedItems[0].Text;
                if (selectedText.Length == 1)
                    return this.glyphs[selectedText[0]];
            }

            return null;
        }

        public void EditSelectedGlyph(GlyphData newData, Image newImg)
        {
            var selectedGlyphData = GetSelectedGlyphData();
            
            selectedGlyphData.SetGlyphRect(newData.GetGlyphRect(), 100);

            var key = selectedGlyphData.Glyph + "";
            listViewGlyphs.LargeImageList.Images.RemoveByKey(key);
            listViewGlyphs.LargeImageList.Images.Add(key, newImg);

            this.listViewGlyphs.Invalidate();
        }

        public void DeleteGlyph(char glyph)
        {
            this.glyphs.Remove(glyph);
            this.listViewGlyphs.LargeImageList.Images.RemoveByKey(glyph + "");
            this.listViewGlyphs.Items.RemoveByKey(glyph + "");
        }

        public void SaveGlyph(GlyphData glyphData, Image image)
        {
            var glyph = glyphData.Glyph;

            if (glyphs.ContainsKey(glyph))
                throw new Exception("Glyph '" + glyph + "' already exists in glyphs list.");
            else
            {
                this.AddGlyphToListView(glyph, image);
                this.glyphs.Add(glyph, glyphData);
            }
        }

        public GlyphData GetGlyphData(char glyph)
        {
            if (this.glyphs.ContainsKey(glyph))
                return this.glyphs[glyph];
            
            return null;
        }

        public void Clear()
        {
            this.listViewGlyphs.Clear();

            if (this.listViewGlyphs.LargeImageList != null)
                this.listViewGlyphs.LargeImageList.Images.Clear();

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
            var img = listViewGlyphs.LargeImageList.Images[e.Item.Text];
            e.Graphics.DrawImage(img, imgRect, new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
        }        
    }
}