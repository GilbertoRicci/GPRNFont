namespace GPRNFont
{
    partial class FormQuickDivide
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelW = new System.Windows.Forms.Label();
            this.numericUpDownW = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownH = new System.Windows.Forms.NumericUpDown();
            this.labelH = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownH)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(90, 124);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelW
            // 
            this.labelW.AutoSize = true;
            this.labelW.Location = new System.Drawing.Point(40, 47);
            this.labelW.Name = "labelW";
            this.labelW.Size = new System.Drawing.Size(38, 13);
            this.labelW.TabIndex = 1;
            this.labelW.Text = "Width:";
            // 
            // numericUpDownW
            // 
            this.numericUpDownW.Location = new System.Drawing.Point(92, 45);
            this.numericUpDownW.Maximum = new decimal(new int[] {
            -559939585,
            902409669,
            54,
            0});
            this.numericUpDownW.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownW.Name = "numericUpDownW";
            this.numericUpDownW.Size = new System.Drawing.Size(118, 20);
            this.numericUpDownW.TabIndex = 2;
            this.numericUpDownW.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownW.ValueChanged += new System.EventHandler(this.numericUpDownW_ValueChanged);
            // 
            // numericUpDownH
            // 
            this.numericUpDownH.Location = new System.Drawing.Point(92, 77);
            this.numericUpDownH.Maximum = new decimal(new int[] {
            -559939585,
            902409669,
            54,
            0});
            this.numericUpDownH.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownH.Name = "numericUpDownH";
            this.numericUpDownH.Size = new System.Drawing.Size(118, 20);
            this.numericUpDownH.TabIndex = 4;
            this.numericUpDownH.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownH.ValueChanged += new System.EventHandler(this.numericUpDownH_ValueChanged);
            // 
            // labelH
            // 
            this.labelH.AutoSize = true;
            this.labelH.Location = new System.Drawing.Point(40, 79);
            this.labelH.Name = "labelH";
            this.labelH.Size = new System.Drawing.Size(41, 13);
            this.labelH.TabIndex = 3;
            this.labelH.Text = "Height:";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(15, 15);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(65, 13);
            this.labelTitle.TabIndex = 5;
            this.labelTitle.Text = "Glyphs Size:";
            // 
            // FormQuickDivide
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 159);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.numericUpDownH);
            this.Controls.Add(this.labelH);
            this.Controls.Add(this.numericUpDownW);
            this.Controls.Add(this.labelW);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormQuickDivide";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quick Divide";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownH)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelW;
        private System.Windows.Forms.NumericUpDown numericUpDownW;
        private System.Windows.Forms.NumericUpDown numericUpDownH;
        private System.Windows.Forms.Label labelH;
        private System.Windows.Forms.Label labelTitle;
    }
}