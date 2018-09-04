namespace PicPick.Forms
{
    partial class AskWhatToDoForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.copyActionKeepBoth = new PicPick.UserControls.CopyActionDisplay();
            this.panel3 = new System.Windows.Forms.Panel();
            this.copyActionSkip = new PicPick.UserControls.CopyActionDisplay();
            this.panel2 = new System.Windows.Forms.Panel();
            this.copyActionOverwrite = new PicPick.UserControls.CopyActionDisplay();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblDestPath = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkDontAskAgain = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(27, 98);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 337);
            this.panel1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.copyActionKeepBoth);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 254);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(512, 75);
            this.panel4.TabIndex = 2;
            // 
            // copyActionKeepBoth
            // 
            this.copyActionKeepBoth.BackColor = System.Drawing.SystemColors.Control;
            this.copyActionKeepBoth.Cursor = System.Windows.Forms.Cursors.Hand;
            this.copyActionKeepBoth.Details = "The file you are copying will be renamed";
            this.copyActionKeepBoth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.copyActionKeepBoth.Header = "Copy, but keep both files";
            this.copyActionKeepBoth.ImagePaneVisible = false;
            this.copyActionKeepBoth.Location = new System.Drawing.Point(0, 0);
            this.copyActionKeepBoth.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.copyActionKeepBoth.Name = "copyActionKeepBoth";
            this.copyActionKeepBoth.Padding = new System.Windows.Forms.Padding(0, 3, 0, 6);
            this.copyActionKeepBoth.Size = new System.Drawing.Size(512, 75);
            this.copyActionKeepBoth.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.copyActionSkip);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 127);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(512, 127);
            this.panel3.TabIndex = 1;
            // 
            // copyActionSkip
            // 
            this.copyActionSkip.BackColor = System.Drawing.SystemColors.Control;
            this.copyActionSkip.Cursor = System.Windows.Forms.Cursors.Hand;
            this.copyActionSkip.Details = "No files will be changed. Leave this file in the destination folder:";
            this.copyActionSkip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.copyActionSkip.Header = "Don\'t copy";
            this.copyActionSkip.ImagePaneVisible = true;
            this.copyActionSkip.Location = new System.Drawing.Point(0, 0);
            this.copyActionSkip.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.copyActionSkip.Name = "copyActionSkip";
            this.copyActionSkip.Padding = new System.Windows.Forms.Padding(0, 3, 0, 6);
            this.copyActionSkip.Size = new System.Drawing.Size(512, 127);
            this.copyActionSkip.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.copyActionOverwrite);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(512, 127);
            this.panel2.TabIndex = 0;
            // 
            // copyActionOverwrite
            // 
            this.copyActionOverwrite.BackColor = System.Drawing.SystemColors.Control;
            this.copyActionOverwrite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.copyActionOverwrite.Details = "Replace the file in the destination folder.";
            this.copyActionOverwrite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.copyActionOverwrite.Header = "Copy and Replace";
            this.copyActionOverwrite.ImagePaneVisible = true;
            this.copyActionOverwrite.Location = new System.Drawing.Point(0, 0);
            this.copyActionOverwrite.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.copyActionOverwrite.Name = "copyActionOverwrite";
            this.copyActionOverwrite.Padding = new System.Windows.Forms.Padding(0, 3, 0, 6);
            this.copyActionOverwrite.Size = new System.Drawing.Size(512, 127);
            this.copyActionOverwrite.TabIndex = 0;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblHeader.Location = new System.Drawing.Point(27, 16);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(312, 18);
            this.lblHeader.TabIndex = 1;
            this.lblHeader.Text = "The file [file] already exists in this location:";
            // 
            // lblDestPath
            // 
            this.lblDestPath.AutoSize = true;
            this.lblDestPath.Location = new System.Drawing.Point(27, 38);
            this.lblDestPath.Name = "lblDestPath";
            this.lblDestPath.Size = new System.Drawing.Size(39, 16);
            this.lblDestPath.TabIndex = 2;
            this.lblDestPath.Text = "C:\\....";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select your choice:";
            // 
            // chkDontAskAgain
            // 
            this.chkDontAskAgain.AutoSize = true;
            this.chkDontAskAgain.Location = new System.Drawing.Point(27, 452);
            this.chkDontAskAgain.Name = "chkDontAskAgain";
            this.chkDontAskAgain.Size = new System.Drawing.Size(194, 20);
            this.chkDontAskAgain.TabIndex = 4;
            this.chkDontAskAgain.Text = "Don\'t ask again (in this activity)";
            this.chkDontAskAgain.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(431, 460);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 31);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 475);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(291, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "You can change the default behavior in the settings";
            // 
            // AskWhatToDoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 508);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.chkDontAskAgain);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblDestPath);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AskWhatToDoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PicPick - Conflict";
            this.Load += new System.EventHandler(this.AskWhatToDoForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblDestPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private UserControls.CopyActionDisplay copyActionSkip;
        private UserControls.CopyActionDisplay copyActionOverwrite;
        private System.Windows.Forms.Panel panel4;
        private UserControls.CopyActionDisplay copyActionKeepBoth;
        private System.Windows.Forms.CheckBox chkDontAskAgain;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
    }
}