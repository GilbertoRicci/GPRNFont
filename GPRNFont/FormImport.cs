using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPRNFont
{
    public partial class FormImport : Form
    {
        private string FontsettingsPath
        {
            get
            {
                return textBoxFontsettings.Text;
            }
        }

        public string ImagePath
        {
            get
            {
                return textBoxImage.Text;
            }
        }

        public List<GlyphData> GlyphDataList
        {
            get;
            private set;
        }

        public FormImport()
        {
            InitializeComponent();
        }

        private bool CheckExtension(string filePath, string[] extensions)
        {
            foreach (var ext in extensions)
                if (Path.GetExtension(filePath) == ext)
                    return true;

            return false;
        }

        private Rectangle ImportGlyphRect(double uvX, double uvY, double uvW, double uvH, double vertW, double vertH, Size imgSize)
        {
            var rect = new Rectangle(0, 0, 1, 1);

            if (vertW > 0.0)
            {
                rect.X = Convert.ToInt32(uvX * vertW * (imgSize.Width / vertW));
                rect.Width = Convert.ToInt32(vertW);
            }

            if (vertH < 0.0)
            {
                rect.Y = Convert.ToInt32((((1.0 - uvY) * (imgSize.Height / -vertH)) - 1.0) * -vertH);
                rect.Height = Convert.ToInt32(-vertH);
            }

            return rect;
        }

        private void ImportGlyphsData()
        {
            var imgSize = new Bitmap(this.ImagePath).Size;

            this.GlyphDataList = new List<GlyphData>();

            using (var reader = new StreamReader(this.FontsettingsPath))
            {
                var indexIdentifier = "    index: ";
                var xIdentifier = "      x: ";
                var yIdentifier = "      y: ";
                var wIdentifier = "      width: ";
                var hIdentifier = "      height: ";
                var vertSession = false;

                GlyphData currentGlyphData = null;
                double uvX = Double.NaN;
                double uvY = Double.NaN;
                double uvW = Double.NaN;
                double uvH = Double.NaN;
                double vertW = Double.NaN;
                double vertH = Double.NaN;

                string line = "";
                do
                {
                    Console.WriteLine(line);
                    if (line.Contains(indexIdentifier))
                    {
                        //create glyph data
                        currentGlyphData = new GlyphData();
                        var indexStr = line.Replace(indexIdentifier, "");
                        var index = Convert.ToInt32(indexStr);
                        currentGlyphData.Glyph = (char)index;
                        this.GlyphDataList.Add(currentGlyphData);

                        uvX = Double.NaN;
                        uvY = Double.NaN;
                        uvW = Double.NaN;
                        uvH = Double.NaN;
                        vertW = Double.NaN;
                        vertH = Double.NaN;
                    }
                    else if (!vertSession && line.Contains(xIdentifier))
                    {
                        var str = line.Replace(xIdentifier, "");
                        uvX = Convert.ToDouble(str, CultureInfo.GetCultureInfo("en-US"));
                    }
                    else if (!vertSession && line.Contains(yIdentifier))
                    {
                        var str = line.Replace(yIdentifier, "");
                        uvY = Convert.ToDouble(str, CultureInfo.GetCultureInfo("en-US"));
                    }
                    else if (line.Contains(wIdentifier))
                    {
                        var str = line.Replace(wIdentifier, "");
                        var dbl = Convert.ToDouble(str, CultureInfo.GetCultureInfo("en-US"));

                        if (vertSession)
                            vertW = dbl;
                        else
                            uvW = dbl;
                    }
                    else if (line.Contains(hIdentifier))
                    {
                        var str = line.Replace(hIdentifier, "");
                        var dbl = Convert.ToDouble(str, CultureInfo.GetCultureInfo("en-US"));

                        if (vertSession)
                        {
                            vertH = dbl;

                            //insert glyph data
                            currentGlyphData.SetGlyphRect(this.ImportGlyphRect(uvX, uvY, uvW, uvH, vertW, vertH, imgSize), 100);
                        }
                        else
                            uvH = dbl;

                        vertSession = !vertSession;
                    }

                    line = reader.ReadLine();
                }
                while (line != null);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!File.Exists(this.ImagePath) || !this.CheckExtension(this.ImagePath, FormPrincipal.IMG_EXTENSIONS))
                MessageBox.Show("Image file '" + this.ImagePath + "' do not exist or is not valid.", "GPRNFont", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!File.Exists(this.FontsettingsPath) || !this.CheckExtension(this.FontsettingsPath, FormPrincipal.FONTSETTINGS_EXTENSION))
                MessageBox.Show(".fontssetings file '" + this.FontsettingsPath + "' do not exist or is not valid.", "GPRNFont", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                this.ImportGlyphsData();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttonImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Import image file";
                dlg.Filter = FormPrincipal.GetImageExtensionString();

                if (dlg.ShowDialog() == DialogResult.OK)
                    textBoxImage.Text = dlg.FileName;
            }
        }

        private void buttonFontsettings_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Impor .fontsettings file";
                dlg.Filter = FormPrincipal.GetFontsettingsExtensionString();

                if (dlg.ShowDialog() == DialogResult.OK)
                    textBoxFontsettings.Text = dlg.FileName;
            }
        }
    }
}
