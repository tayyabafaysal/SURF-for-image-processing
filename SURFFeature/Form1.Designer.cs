namespace SURFFeature
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.img_original = new System.Windows.Forms.PictureBox();
            this.img_processed = new System.Windows.Forms.PictureBox();
            this.lblPer = new System.Windows.Forms.Label();
            this.saveProImage = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.img_original)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.img_processed)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 24);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open Image";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(417, 359);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(117, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Process Now";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(230, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Original Image";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(642, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Processed Image";
            // 
            // img_original
            // 
            this.img_original.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.img_original.Image = global::SURFFeature.Properties.Resources.NoImageSelected;
            this.img_original.Location = new System.Drawing.Point(130, 31);
            this.img_original.MaximumSize = new System.Drawing.Size(350, 700);
            this.img_original.Name = "img_original";
            this.img_original.Size = new System.Drawing.Size(280, 608);
            this.img_original.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.img_original.TabIndex = 3;
            this.img_original.TabStop = false;
            // 
            // img_processed
            // 
            this.img_processed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.img_processed.Image = global::SURFFeature.Properties.Resources.NoImageProcessed1;
            this.img_processed.Location = new System.Drawing.Point(540, 31);
            this.img_processed.MaximumSize = new System.Drawing.Size(350, 700);
            this.img_processed.Name = "img_processed";
            this.img_processed.Size = new System.Drawing.Size(280, 608);
            this.img_processed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.img_processed.TabIndex = 1;
            this.img_processed.TabStop = false;
            // 
            // lblPer
            // 
            this.lblPer.AutoSize = true;
            this.lblPer.Location = new System.Drawing.Point(12, 645);
            this.lblPer.Name = "lblPer";
            this.lblPer.Size = new System.Drawing.Size(78, 13);
            this.lblPer.TabIndex = 8;
            this.lblPer.Text = "Process Status";
            // 
            // saveProImage
            // 
            this.saveProImage.Location = new System.Drawing.Point(419, 401);
            this.saveProImage.Name = "saveProImage";
            this.saveProImage.Size = new System.Drawing.Size(115, 23);
            this.saveProImage.TabIndex = 9;
            this.saveProImage.Text = "StoreProcessedImage";
            this.saveProImage.UseVisualStyleBackColor = true;
            this.saveProImage.Click += new System.EventHandler(this.saveProImage_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 670);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1123, 23);
            this.progressBar1.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(865, 741);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.saveProImage);
            this.Controls.Add(this.lblPer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.img_original);
            this.Controls.Add(this.img_processed);
            this.Controls.Add(this.button1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Segmentation";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.img_original)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.img_processed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox img_processed;
        private System.Windows.Forms.PictureBox img_original;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblPer;
        private System.Windows.Forms.Button saveProImage;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}