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
            this.numericUpDownThickness = new System.Windows.Forms.NumericUpDown();
            this.buttonEnd = new System.Windows.Forms.Button();
            this.btnChooseColor = new System.Windows.Forms.Button();
            this.txtImageUrl = new System.Windows.Forms.TextBox();
            this.btnInsertImage = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThickness)).BeginInit();
            this.SuspendLayout();
            // 
            // panelWhiteboard
            // 
            this.panelWhiteboard.Location = new System.Drawing.Point(17, 26);
            this.panelWhiteboard.Name = "panelWhiteboard";
            this.panelWhiteboard.Size = new System.Drawing.Size(857, 362);
            this.panelWhiteboard.TabIndex = 0;
            // 
            // numericUpDownThickness
            // 
            this.numericUpDownThickness.Location = new System.Drawing.Point(140, 402);
            this.numericUpDownThickness.Name = "numericUpDownThickness";
            this.numericUpDownThickness.Size = new System.Drawing.Size(96, 22);
            this.numericUpDownThickness.TabIndex = 2;
            // 
            // buttonEnd
            // 
            this.buttonEnd.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnd.ForeColor = System.Drawing.Color.DarkGreen;
            this.buttonEnd.Location = new System.Drawing.Point(722, 434);
            this.buttonEnd.Name = "buttonEnd";
            this.buttonEnd.Size = new System.Drawing.Size(105, 39);
            this.buttonEnd.TabIndex = 3;
            this.buttonEnd.Text = "SAVE";
            this.buttonEnd.UseVisualStyleBackColor = true;
            this.buttonEnd.Click += new System.EventHandler(this.buttonEnd_Click);
            // 
            // btnChooseColor
            // 
            this.btnChooseColor.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChooseColor.ForeColor = System.Drawing.Color.Navy;
            this.btnChooseColor.Location = new System.Drawing.Point(46, 394);
            this.btnChooseColor.Name = "btnChooseColor";
            this.btnChooseColor.Size = new System.Drawing.Size(88, 42);
            this.btnChooseColor.TabIndex = 5;
            this.btnChooseColor.Text = "COLOR";
            this.btnChooseColor.UseVisualStyleBackColor = true;
            this.btnChooseColor.Click += new System.EventHandler(this.btnChooseColor_Click);
            // 
            // txtImageUrl
            // 
            this.txtImageUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtImageUrl.Location = new System.Drawing.Point(216, 442);
            this.txtImageUrl.Name = "txtImageUrl";
            this.txtImageUrl.Size = new System.Drawing.Size(355, 27);
            this.txtImageUrl.TabIndex = 6;
            // 
            // btnInsertImage
            // 
            this.btnInsertImage.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertImage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnInsertImage.Location = new System.Drawing.Point(589, 434);
            this.btnInsertImage.Name = "btnInsertImage";
            this.btnInsertImage.Size = new System.Drawing.Size(111, 39);
            this.btnInsertImage.TabIndex = 7;
            this.btnInsertImage.Text = "INSERT";
            this.btnInsertImage.UseVisualStyleBackColor = true;
            this.btnInsertImage.Click += new System.EventHandler(this.btnInsertImage_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(12, 442);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 28);
            this.label2.TabIndex = 8;
            this.label2.Text = "Enter Image URL:";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 508);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnInsertImage);
            this.Controls.Add(this.txtImageUrl);
            this.Controls.Add(this.btnChooseColor);
            this.Controls.Add(this.buttonEnd);
            this.Controls.Add(this.numericUpDownThickness);
            this.Controls.Add(this.panelWhiteboard);
            this.Name = "ClientForm";
            this.Text = "s";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThickness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelWhiteboard;
        private System.Windows.Forms.NumericUpDown numericUpDownThickness;
        private System.Windows.Forms.Button buttonEnd;
        private System.Windows.Forms.Button btnChooseColor;
        private System.Windows.Forms.TextBox txtImageUrl;
        private System.Windows.Forms.Button btnInsertImage;
        private System.Windows.Forms.Label label2;
    }
}

