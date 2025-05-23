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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientForm));
            this.panelWhiteboard = new System.Windows.Forms.Panel();
            this.numericUpDownThickness = new System.Windows.Forms.NumericUpDown();
            this.buttonEnd = new System.Windows.Forms.Button();
            this.btnChooseColor = new System.Windows.Forms.Button();
            this.txtImageUrl = new System.Windows.Forms.TextBox();
            this.btnInsertImage = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.chkEraser = new System.Windows.Forms.CheckBox();
            this.btnIncreaseThickness = new System.Windows.Forms.Button();
            this.btnDecreaseThickness = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThickness)).BeginInit();
            this.SuspendLayout();
            // 
            // panelWhiteboard
            // 
            this.panelWhiteboard.Location = new System.Drawing.Point(17, 12);
            this.panelWhiteboard.Name = "panelWhiteboard";
            this.panelWhiteboard.Size = new System.Drawing.Size(857, 376);
            this.panelWhiteboard.TabIndex = 0;
            // 
            // numericUpDownThickness
            // 
            this.numericUpDownThickness.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericUpDownThickness.Cursor = System.Windows.Forms.Cursors.Default;
            this.numericUpDownThickness.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownThickness.ForeColor = System.Drawing.Color.Navy;
            this.numericUpDownThickness.Location = new System.Drawing.Point(439, 404);
            this.numericUpDownThickness.Name = "numericUpDownThickness";
            this.numericUpDownThickness.Size = new System.Drawing.Size(41, 30);
            this.numericUpDownThickness.TabIndex = 2;
            // 
            // buttonEnd
            // 
            this.buttonEnd.Font = new System.Drawing.Font("Segoe UI Black", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnd.ForeColor = System.Drawing.Color.Navy;
            this.buttonEnd.Location = new System.Drawing.Point(764, 451);
            this.buttonEnd.Name = "buttonEnd";
            this.buttonEnd.Size = new System.Drawing.Size(80, 39);
            this.buttonEnd.TabIndex = 3;
            this.buttonEnd.Text = "SAVE";
            this.buttonEnd.UseVisualStyleBackColor = true;
            this.buttonEnd.Click += new System.EventHandler(this.buttonEnd_Click);
            // 
            // btnChooseColor
            // 
            this.btnChooseColor.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChooseColor.ForeColor = System.Drawing.Color.Navy;
            this.btnChooseColor.Location = new System.Drawing.Point(225, 403);
            this.btnChooseColor.Name = "btnChooseColor";
            this.btnChooseColor.Size = new System.Drawing.Size(80, 33);
            this.btnChooseColor.TabIndex = 5;
            this.btnChooseColor.Text = "Color";
            this.btnChooseColor.UseVisualStyleBackColor = true;
            // 
            // txtImageUrl
            // 
            this.txtImageUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtImageUrl.Location = new System.Drawing.Point(233, 459);
            this.txtImageUrl.Name = "txtImageUrl";
            this.txtImageUrl.Size = new System.Drawing.Size(355, 27);
            this.txtImageUrl.TabIndex = 6;
            // 
            // btnInsertImage
            // 
            this.btnInsertImage.Font = new System.Drawing.Font("Segoe UI Black", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertImage.ForeColor = System.Drawing.Color.Navy;
            this.btnInsertImage.Location = new System.Drawing.Point(665, 451);
            this.btnInsertImage.Name = "btnInsertImage";
            this.btnInsertImage.Size = new System.Drawing.Size(84, 39);
            this.btnInsertImage.TabIndex = 7;
            this.btnInsertImage.Text = "INSERT";
            this.btnInsertImage.UseVisualStyleBackColor = true;
            this.btnInsertImage.Click += new System.EventHandler(this.btnInsertImage_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Font = new System.Drawing.Font("Segoe UI Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(29, 459);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 28);
            this.label2.TabIndex = 8;
            this.label2.Text = "Enter Image URL:";
            // 
            // chkEraser
            // 
            this.chkEraser.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkEraser.AutoSize = true;
            this.chkEraser.BackColor = System.Drawing.Color.Transparent;
            this.chkEraser.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold);
            this.chkEraser.ForeColor = System.Drawing.Color.Navy;
            this.chkEraser.Location = new System.Drawing.Point(311, 403);
            this.chkEraser.Name = "chkEraser";
            this.chkEraser.Size = new System.Drawing.Size(71, 33);
            this.chkEraser.TabIndex = 9;
            this.chkEraser.Text = "Eraser";
            this.chkEraser.UseVisualStyleBackColor = false;
            // 
            // btnIncreaseThickness
            // 
            this.btnIncreaseThickness.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIncreaseThickness.ForeColor = System.Drawing.Color.Navy;
            this.btnIncreaseThickness.Location = new System.Drawing.Point(486, 401);
            this.btnIncreaseThickness.Name = "btnIncreaseThickness";
            this.btnIncreaseThickness.Size = new System.Drawing.Size(37, 35);
            this.btnIncreaseThickness.TabIndex = 10;
            this.btnIncreaseThickness.Text = "+";
            this.btnIncreaseThickness.UseVisualStyleBackColor = true;
            this.btnIncreaseThickness.Click += new System.EventHandler(this.btnIncreaseThickness_Click);
            // 
            // btnDecreaseThickness
            // 
            this.btnDecreaseThickness.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDecreaseThickness.ForeColor = System.Drawing.Color.Navy;
            this.btnDecreaseThickness.Location = new System.Drawing.Point(529, 401);
            this.btnDecreaseThickness.Name = "btnDecreaseThickness";
            this.btnDecreaseThickness.Size = new System.Drawing.Size(37, 35);
            this.btnDecreaseThickness.TabIndex = 11;
            this.btnDecreaseThickness.Text = "-";
            this.btnDecreaseThickness.UseVisualStyleBackColor = true;
            this.btnDecreaseThickness.Click += new System.EventHandler(this.btnDecreaseThickness_Click);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(886, 508);
            this.Controls.Add(this.btnDecreaseThickness);
            this.Controls.Add(this.btnIncreaseThickness);
            this.Controls.Add(this.chkEraser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnInsertImage);
            this.Controls.Add(this.txtImageUrl);
            this.Controls.Add(this.btnChooseColor);
            this.Controls.Add(this.buttonEnd);
            this.Controls.Add(this.numericUpDownThickness);
            this.Controls.Add(this.panelWhiteboard);
            this.DoubleBuffered = true;
            this.Name = "ClientForm";
            this.Text = "DrawingPannel";
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
        private System.Windows.Forms.CheckBox chkEraser;
        private System.Windows.Forms.Button btnIncreaseThickness;
        private System.Windows.Forms.Button btnDecreaseThickness;
    }
}

