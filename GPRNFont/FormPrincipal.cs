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
        private bool isDividing;
        private FormQuickDivide formQuickDivide;

        private double ZoomFactor { get { return this.zoom / 100.0; } }

        public FormPrincipal()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.selectionManager = new SelectionManager();
            this.glyphsList = new GlyphsList(listViewGlyphs);
            this.xmlSerializer = new XmlSerializer(typeof(Project));
        }

        private Image DrawImageCopy(Rectangle area, PictureBox pictureBox)
        {
            var image = new Bitmap(pictureBox.Width, pictureBox.Height);

            using (var g = Graphics.FromImage(image))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(this.originalImage, new Rectangle(0, 0, pictureBox.Width, pictureBox.Height), area, GraphicsUnit.Pixel);
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
                buttonCopyGlyph.Enabled = false;
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
                }
                else
                {
                    this.ChangeTextBoxGlyph(this.currentGlyph.Glyph + "");
                    buttonSaveGlyph.Enabled = true;
                }
                buttonCopyGlyph.Enabled = this.glyphsList.GetSelectedGlyphData() != null;
                buttonDeleteGlyph.Enabled = this.glyphsList.GetSelectedGlyphData() != null;

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
                pictureBoxPedacoImg.Image = this.DrawImageCopy(selectedArea, pictureBoxPedacoImg);
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
            toolStripTextBoxZoom.Text = this.zoom + "%";

            this.DrawMainImage();

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

        private void SaveGlyph()
        {
            var glyphData = this.currentGlyph;
            var glyphImg = pictureBoxPedacoImg.Image;
            var selectedGlyphData = this.glyphsList.GetSelectedGlyphData();

            var result = DialogResult.Yes;

            if (selectedGlyphData != null) //editing existing glyph data
            {
                if (selectedGlyphData.Glyph != glyphData.Glyph) //changing glyph data character
                    result = MessageBox.Show("Change glyph from '" + selectedGlyphData.Glyph + "' to '" + glyphData.Glyph + "'?", "GPRNFont", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                else
                {
                    this.glyphsList.EditSelectedGlyph(glyphData, glyphImg);
                    result = DialogResult.No;
                }
            }

            if (result == DialogResult.Yes)
            {
                if (this.glyphsList.GetGlyphData(glyphData.Glyph) != null) //glyph is repeated
                {
                    result = MessageBox.Show("Glyph '" + glyphData.Glyph + "' is already used. Do you want to override it?", "GPRNFont", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        this.glyphsList.DeleteGlyph(glyphData.Glyph);
                }

                if (result == DialogResult.Yes)
                {
                    if (selectedGlyphData != null)
                        this.glyphsList.DeleteGlyph(selectedGlyphData.Glyph);

                    this.glyphsList.SaveGlyph(glyphData, glyphImg);
                    this.ClearSelection();
                }
            }
        }

        private void DrawMainImage()
        {
            pictureBoxImagem.Image = null;
            pictureBoxImagem.Width = Convert.ToInt32(this.originalImage.Width * this.ZoomFactor);
            pictureBoxImagem.Height = Convert.ToInt32(this.originalImage.Height * this.ZoomFactor);
            pictureBoxImagem.Image = this.DrawImageCopy(new Rectangle(new Point(0, 0), this.originalImage.Size), pictureBoxImagem);
        }

        private void OpenProject(string imgPath, List<GlyphData> glyphs)
        {
            this.imgPath = imgPath;

            this.ClearSelection();
            this.glyphsList.Clear();

            this.zoom = 100;
            toolStripTextBoxZoom.Text = "100%";

            this.originalImage = new Bitmap(this.imgPath);
            this.DrawMainImage();

            toolStripButtonZoomMinus.Enabled = true;
            toolStripButtonZoomPlus.Enabled = true;

            if (glyphs != null)
                foreach (var glyphData in glyphs)
                {
                    var img = this.DrawImageCopy(glyphData.GetGlyphRect(this.zoom), pictureBoxPedacoImg);
                    this.glyphsList.SaveGlyph(glyphData, img);
                }

            this.toolStripButtonSaveProject.Enabled = true;
            this.toolStripButtonExport.Enabled = true;
            this.toolStripButtonQuickDivide.Enabled = true;
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

        private void CopyGlyph()
        {
            var glyphCode = 33;
            while (this.glyphsList.GetGlyphData((char)glyphCode) != null)
                glyphCode++;
            
            this.currentGlyph.Glyph = (char)glyphCode;

            this.glyphsList.SaveGlyph(this.currentGlyph, pictureBoxPedacoImg.Image);
            this.ClearSelection();
        }

        private void DeleteGlyph()
        {
            var selectedGlyphData = this.glyphsList.GetSelectedGlyphData();

            var result = MessageBox.Show("Do you want to delete the glyph '" + selectedGlyphData.Glyph + "'?", "GPRNFont", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                this.glyphsList.DeleteGlyph(selectedGlyphData.Glyph);
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
                    catch (InvalidOperationException)
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

        private void ForEachDividedGlyph(Action<int, int> function)
        {
            var a = this.formQuickDivide.DivisionArea;

            for (var y = a.Y; y + this.formQuickDivide.GlyphsHeight <= a.Bottom; y = y + this.formQuickDivide.GlyphsHeight)
            {
                for (var x = a.X; x + this.formQuickDivide.GlyphsWidth <= a.Right; x = x + this.formQuickDivide.GlyphsWidth)
                {
                    function.Invoke(x, y);
                }
            }
        }

        private void CreateDividedGlyph(ref int glyphCode, int x, int y)
        {
            var glyphArea = new Rectangle(x, y, this.formQuickDivide.GlyphsWidth, this.formQuickDivide.GlyphsHeight);
            var img = this.DrawImageCopy(glyphArea, pictureBoxPedacoImg);

            var glyphData = new GlyphData();

            if (this.formQuickDivide.CharEnumerator.MoveNext())
                glyphData.Glyph = this.formQuickDivide.CharEnumerator.Current;

            if (this.glyphsList.GetGlyphData(glyphData.Glyph) != null)
                glyphData.Glyph = '\0';

            if (glyphData.Glyph == '\0')
            {
                do
                {
                    glyphCode++;
                    glyphData.Glyph = (char)glyphCode;
                }
                while (this.glyphsList.GetGlyphData(glyphData.Glyph) != null);
            }

            glyphData.SetGlyphRect(glyphArea, 100);

            this.glyphsList.SaveGlyph(glyphData, img);
        }

        public void QuickDivide()
        {
            this.isDividing = true;
            
            try
            {
                Rectangle divisionArea;
                if (this.currentGlyph == null)
                    divisionArea = new Rectangle(new Point(0, 0), this.originalImage.Size);
                else
                    divisionArea = this.currentGlyph.GetGlyphRect();

                this.formQuickDivide = new FormQuickDivide(pictureBoxImagem, divisionArea);
                if (this.formQuickDivide.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    this.ClearSelection();

                    if (this.formQuickDivide.OverrideExistent)
                        this.glyphsList.Clear();

                    var glyphCode = 32;

                    this.ForEachDividedGlyph((x, y) => CreateDividedGlyph(ref glyphCode, x, y));
                }
            }
            finally
            {
                this.isDividing = false;
                this.formQuickDivide.Dispose();
                this.formQuickDivide = null;
                Cursor.Current = Cursors.Default;
                pictureBoxImagem.Invalidate();
            }
        }

        private void DrawGlyphDivision(Graphics g, int x, int y)
        {
            var glyphArea = new Rectangle(x, y, this.formQuickDivide.GlyphsWidth, this.formQuickDivide.GlyphsHeight);
            glyphArea.X = Convert.ToInt32(glyphArea.X * this.ZoomFactor);
            glyphArea.Y = Convert.ToInt32(glyphArea.Y * this.ZoomFactor);
            glyphArea.Width = Convert.ToInt32(glyphArea.Width * this.ZoomFactor);
            glyphArea.Height = Convert.ToInt32(glyphArea.Height * this.ZoomFactor);

            using (var pen = new Pen(Color.Lime, 1F))
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(pen, glyphArea);
            }
            using (var brush = new SolidBrush(Color.FromArgb(128, Color.Lime)))
            {
                g.FillRectangle(brush, glyphArea);
            }
        }

        private void DrawDivisionArea(Graphics g)
        {
            var divisionAreaZ = new Rectangle(
                Convert.ToInt32(this.formQuickDivide.DivisionArea.X * this.ZoomFactor),
                Convert.ToInt32(this.formQuickDivide.DivisionArea.Y * this.ZoomFactor),
                Convert.ToInt32(this.formQuickDivide.DivisionArea.Width * this.ZoomFactor),
                Convert.ToInt32(this.formQuickDivide.DivisionArea.Height * this.ZoomFactor));

            using (var pen = new Pen(Color.Red, 1F))
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(pen, divisionAreaZ);
            }
            using (var brush = new SolidBrush(Color.FromArgb(128, Color.Red)))
            {
                g.FillRectangle(brush, divisionAreaZ);
            }
        }

        private void DrawGlyphsDivision(Graphics g)
        {
            this.DrawDivisionArea(g);
            this.ForEachDividedGlyph((x, y) => DrawGlyphDivision(g, x, y));
        }

        private void pictureBoxImagem_MouseDown(object sender, MouseEventArgs e)
        {
            this.selectionManager.StartSelection(pictureBoxImagem.PointToClient(MousePosition), Convert.ToInt32(this.ZoomFactor));
            this.UpdateCurrentGlyph();
        }

        private void pictureBoxImagem_MouseUp(object sender, MouseEventArgs e)
        {
            this.selectionManager.EndSelection(pictureBoxImagem.PointToClient(MousePosition), pictureBoxImagem.Size, Convert.ToInt32(this.ZoomFactor));
            this.UpdateCurrentGlyph();
        }

        private void pictureBoxImagem_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePoint = pictureBoxImagem.PointToClient(MousePosition);

            this.selectionManager.UpdateSelection(mousePoint, pictureBoxImagem.Size, Convert.ToInt32(this.ZoomFactor));

            Cursor.Current = this.selectionManager.GetCursor(mousePoint);

            pictureBoxImagem.Invalidate();
        }

        private void pictureBoxImagem_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            if (this.isDividing)
                this.DrawGlyphsDivision(g);
            else
                this.selectionManager.DrawSelectionRect(g);
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
            this.SaveGlyph();
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

        private void buttonCopyGlyph_Click(object sender, EventArgs e)
        {
            this.CopyGlyph();
        }

        private void buttonDeleteGlyph_Click(object sender, EventArgs e)
        {
            this.DeleteGlyph();
        }

        private void toolStripButtonNewProject_Click(object sender, EventArgs e)
        {
            this.NewProject();
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

        private void toolStripButtonQuickDivide_Click(object sender, EventArgs e)
        {
            this.QuickDivide();
        }
    }
}