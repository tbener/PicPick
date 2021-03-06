﻿namespace PicPick.UserControls
{
    partial class ImageInfo
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPath = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.panelPic = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblPath
            // 
            this.lblPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPath.Location = new System.Drawing.Point(92, 0);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(292, 47);
            this.lblPath.TabIndex = 1;
            this.lblPath.Text = "C:\\....";
            // 
            // lblDate
            // 
            this.lblDate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblDate.Location = new System.Drawing.Point(92, 71);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(292, 24);
            this.lblDate.TabIndex = 2;
            this.lblDate.Text = "Date:";
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblSize
            // 
            this.lblSize.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblSize.Location = new System.Drawing.Point(92, 47);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(292, 24);
            this.lblSize.TabIndex = 3;
            this.lblSize.Text = "Size:";
            this.lblSize.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // panelPic
            // 
            this.panelPic.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelPic.Location = new System.Drawing.Point(0, 0);
            this.panelPic.Name = "panelPic";
            this.panelPic.Size = new System.Drawing.Size(92, 95);
            this.panelPic.TabIndex = 4;
            this.panelPic.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPic_Paint);
            // 
            // ImageInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.panelPic);
            this.Name = "ImageInfo";
            this.Size = new System.Drawing.Size(384, 95);
            this.Resize += new System.EventHandler(this.ImageInfo_Resize);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Panel panelPic;
    }
}
