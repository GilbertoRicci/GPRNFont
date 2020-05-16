using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPRNFont
{
    public partial class FormPrincipal : Form
    {
        private SelectionManager selectionManager;
        private bool fillingRectData;

        public FormPrincipal()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.selectionManager = new SelectionManager();
        }

        private void DrawImageCopy(Rectangle selectedArea)
        {
            pictureBoxPedacoImg.Image = new Bitmap(pictureBoxPedacoImg.Width, pictureBoxPedacoImg.Height);
            using (var g = Graphics.FromImage(pictureBoxPedacoImg.Image))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(pictureBoxImagem.Image, new Rectangle(0, 0, pictureBoxPedacoImg.Width, pictureBoxPedacoImg.Height), selectedArea, GraphicsUnit.Pixel);
            }
        }

        private void FillRectData(Rectangle selectedArea)
        {
            this.fillingRectData = true;

            if (selectedArea.IsEmpty)
            {
                numericUpDownPosX.Value = 0;
                numericUpDownPosY.Value = 0;
                numericUpDownWidth.Value = 0;
                numericUpDownHeight.Value = 0;
                textBoxGlyph.Text = "";

                numericUpDownPosX.Enabled = false;
                numericUpDownPosY.Enabled = false;
                numericUpDownWidth.Enabled = false;
                numericUpDownHeight.Enabled = false;
            }
            else
            {
                numericUpDownPosX.Value = selectedArea.X;
                numericUpDownPosY.Value = selectedArea.Y;
                numericUpDownWidth.Value = selectedArea.Width;
                numericUpDownHeight.Value = selectedArea.Height;

                numericUpDownPosX.Enabled = true;
                numericUpDownPosY.Enabled = true;
                numericUpDownWidth.Enabled = true;
                numericUpDownHeight.Enabled = true;
            }

            this.fillingRectData = false;
        }

        private void UpdateGlyphData()
        {
            var selectedArea = this.selectionManager.GetSelectedArea();

            this.DrawImageCopy(selectedArea);
            this.FillRectData(selectedArea);
            this.textBoxGlyph.Enabled = !selectedArea.IsEmpty;
        }

        private void ChangeSelectionRect()
        {
            var newSelectedArea = new Rectangle(Decimal.ToInt32(numericUpDownPosX.Value), Decimal.ToInt32(numericUpDownPosY.Value),
                                    Decimal.ToInt32(numericUpDownWidth.Value), Decimal.ToInt32(numericUpDownHeight.Value));

            this.selectionManager.ChangeSelectionRect(newSelectedArea);
            this.DrawImageCopy(newSelectedArea);
            pictureBoxImagem.Invalidate();
        }
        
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Image";
                dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.selectionManager.ResetSelection();
                    pictureBoxImagem.Image = new Bitmap(dlg.FileName);
                }
            }
        }

        private void pictureBoxImagem_MouseDown(object sender, MouseEventArgs e)
        {
            this.selectionManager.StartSelection(pictureBoxImagem.PointToClient(MousePosition));
            pictureBoxImagem.Invalidate();
        }

        private void pictureBoxImagem_MouseUp(object sender, MouseEventArgs e)
        {
            this.selectionManager.EndSelection(pictureBoxImagem.PointToClient(MousePosition), pictureBoxImagem.Size);
            pictureBoxImagem.Invalidate();

            this.UpdateGlyphData();
        }

        private void pictureBoxImagem_MouseMove(object sender, MouseEventArgs e)
        {
            this.selectionManager.UpdateSelection(pictureBoxImagem.PointToClient(MousePosition), pictureBoxImagem.Size);
            pictureBoxImagem.Invalidate();
        }

        private void pictureBoxImagem_Paint(object sender, PaintEventArgs e)
        {
            this.selectionManager.DrawSelectionRect(e.Graphics);
        }

        private void textBoxGlyph_TextChanged(object sender, EventArgs e)
        {
            buttonSaveGlyph.Enabled = textBoxGlyph.Text != "";
        }

        private void numericUpDownPosX_ValueChanged(object sender, EventArgs e)
        {
            if (!this.fillingRectData)
            {
                if (numericUpDownPosX.Value + numericUpDownWidth.Value > pictureBoxImagem.Size.Width)
                    numericUpDownPosX.Value = pictureBoxImagem.Size.Width - numericUpDownWidth.Value;

                this.ChangeSelectionRect();
            }
        }

        private void numericUpDownPosY_ValueChanged(object sender, EventArgs e)
        {
            if (!this.fillingRectData)
            {
                if (numericUpDownPosY.Value + numericUpDownHeight.Value > pictureBoxImagem.Size.Height)
                    numericUpDownPosY.Value = pictureBoxImagem.Size.Height - numericUpDownHeight.Value;

                this.ChangeSelectionRect();
            }
        }

        private void numericUpDownWidth_ValueChanged(object sender, EventArgs e)
        {
            if (!this.fillingRectData)
            {
                if (numericUpDownPosX.Value + numericUpDownWidth.Value > pictureBoxImagem.Size.Width)
                    numericUpDownWidth.Value = pictureBoxImagem.Size.Width - numericUpDownPosX.Value;

                this.ChangeSelectionRect();
            }
        }

        private void numericUpDownHeight_ValueChanged(object sender, EventArgs e)
        {
            if (!this.fillingRectData)
            {
                if (numericUpDownPosY.Value + numericUpDownHeight.Value > pictureBoxImagem.Size.Height)
                    numericUpDownHeight.Value = pictureBoxImagem.Size.Height - numericUpDownPosY.Value;

                this.ChangeSelectionRect();
            }
        }
    }
}