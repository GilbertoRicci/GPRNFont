using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPRNFont
{
    public partial class FormQuickDivide : Form
    {
        private PictureBox pictureBoxImagem;

        public int GlyphsWidth { get; private set; }
        public int GlyphsHeight { get; private set; }
        public Rectangle DivisionArea { get; private set; }

        public FormQuickDivide(PictureBox pictureBoxImagem, Rectangle divisionArea)
        {
            InitializeComponent();

            this.pictureBoxImagem = pictureBoxImagem;
            this.GlyphsWidth = Convert.ToInt32(this.numericUpDownW.Value);
            this.GlyphsHeight = Convert.ToInt32(this.numericUpDownH.Value);

            this.DivisionArea = divisionArea;

            this.pictureBoxImagem.Invalidate();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void numericUpDownW_ValueChanged(object sender, EventArgs e)
        {
            this.GlyphsWidth = Convert.ToInt32(this.numericUpDownW.Value);
            this.pictureBoxImagem.Invalidate();
        }

        private void numericUpDownH_ValueChanged(object sender, EventArgs e)
        {
            this.GlyphsHeight = Convert.ToInt32(this.numericUpDownH.Value);
            this.pictureBoxImagem.Invalidate();
        }
    }
}