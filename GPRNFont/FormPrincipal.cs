﻿using System;
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
        private Image originalImage;
        private int zoom = 100;

        public FormPrincipal()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.selectionManager = new SelectionManager();
        }

        private void DrawImageCopy()
        {
            var selectedArea = new Rectangle(Convert.ToInt32(numericUpDownPosX.Value), Convert.ToInt32(numericUpDownPosY.Value),
                                                Convert.ToInt32(numericUpDownWidth.Value), Convert.ToInt32(numericUpDownHeight.Value));

            pictureBoxPedacoImg.Image = new Bitmap(pictureBoxPedacoImg.Width, pictureBoxPedacoImg.Height);
            using (var g = Graphics.FromImage(pictureBoxPedacoImg.Image))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(this.originalImage, new Rectangle(0, 0, pictureBoxPedacoImg.Width, pictureBoxPedacoImg.Height), selectedArea, GraphicsUnit.Pixel);
            }
        }

        private void FillRectData(Rectangle selectedArea)
        {
            this.fillingRectData = true;

            if (!selectedArea.IsEmpty)
            {
                numericUpDownPosX.Value = selectedArea.X * 100 / this.zoom;
                numericUpDownPosY.Value = selectedArea.Y * 100 / this.zoom;
                numericUpDownWidth.Value = selectedArea.Width * 100 / this.zoom;
                numericUpDownHeight.Value = selectedArea.Height * 100 / this.zoom;
            }

            this.fillingRectData = false;
        }

        private void CalcUnityData(Rectangle selectedArea)
        {
            if (!selectedArea.IsEmpty)
            {
                decimal imgWidth = pictureBoxImagem.Image.Size.Width;
                decimal imgHeight = pictureBoxImagem.Image.Size.Height;
                decimal charPosX = selectedArea.X;
                decimal charPosY = selectedArea.Y;
                decimal charWidth = selectedArea.Width;
                decimal charHeight = selectedArea.Height;

                numericUpDownAdvance.Value = charWidth;
                numericUpDownUVX.Value = 1 / (imgWidth / charWidth) * charPosX / charWidth;
                numericUpDownUVY.Value = 1 - 1 / (imgHeight / charHeight) * ((charPosY / charHeight) + 1);
                numericUpDownUVW.Value = 1 / (imgWidth / charWidth);
                numericUpDownUVH.Value = 1 / (imgHeight / charHeight);
                numericUpDownVertX.Value = 0;
                numericUpDownVertY.Value = 0;
                numericUpDownVertW.Value = charWidth;
                numericUpDownVertH.Value = -charHeight;
            }
        }

        private void EnableTextBoxes(bool enable)
        {
            textBoxGlyph.Enabled = enable;
            numericUpDownPosX.Enabled = enable;
            numericUpDownPosY.Enabled = enable;
            numericUpDownWidth.Enabled = enable;
            numericUpDownHeight.Enabled = enable;
            numericUpDownIndex.Enabled = enable;
            numericUpDownAdvance.Enabled = enable;
            numericUpDownUVX.Enabled = enable;
            numericUpDownUVY.Enabled = enable;
            numericUpDownUVW.Enabled = enable;
            numericUpDownUVH.Enabled = enable;
            numericUpDownVertX.Enabled = enable;
            numericUpDownVertY.Enabled = enable;
            numericUpDownVertW.Enabled = enable;
            numericUpDownVertH.Enabled = enable;

            if (!enable)
            {
                this.fillingRectData = true;

                numericUpDownPosX.Value = 0;
                numericUpDownPosY.Value = 0;
                numericUpDownWidth.Value = 0;
                numericUpDownHeight.Value = 0;
                textBoxGlyph.Text = "";
                numericUpDownAdvance.Value = 0;
                numericUpDownUVX.Value = 0;
                numericUpDownUVY.Value = 0;
                numericUpDownUVW.Value = 0;
                numericUpDownUVH.Value = 0;
                numericUpDownVertX.Value = 0;
                numericUpDownVertY.Value = 0;
                numericUpDownVertW.Value = 0;
                numericUpDownVertH.Value = 0;

                this.fillingRectData = false;
            }
        }

        private void UpdateGlyphData(Rectangle selectedArea)
        {
            this.DrawImageCopy();
            this.CalcUnityData(selectedArea);
            this.EnableTextBoxes(!selectedArea.IsEmpty);
        }

        private void ChangeSelectionRect()
        {
            var newSelectedArea = new Rectangle(Decimal.ToInt32(numericUpDownPosX.Value), Decimal.ToInt32(numericUpDownPosY.Value),
                                    Decimal.ToInt32(numericUpDownWidth.Value), Decimal.ToInt32(numericUpDownHeight.Value));

            this.selectionManager.ChangeSelectionRect(newSelectedArea);
            this.UpdateGlyphData(newSelectedArea);
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
                    this.originalImage = new Bitmap(dlg.FileName);
                    pictureBoxImagem.Image = originalImage;

                    toolStripButtonZoomMinus.Enabled = true;
                    toolStripButtonZoomPlus.Enabled = true;
                }
            }
        }

        public void ZoomApply()
        {
            this.pictureBoxImagem.Image = new Bitmap(this.originalImage, Convert.ToInt32(originalImage.Width * this.zoom / 100), Convert.ToInt32(originalImage.Height * this.zoom / 100));
            Graphics grap = Graphics.FromImage(this.pictureBoxImagem.Image);
            grap.InterpolationMode = InterpolationMode.NearestNeighbor;

            this.toolStripTextBoxZoom.Text = this.zoom + "%";
            this.selectionManager.ZoomApply(this.zoom);

            pictureBoxImagem.Invalidate();
        }

        private void pictureBoxImagem_MouseDown(object sender, MouseEventArgs e)
        {
            this.selectionManager.StartSelection(pictureBoxImagem.PointToClient(MousePosition), this.zoom);
            this.UpdateGlyphData(this.selectionManager.GetSelectedArea());

            pictureBoxImagem.Invalidate();
        }

        private void pictureBoxImagem_MouseUp(object sender, MouseEventArgs e)
        {
            this.selectionManager.EndSelection(pictureBoxImagem.PointToClient(MousePosition), pictureBoxImagem.Size);
            var selectedArea = this.selectionManager.GetSelectedArea();

            this.FillRectData(selectedArea);
            this.UpdateGlyphData(selectedArea);

            pictureBoxImagem.Invalidate();
        }

        private void pictureBoxImagem_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePoint = pictureBoxImagem.PointToClient(MousePosition);

            this.selectionManager.UpdateSelection(mousePoint, pictureBoxImagem.Size);

            Cursor.Current = this.selectionManager.GetCursor(mousePoint);

            pictureBoxImagem.Invalidate();
        }

        private void pictureBoxImagem_Paint(object sender, PaintEventArgs e)
        {
            this.selectionManager.DrawSelectionRect(e.Graphics, 100);
        }

        private void textBoxGlyph_TextChanged(object sender, EventArgs e)
        {
            var glyph = textBoxGlyph.Text.ToCharArray();
            buttonSaveGlyph.Enabled = glyph.Length == 1;
            if (buttonSaveGlyph.Enabled)
                numericUpDownIndex.Value = (int) glyph[0];
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

        private void toolStripButtonZoomPlus_Click(object sender, EventArgs e)
        {
            this.zoom *= 2;

            if (this.zoom >= 800)
                toolStripButtonZoomPlus.Enabled = false;
            toolStripButtonZoomMinus.Enabled = true;

            this.ZoomApply();
        }

        private void toolStripButtonZoomLess_Click(object sender, EventArgs e)
        {
            this.zoom /= 2;

            if (this.zoom <= 25)
                toolStripButtonZoomMinus.Enabled = false;
            toolStripButtonZoomPlus.Enabled = true;

            this.ZoomApply();
        }
    }
}