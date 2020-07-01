namespace GPRNFont
{
    partial class FormImport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxImage = new System.Windows.Forms.TextBox();
            this.buttonImage = new System.Windows.Forms.Button();
            this.groupBoxImage = new System.Windows.Forms.GroupBox();
            this.groupBoxFontsettings = new System.Windows.Forms.GroupBox();
            this.textBoxFontsettings = new System.Windows.Forms.TextBox();
            this.buttonFontsettings = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.groupBoxImage.SuspendLayout();
            this.groupBoxFontsettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxImage
            // 
            this.textBoxImage.Location = new System.Drawing.Point(104, 31);
            this.textBoxImage.Name = "textBoxImage";
            this.textBoxImage.Size = new System.Drawing.Size(247, 20);
            this.textBoxImage.TabIndex = 5;
            // 
            // buttonImage
            // 
            this.buttonImage.Location = new System.Drawing.Point(23, 29);
            this.buttonImage.Name = "buttonImage";
            this.buttonImage.Size = new System.Drawing.Size(75, 23);
            this.buttonImage.TabIndex = 4;
            this.buttonImage.Text = "Browse...";
            this.buttonImage.UseVisualStyleBackColor = true;
            this.buttonImage.Click += new System.EventHandler(this.buttonImage_Click);
            // 
            // groupBoxImage
            // 
            this.groupBoxImage.Controls.Add(this.textBoxImage);
            this.groupBoxImage.Controls.Add(this.buttonImage);
            this.groupBoxImage.Location = new System.Drawing.Point(12, 12);
            this.groupBoxImage.Name = "groupBoxImage";
            this.groupBoxImage.Size = new System.Drawing.Size(381, 74);
            this.groupBoxImage.TabIndex = 6;
            this.groupBoxImage.TabStop = false;
            this.groupBoxImage.Text = "Image file:";
            // 
            // groupBoxFontsettings
            // 
            this.groupBoxFontsettings.Controls.Add(this.textBoxFontsettings);
            this.groupBoxFontsettings.Controls.Add(this.buttonFontsettings);
            this.groupBoxFontsettings.Location = new System.Drawing.Point(11, 92);
            this.groupBoxFontsettings.Name = "groupBoxFontsettings";
            this.groupBoxFontsettings.Size = new System.Drawing.Size(382, 74);
            this.groupBoxFontsettings.TabIndex = 7;
            this.groupBoxFontsettings.TabStop = false;
            this.groupBoxFontsettings.Text = ".fontsettings file:";
            // 
            // textBoxFontsettings
            // 
            this.textBoxFontsettings.Location = new System.Drawing.Point(104, 31);
            this.textBoxFontsettings.Name = "textBoxFontsettings";
            this.textBoxFontsettings.Size = new System.Drawing.Size(247, 20);
            this.textBoxFontsettings.TabIndex = 5;
            // 
            // buttonFontsettings
            // 
            this.buttonFontsettings.Location = new System.Drawing.Point(23, 29);
            this.buttonFontsettings.Name = "buttonFontsettings";
            this.buttonFontsettings.Size = new System.Drawing.Size(75, 23);
            this.buttonFontsettings.TabIndex = 4;
            this.buttonFontsettings.Text = "Browse...";
            this.buttonFontsettings.UseVisualStyleBackColor = true;
            this.buttonFontsettings.Click += new System.EventHandler(this.buttonFontsettings_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(160, 172);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(75, 23);
            this.buttonImport.TabIndex = 8;
            this.buttonImport.Text = "Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // FormImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 208);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.groupBoxFontsettings);
            this.Controls.Add(this.groupBoxImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormImport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Import";
            this.groupBoxImage.ResumeLayout(false);
            this.groupBoxImage.PerformLayout();
            this.groupBoxFontsettings.ResumeLayout(false);
            this.groupBoxFontsettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxImage;
        private System.Windows.Forms.Button buttonImage;
        private System.Windows.Forms.GroupBox groupBoxImage;
        private System.Windows.Forms.GroupBox groupBoxFontsettings;
        private System.Windows.Forms.TextBox textBoxFontsettings;
        private System.Windows.Forms.Button buttonFontsettings;
        private System.Windows.Forms.Button buttonImport;
    }
}