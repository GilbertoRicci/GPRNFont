using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GPRNFont
{
    public partial class FormPrincipal : Form
    {
        private XmlSerializer xmlSerializer;
        private string imgPath;
        private SelectionManager selectionManager;
        private Image originalImage;
        private GlyphData currentGlyph;
        private GlyphsList glyphsList;
        private bool fillingRectData = false;
        private bool changingTextBoxGlyph = false;
        private int zoom = 100;

        public FormPrincipal()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.selectionManager = new SelectionManager();
            this.glyphsList = new GlyphsList(listViewGlyphs);
            this.xmlSerializer = new XmlSerializer(typeof(Project));
        }

        private Image DrawImageCopy(Rectangle area)
        {
            var image = new Bitmap(pictureBoxPedacoImg.Width, pictureBoxPedacoImg.Height);

            using (var g = Graphics.FromImage(image))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(this.originalImage, new Rectangle(0, 0, pictureBoxPedacoImg.Width, pictureBoxPedacoImg.Height), area, GraphicsUnit.Pixel);
            }

            return image;
        }

        private void ChangeTextBoxGlyph(string text)
        {
            this.changingTextBoxGlyph = true;
            textBoxGlyph.Text = text;
            this.changingTextBoxGlyph = false;
        }

        private void SetTextBoxValues()
        {
            if (this.currentGlyph == null)
            {
                textBoxGlyph.Enabled = false;
                this.ChangeTextBoxGlyph("");
                buttonSaveGlyph.Enabled = false;
                buttonDeleteGlyph.Enabled = false;

                numericUpDownPosX.Value = 0;
                numericUpDownPosY.Value = 0;
                numericUpDownWidth.Value = 1;
                numericUpDownHeight.Value = 1;
                numericUpDownAdvance.Value = 0;
                numericUpDownUVX.Value = 0;
                numericUpDownUVY.Value = 0;
                numericUpDownUVW.Value = 0;
                numericUpDownUVH.Value = 0;
                numericUpDownVertX.Value = 0;
                numericUpDownVertY.Value = 0;
                numericUpDownVertW.Value = 0;
                numericUpDownVertH.Value = 0;
            }
            else
            {
                textBoxGlyph.Enabled = true;
                if (this.currentGlyph.Glyph == '\0')
                {
                    this.ChangeTextBoxGlyph("");
                    buttonSaveGlyph.Enabled = false;
                    buttonDeleteGlyph.Enabled = false;
                }
                else
                {
                    this.ChangeTextBoxGlyph(this.currentGlyph.Glyph + "");
                    buttonSaveGlyph.Enabled = true;
                    buttonDeleteGlyph.Enabled = this.glyphsList.GetGlyphData(this.currentGlyph.Glyph) != null;
                }

                numericUpDownPosX.Value = this.currentGlyph.XPosition;
                numericUpDownPosY.Value = this.currentGlyph.YPosition;
                numericUpDownWidth.Value = this.currentGlyph.Width;
                numericUpDownHeight.Value = this.currentGlyph.Height;

                numericUpDownIndex.Value = this.currentGlyph.GetIndex();
                numericUpDownAdvance.Value = this.currentGlyph.GetAdvance();
                numericUpDownUVX.Value = this.currentGlyph.GetUVX(this.originalImage.Width);
                numericUpDownUVY.Value = this.currentGlyph.GetUVY(this.originalImage.Height);
                numericUpDownUVW.Value = this.currentGlyph.GetUVW(this.originalImage.Width);
                numericUpDownUVH.Value = this.currentGlyph.GetUVH(this.originalImage.Height);
                numericUpDownVertX.Value = this.currentGlyph.GetVertX();
                numericUpDownVertY.Value = this.currentGlyph.GetVertY();
                numericUpDownVertW.Value = this.currentGlyph.GetVertW();
                numericUpDownVertH.Value = this.currentGlyph.GetVertH();
            }
        }

        private void EnableTextBoxes(bool enable)
        {
            textBoxGlyph.Enabled = enable;
            numericUpDownPosX.Enabled = enable;
            numericUpDownPosY.Enabled = enable;
            numericUpDownWidth.Enabled = enable;
            numericUpDownHeight.Enabled = enable;
        }

        private void UpdateCurrentGlyphImage()
        {
            if (this.currentGlyph == null)
                pictureBoxPedacoImg.Image = null;
            else
            {
                var selectedArea = this.currentGlyph.GetGlyphRect();
                pictureBoxPedacoImg.Image = this.DrawImageCopy(selectedArea);
            }
        }

        private void ShowGlyphData()
        {
            this.fillingRectData = true;
            this.UpdateCurrentGlyphImage();
            this.SetTextBoxValues();
            this.EnableTextBoxes(this.currentGlyph != null);
            
            this.fillingRectData = false;
        }

        private void UpdateCurrentGlyph()
        {
            if (this.selectionManager.SelectionRectHasArea())
            {
                var selectedArea = this.selectionManager.SelectionRect;

                if (this.currentGlyph == null)
                    this.currentGlyph = new GlyphData();

                this.currentGlyph.SetGlyphRect(selectedArea, this.zoom);
            }
            else
            {
                this.currentGlyph = null;
                listViewGlyphs.SelectedItems.Clear();
            }
                
            this.ShowGlyphData();

            pictureBoxImagem.Invalidate();
        }

        private void ClearSelection()
        {
            this.selectionManager.ResetSelection();
            this.currentGlyph = null;
            this.ShowGlyphData();
            pictureBoxImagem.Invalidate();
        }

        private void ChangeSelectionRect()
        {
            this.selectionManager.SelectionRect = this.currentGlyph.GetGlyphRect(this.zoom);
            this.ShowGlyphData();
            pictureBoxImagem.Invalidate();
        }

        private void ZoomApply()
        {
            this.toolStripTextBoxZoom.Text = this.zoom + "%";

            this.pictureBoxImagem.Image = new Bitmap(this.originalImage,
                                                        Convert.ToInt32(originalImage.Width * this.zoom / 100),
                                                        Convert.ToInt32(originalImage.Height * this.zoom / 100));

            if (this.currentGlyph != null)
                this.selectionManager.SelectionRect = this.currentGlyph.GetGlyphRect(this.zoom);

            pictureBoxImagem.Invalidate();
        }

        private void SelectGlyph(char glyph)
        {
            var glyphData = this.glyphsList.GetGlyphData(glyph);

            if (glyphData == null)
                MessageBox.Show("Glyph '" + glyph + "' do not exist in glyph list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                this.currentGlyph = new GlyphData
                {
                    Glyph = glyph,
                    XPosition = glyphData.XPosition,
                    YPosition = glyphData.YPosition,
                    Width = glyphData.Width,
                    Height = glyphData.Height
                };

                this.selectionManager.SelectionRect = glyphData.GetGlyphRect(this.zoom);

                this.ShowGlyphData();
                pictureBoxImagem.Invalidate();
            }
        }

        private void OpenProject(string imgPath, List<GlyphData> glyphs)
        {
            this.imgPath = imgPath;

            this.ClearSelection();
            this.glyphsList.Clear();

            this.zoom = 100;
            toolStripTextBoxZoom.Text = "100%";

            this.originalImage = new Bitmap(this.imgPath);
            pictureBoxImagem.Image = originalImage;

            toolStripButtonZoomMinus.Enabled = true;
            toolStripButtonZoomPlus.Enabled = true;

            if (glyphs != null)
                foreach (var glyphData in glyphs)
                {
                    var img = this.DrawImageCopy(glyphData.GetGlyphRect(this.zoom));
                    this.glyphsList.SaveGlyph(glyphData, img);
                }
        }

        private void NewProject()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "New Project";
                dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

                if (dlg.ShowDialog() == DialogResult.OK)
                    this.OpenProject(dlg.FileName, null);
            }
        }

        private void DeleteGlyph()
        {
            var result = MessageBox.Show("Do you want to delete the glyph '" + this.currentGlyph.Glyph + "'?", "GPRNFont", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                this.glyphsList.DeleteGlyph(this.currentGlyph.Glyph);
        }

        private void SaveProject()
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Save Project";
            dialog.Filter = "GPRNFont Project (*.gfpj) | *.gfpj";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var project = new Project
                {
                    ImagePath = this.imgPath,
                    GlyphsData = this.glyphsList.GetGlyphsData()
                };

                using (var writer = new StreamWriter(dialog.FileName))
                {
                    xmlSerializer.Serialize(writer, project);
                }
            }
        }

        private void SelectProject()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Open Project";
            dialog.Filter = "GPRNFont Project (*.gfpj) | *.gfpj";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Project project = null;

                using (var reader = new StreamReader(dialog.FileName))
                {
                    try
                    {
                        project = (Project)xmlSerializer.Deserialize(reader);
                    }
                    catch (InvalidOperationException e)
                    {
                        MessageBox.Show("Invalid project file.", "GPRNFont", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                this.OpenProject(project.ImagePath, project.GlyphsData);
            }
        }

        private string GetFontsettingsText(string fileName)
        {
            var fontsettingsText = "";

            using (var reader = new StreamReader(fileName))
            {
                var isBetweenCharRectsAndKerningValues = false;
                var line = reader.ReadLine();

                while (line != null && !isBetweenCharRectsAndKerningValues)
                {
                    if (line.Contains("m_LineSpacing"))
                        fontsettingsText += "  m_LineSpacing: " + this.glyphsList.GetLineSpacing() + Environment.NewLine;
                    else
                        fontsettingsText += line + Environment.NewLine;

                    if (line.Contains("m_CharacterRects"))
                        isBetweenCharRectsAndKerningValues = true;

                    line = reader.ReadLine();
                }

                if (isBetweenCharRectsAndKerningValues)
                    foreach (var glyphData in glyphsList.GetGlyphsData())
                        fontsettingsText += glyphData.GetExportText(this.originalImage.Width, this.originalImage.Height);

                while (line != null)
                {
                    if (line.Contains("m_KerningValues"))
                        isBetweenCharRectsAndKerningValues = false;

                    if (!isBetweenCharRectsAndKerningValues)
                        fontsettingsText += line + Environment.NewLine;

                    line = reader.ReadLine();
                }
            }

            return fontsettingsText;
        }
        private void Export()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Export";
            dialog.Filter = ".fontsettings file (*.fontsettings) | *.fontsettings";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(dialog.FileName, this.GetFontsettingsText(dialog.FileName));
                    MessageBox.Show("Content successfully exported to file " + dialog.FileName + ".", "GPRNFont", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch(Exception e)
                {
                    MessageBox.Show("Export to file " + dialog.FileName + " failed (" + e.Message + ").", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.NewProject();
        }

        private void pictureBoxImagem_MouseDown(object sender, MouseEventArgs e)
        {
            this.selectionManager.StartSelection(pictureBoxImagem.PointToClient(MousePosition), this.zoom / 100);
            this.UpdateCurrentGlyph();
        }

        private void pictureBoxImagem_MouseUp(object sender, MouseEventArgs e)
        {
            this.selectionManager.EndSelection(pictureBoxImagem.PointToClient(MousePosition), pictureBoxImagem.Size, this.zoom / 100);
            this.UpdateCurrentGlyph();
        }

        private void pictureBoxImagem_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePoint = pictureBoxImagem.PointToClient(MousePosition);

            this.selectionManager.UpdateSelection(mousePoint, pictureBoxImagem.Size, this.zoom/100);

            Cursor.Current = this.selectionManager.GetCursor(mousePoint);

            pictureBoxImagem.Invalidate();
        }

        private void pictureBoxImagem_Paint(object sender, PaintEventArgs e)
        {
            this.selectionManager.DrawSelectionRect(e.Graphics);
        }

        private void textBoxGlyph_TextChanged(object sender, EventArgs e)
        {
            if (!this.changingTextBoxGlyph)
            {
                var glyph = textBoxGlyph.Text.ToCharArray();

                if (glyph.Length == 0)
                    this.currentGlyph.Glyph = '\0';
                else if (glyph.Length == 1)
                    this.currentGlyph.Glyph = glyph[0];

                this.SetTextBoxValues();
            }
        }

        private void numericUpDownPosX_ValueChanged(object sender, EventArgs e)
        {
            if (!this.fillingRectData)
            {
                if (numericUpDownPosX.Value + numericUpDownWidth.Value > originalImage.Size.Width)
                {
                    this.fillingRectData = true;
                    numericUpDownPosX.Value = originalImage.Size.Width - numericUpDownWidth.Value;
                    this.fillingRectData = false;
                }
                this.currentGlyph.XPosition = Convert.ToInt32(numericUpDownPosX.Value);

                this.ChangeSelectionRect();
            }
        }

        private void numericUpDownPosY_ValueChanged(object sender, EventArgs e)
        {
            if (!this.fillingRectData)
            {
                if (numericUpDownPosY.Value + numericUpDownHeight.Value > originalImage.Size.Height)
                {
                    this.fillingRectData = true;
                    numericUpDownPosY.Value = originalImage.Size.Height - numericUpDownHeight.Value;
                    this.fillingRectData = false;
                }
                this.currentGlyph.YPosition = Convert.ToInt32(numericUpDownPosY.Value);

                this.ChangeSelectionRect();
            }
        }

        private void numericUpDownWidth_ValueChanged(object sender, EventArgs e)
        {
            if (!this.fillingRectData)
            {
                if (numericUpDownPosX.Value + numericUpDownWidth.Value > originalImage.Size.Width)
                {
                    this.fillingRectData = true;
                    numericUpDownWidth.Value = originalImage.Size.Width - numericUpDownPosX.Value;
                    this.fillingRectData = false;
                }
                this.currentGlyph.Width = Convert.ToInt32(numericUpDownWidth.Value);

                this.ChangeSelectionRect();
            }
        }

        private void numericUpDownHeight_ValueChanged(object sender, EventArgs e)
        {
            if (!this.fillingRectData)
            {
                if (numericUpDownPosY.Value + numericUpDownHeight.Value > originalImage.Size.Height)
                {
                    this.fillingRectData = true;
                    numericUpDownHeight.Value = originalImage.Size.Height - numericUpDownPosY.Value;
                    this.fillingRectData = false;
                }
                this.currentGlyph.Height = Convert.ToInt32(numericUpDownHeight.Value);

                this.ChangeSelectionRect();
            }
        }

        private void toolStripButtonZoomPlus_Click(object sender, EventArgs e)
        {
            this.zoom = this.zoom * 2;

            if (this.zoom >= 800)
                toolStripButtonZoomPlus.Enabled = false;
            toolStripButtonZoomMinus.Enabled = true;

            this.ZoomApply();
        }

        private void toolStripButtonZoomLess_Click(object sender, EventArgs e)
        {
            this.zoom = this.zoom / 2;

            if (this.zoom <= 25)
                toolStripButtonZoomMinus.Enabled = false;
            toolStripButtonZoomPlus.Enabled = true;

            this.ZoomApply();
        }

        private void buttonSaveGlyph_Click(object sender, EventArgs e)
        {
            if(this.glyphsList.SaveGlyph(this.currentGlyph, pictureBoxPedacoImg.Image))
                this.ClearSelection();
        }

        private void listViewGlyphs_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            this.glyphsList.Draw(e);
        }

        private void listViewGlyphs_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                var glyph = e.Item.Text[0];
                this.SelectGlyph(glyph);
            }
            else
                this.ClearSelection();
        }

        private void buttonDeleteGlyph_Click(object sender, EventArgs e)
        {
            this.DeleteGlyph();
        }

        private void toolStripButtonSaveProject_Click(object sender, EventArgs e)
        {
            this.SaveProject();
        }

        private void toolStripButtonOpenProject_Click(object sender, EventArgs e)
        {
            this.SelectProject();
        }

        private void toolStripButtonExport_Click(object sender, EventArgs e)
        {
            this.Export();
        }
    }
}