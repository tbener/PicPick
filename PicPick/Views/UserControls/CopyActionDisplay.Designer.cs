namespace PicPick.UserControls
{
    partial class CopyActionDisplay
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
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            this.imageInfoControl = new PicPick.UserControls.ImageInfo();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblHeader.Location = new System.Drawing.Point(0, 3);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(149, 18);
            this.lblHeader.TabIndex = 2;
            this.lblHeader.Text = "Copy and Replace";
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblText.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblText.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblText.Location = new System.Drawing.Point(0, 21);
            this.lblText.Name = "lblText";
            this.lblText.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.lblText.Size = new System.Drawing.Size(249, 22);
            this.lblText.TabIndex = 3;
            this.lblText.Text = "Replace the file in the destination folder.";
            // 
            // imageInfoControl
            // 
            this.imageInfoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageInfoControl.ImagePath = null;
            this.imageInfoControl.Location = new System.Drawing.Point(0, 43);
            this.imageInfoControl.Name = "imageInfoControl";
            this.imageInfoControl.Size = new System.Drawing.Size(530, 94);
            this.imageInfoControl.TabIndex = 4;
            // 
            // CopyActionDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.imageInfoControl);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.lblHeader);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "CopyActionDisplay";
            this.Padding = new System.Windows.Forms.Padding(0, 3, 0, 6);
            this.Size = new System.Drawing.Size(530, 143);
            this.MouseEnter += new System.EventHandler(this.CopyActionDisplay_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.CopyActionDisplay_MouseLeave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblText;
        private ImageInfo imageInfoControl;
    }
}
