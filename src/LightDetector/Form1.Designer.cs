namespace LightDetector
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BtnOpen = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.Group1 = new System.Windows.Forms.GroupBox();
            this.GvData = new System.Windows.Forms.DataGridView();
            this.BtnReset = new System.Windows.Forms.Button();
            this.BtnExport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.Group1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GvData)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 68);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(652, 426);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // BtnOpen
            // 
            this.BtnOpen.Location = new System.Drawing.Point(12, 12);
            this.BtnOpen.Name = "BtnOpen";
            this.BtnOpen.Size = new System.Drawing.Size(118, 50);
            this.BtnOpen.TabIndex = 1;
            this.BtnOpen.Text = "&Open Video";
            this.BtnOpen.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(670, 68);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(652, 426);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // Group1
            // 
            this.Group1.Controls.Add(this.GvData);
            this.Group1.Location = new System.Drawing.Point(12, 500);
            this.Group1.Name = "Group1";
            this.Group1.Size = new System.Drawing.Size(1310, 228);
            this.Group1.TabIndex = 4;
            this.Group1.TabStop = false;
            this.Group1.Text = "Summary";
            // 
            // GvData
            // 
            this.GvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GvData.Location = new System.Drawing.Point(6, 26);
            this.GvData.Name = "GvData";
            this.GvData.RowHeadersWidth = 51;
            this.GvData.RowTemplate.Height = 29;
            this.GvData.Size = new System.Drawing.Size(1298, 188);
            this.GvData.TabIndex = 4;
            // 
            // BtnReset
            // 
            this.BtnReset.Location = new System.Drawing.Point(136, 12);
            this.BtnReset.Name = "BtnReset";
            this.BtnReset.Size = new System.Drawing.Size(118, 50);
            this.BtnReset.TabIndex = 5;
            this.BtnReset.Text = "&Reset Data";
            this.BtnReset.UseVisualStyleBackColor = true;
            // 
            // BtnExport
            // 
            this.BtnExport.Location = new System.Drawing.Point(260, 12);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(118, 50);
            this.BtnExport.TabIndex = 6;
            this.BtnExport.Text = "&Export to CSV";
            this.BtnExport.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1334, 740);
            this.Controls.Add(this.BtnExport);
            this.Controls.Add(this.BtnReset);
            this.Controls.Add(this.Group1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.BtnOpen);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Light Detector Sample - BMC (c) 2022";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.Group1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox pictureBox1;
        private Button BtnOpen;
        private PictureBox pictureBox2;
        private GroupBox Group1;
        private DataGridView GvData;
        private Button BtnReset;
        private Button BtnExport;
    }
}