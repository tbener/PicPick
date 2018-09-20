namespace PicPick.Views
{
    partial class WizardForm
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
            this.panelControls = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelPanels = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dtSample = new System.Windows.Forms.DateTimePicker();
            this.ctlPattern = new PicPick.Views.UserControls1.PatternComboBox();
            this.lblPatternPreview = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pathDestination = new PicPick.UserControls.PathBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.panel0 = new System.Windows.Forms.Panel();
            this.btnFindFiles = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.pathSource = new PicPick.UserControls.PathBrowser();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panelControls.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.panelPanels.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel0.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.btnCancel);
            this.panelControls.Controls.Add(this.btnBack);
            this.panelControls.Controls.Add(this.btnNext);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControls.Location = new System.Drawing.Point(0, 393);
            this.panelControls.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(734, 68);
            this.panelControls.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(30, 18);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(119, 33);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Location = new System.Drawing.Point(455, 18);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(119, 33);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "< Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(589, 18);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(119, 33);
            this.btnNext.TabIndex = 0;
            this.btnNext.Text = "Next >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.panelPanels);
            this.panelContent.Controls.Add(this.lblPageTitle);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(734, 393);
            this.panelContent.TabIndex = 1;
            // 
            // panelPanels
            // 
            this.panelPanels.Controls.Add(this.panel1);
            this.panelPanels.Controls.Add(this.panel2);
            this.panelPanels.Controls.Add(this.panel0);
            this.panelPanels.Controls.Add(this.panel3);
            this.panelPanels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPanels.Location = new System.Drawing.Point(0, 55);
            this.panelPanels.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelPanels.Name = "panelPanels";
            this.panelPanels.Size = new System.Drawing.Size(734, 338);
            this.panelPanels.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dtSample);
            this.panel2.Controls.Add(this.ctlPattern);
            this.panel2.Controls.Add(this.lblPatternPreview);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(473, 90);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 187);
            this.panel2.TabIndex = 1;
            // 
            // dtSample
            // 
            this.dtSample.Location = new System.Drawing.Point(23, 140);
            this.dtSample.Name = "dtSample";
            this.dtSample.Size = new System.Drawing.Size(200, 24);
            this.dtSample.TabIndex = 10;
            this.dtSample.ValueChanged += new System.EventHandler(this.dtSample_ValueChanged);
            // 
            // ctlPattern
            // 
            this.ctlPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctlPattern.FormattingEnabled = true;
            this.ctlPattern.Items.AddRange(new object[] {
            "YYYY\\MM",
            "asdf"});
            this.ctlPattern.Location = new System.Drawing.Point(23, 58);
            this.ctlPattern.Name = "ctlPattern";
            this.ctlPattern.Size = new System.Drawing.Size(206, 25);
            this.ctlPattern.TabIndex = 9;
            // 
            // lblPatternPreview
            // 
            this.lblPatternPreview.AutoSize = true;
            this.lblPatternPreview.Location = new System.Drawing.Point(20, 229);
            this.lblPatternPreview.Name = "lblPatternPreview";
            this.lblPatternPreview.Size = new System.Drawing.Size(159, 19);
            this.lblPatternPreview.TabIndex = 7;
            this.lblPatternPreview.Text = "[Destination folder]\\...";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 200);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(166, 19);
            this.label7.TabIndex = 7;
            this.label7.Text = "The result folder will be:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 110);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(147, 19);
            this.label6.TabIndex = 7;
            this.label6.Text = "Select sample date:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "Pattern:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.pathDestination);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(259, 15);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(197, 172);
            this.panel1.TabIndex = 1;
            // 
            // pathDestination
            // 
            this.pathDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathDestination.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pathDestination.BackColor = System.Drawing.SystemColors.Control;
            this.pathDestination.Location = new System.Drawing.Point(23, 58);
            this.pathDestination.Margin = new System.Windows.Forms.Padding(0);
            this.pathDestination.Name = "pathDestination";
            this.pathDestination.ShowExplorerButton = false;
            this.pathDestination.Size = new System.Drawing.Size(149, 34);
            this.pathDestination.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select destination base path";
            // 
            // panel0
            // 
            this.panel0.Controls.Add(this.btnFindFiles);
            this.panel0.Controls.Add(this.label3);
            this.panel0.Controls.Add(this.label2);
            this.panel0.Controls.Add(this.lblFileCount);
            this.panel0.Controls.Add(this.txtFilter);
            this.panel0.Controls.Add(this.pathSource);
            this.panel0.Location = new System.Drawing.Point(12, 5);
            this.panel0.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel0.Name = "panel0";
            this.panel0.Size = new System.Drawing.Size(218, 197);
            this.panel0.TabIndex = 0;
            // 
            // btnFindFiles
            // 
            this.btnFindFiles.Location = new System.Drawing.Point(23, 225);
            this.btnFindFiles.Name = "btnFindFiles";
            this.btnFindFiles.Size = new System.Drawing.Size(127, 33);
            this.btnFindFiles.TabIndex = 18;
            this.btnFindFiles.Text = "Check";
            this.btnFindFiles.UseVisualStyleBackColor = true;
            this.btnFindFiles.Click += new System.EventHandler(this.btnFindFiles_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.label3.Location = new System.Drawing.Point(20, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 19);
            this.label3.TabIndex = 17;
            this.label3.Text = "Select filter(s):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.label2.Location = new System.Drawing.Point(20, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 19);
            this.label2.TabIndex = 17;
            this.label2.Text = "Select source path:";
            // 
            // lblFileCount
            // 
            this.lblFileCount.AutoSize = true;
            this.lblFileCount.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileCount.Location = new System.Drawing.Point(19, 265);
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new System.Drawing.Size(99, 21);
            this.lblFileCount.TabIndex = 16;
            this.lblFileCount.Text = "x files found";
            // 
            // txtFilter
            // 
            this.txtFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilter.Location = new System.Drawing.Point(23, 158);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(292, 24);
            this.txtFilter.TabIndex = 15;
            // 
            // pathSource
            // 
            this.pathSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathSource.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pathSource.BackColor = System.Drawing.SystemColors.Control;
            this.pathSource.Location = new System.Drawing.Point(23, 58);
            this.pathSource.Margin = new System.Windows.Forms.Padding(0);
            this.pathSource.Name = "pathSource";
            this.pathSource.ShowExplorerButton = false;
            this.pathSource.Size = new System.Drawing.Size(168, 34);
            this.pathSource.TabIndex = 14;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label5);
            this.panel3.Location = new System.Drawing.Point(72, 251);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(384, 179);
            this.panel3.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(35, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 19);
            this.label5.TabIndex = 0;
            this.label5.Text = "Finished.";
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPageTitle.Font = new System.Drawing.Font("Century Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Padding = new System.Windows.Forms.Padding(26, 0, 0, 0);
            this.lblPageTitle.Size = new System.Drawing.Size(734, 55);
            this.lblPageTitle.TabIndex = 2;
            this.lblPageTitle.Text = "Title";
            this.lblPageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Location = new System.Drawing.Point(20, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(151, 167);
            this.label8.TabIndex = 16;
            this.label8.Text = "- Leave empty to use the same path as the Source.\r\n- If the folder does not exist" +
    ", it will be automatically created.\r\n- You can as many Destinations as you want " +
    "in the main editor.";
            // 
            // WizardForm
            // 
            this.AcceptButton = this.btnFindFiles;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(734, 461);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelControls);
            this.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WizardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WizardForm";
            this.Load += new System.EventHandler(this.WizardForm_Load);
            this.panelControls.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.panelPanels.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel0.ResumeLayout(false);
            this.panel0.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panel0;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.TextBox txtFilter;
        private UserControls.PathBrowser pathSource;
        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.Panel panelPanels;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private UserControls.PathBrowser pathDestination;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnFindFiles;
        private UserControls1.PatternComboBox ctlPattern;
        private System.Windows.Forms.DateTimePicker dtSample;
        private System.Windows.Forms.Label lblPatternPreview;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
    }
}