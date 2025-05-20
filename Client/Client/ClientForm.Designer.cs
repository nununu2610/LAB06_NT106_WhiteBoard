namespace Client
{
    partial class ClientForm
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
            this.panelWhiteboard = new System.Windows.Forms.Panel();
            this.comboBoxColor = new System.Windows.Forms.ComboBox();
            this.numericUpDownThickness = new System.Windows.Forms.NumericUpDown();
            this.buttonEnd = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThickness)).BeginInit();
            this.SuspendLayout();
            // 
            // panelWhiteboard
            // 
            this.panelWhiteboard.Location = new System.Drawing.Point(55, 26);
            this.panelWhiteboard.Name = "panelWhiteboard";
            this.panelWhiteboard.Size = new System.Drawing.Size(753, 278);
            this.panelWhiteboard.TabIndex = 0;
            this.panelWhiteboard.Paint += new System.Windows.Forms.PaintEventHandler(this.panelWhiteboard_Paint);
            // 
            // comboBoxColor
            // 
            this.comboBoxColor.FormattingEnabled = true;
            this.comboBoxColor.Location = new System.Drawing.Point(55, 330);
            this.comboBoxColor.Name = "comboBoxColor";
            this.comboBoxColor.Size = new System.Drawing.Size(619, 24);
            this.comboBoxColor.TabIndex = 1;
            // 
            // numericUpDownThickness
            // 
            this.numericUpDownThickness.Location = new System.Drawing.Point(55, 397);
            this.numericUpDownThickness.Name = "numericUpDownThickness";
            this.numericUpDownThickness.Size = new System.Drawing.Size(142, 22);
            this.numericUpDownThickness.TabIndex = 2;
            // 
            // buttonEnd
            // 
            this.buttonEnd.Location = new System.Drawing.Point(637, 408);
            this.buttonEnd.Name = "buttonEnd";
            this.buttonEnd.Size = new System.Drawing.Size(198, 76);
            this.buttonEnd.TabIndex = 3;
            this.buttonEnd.Text = "button1";
            this.buttonEnd.UseVisualStyleBackColor = true;
            this.buttonEnd.Click += new System.EventHandler(this.buttonEnd_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(814, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 508);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonEnd);
            this.Controls.Add(this.numericUpDownThickness);
            this.Controls.Add(this.comboBoxColor);
            this.Controls.Add(this.panelWhiteboard);
            this.Name = "ClientForm";
            this.Text = "s";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThickness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelWhiteboard;
        private System.Windows.Forms.ComboBox comboBoxColor;
        private System.Windows.Forms.NumericUpDown numericUpDownThickness;
        private System.Windows.Forms.Button buttonEnd;
        private System.Windows.Forms.Label label1;
    }
}

