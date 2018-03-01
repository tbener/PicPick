namespace FolderCleaner.UserControls
{
    partial class TemplatePath
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
            this.pathControl = new FolderCleaner.UserControls.PathBrowser();
            this.txtTemplate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPreview = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pathControl
            // 
            this.pathControl.Location = new System.Drawing.Point(3, 0);
            this.pathControl.Name = "pathControl";
            this.pathControl.Size = new System.Drawing.Size(330, 46);
            this.pathControl.TabIndex = 0;
            // 
            // txtTemplate
            // 
            this.txtTemplate.Location = new System.Drawing.Point(339, 13);
            this.txtTemplate.Name = "txtTemplate";
            this.txtTemplate.Size = new System.Drawing.Size(163, 20);
            this.txtTemplate.TabIndex = 1;
            this.txtTemplate.Text = "yyyy-MM";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Preview:";
            // 
            // lblPreview
            // 
            this.lblPreview.AutoEllipsis = true;
            this.lblPreview.Location = new System.Drawing.Point(58, 49);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(444, 23);
            this.lblPreview.TabIndex = 3;
            this.lblPreview.Text = "-";
            // 
            // TemplatePath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTemplate);
            this.Controls.Add(this.pathControl);
            this.Name = "TemplatePath";
            this.Size = new System.Drawing.Size(527, 87);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PathBrowser pathControl;
        private System.Windows.Forms.TextBox txtTemplate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPreview;
    }
}
