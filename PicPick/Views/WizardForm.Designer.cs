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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardForm));
            this.panelControls = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelPanels = new System.Windows.Forms.Panel();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.radioDontDeleteSource = new System.Windows.Forms.RadioButton();
            this.radioDeleteSource = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.panelStart = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.panelDestination = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.pathDestination = new PicPick.UserControls.PathBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.panelPattern = new System.Windows.Forms.Panel();
            this.dtSample = new System.Windows.Forms.DateTimePicker();
            this.ctlPattern = new PicPick.Views.UserControls1.PatternComboBox();
            this.lblPatternPreview = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panelSource = new System.Windows.Forms.Panel();
            this.btnFindFiles = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.pathSource = new PicPick.UserControls.PathBrowser();
            this.panelFinish = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.btnDone = new System.Windows.Forms.Button();
            this.panelControls.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.panelPanels.SuspendLayout();
            this.panelOptions.SuspendLayout();
            this.panelStart.SuspendLayout();
            this.panelDestination.SuspendLayout();
            this.panelPattern.SuspendLayout();
            this.panelSource.SuspendLayout();
            this.panelFinish.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.btnCancel);
            this.panelControls.Controls.Add(this.btnBack);
            this.panelControls.Controls.Add(this.btnDone);
            this.panelControls.Controls.Add(this.btnNext);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControls.Location = new System.Drawing.Point(0, 525);
            this.panelControls.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(1057, 68);
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
            this.btnBack.Location = new System.Drawing.Point(630, 18);
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
            this.btnNext.Location = new System.Drawing.Point(764, 18);
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
            this.panelContent.Size = new System.Drawing.Size(1057, 525);
            this.panelContent.TabIndex = 1;
            // 
            // panelPanels
            // 
            this.panelPanels.Controls.Add(this.panelOptions);
            this.panelPanels.Controls.Add(this.panelStart);
            this.panelPanels.Controls.Add(this.panelDestination);
            this.panelPanels.Controls.Add(this.panelPattern);
            this.panelPanels.Controls.Add(this.panelSource);
            this.panelPanels.Controls.Add(this.panelFinish);
            this.panelPanels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPanels.Location = new System.Drawing.Point(0, 55);
            this.panelPanels.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelPanels.Name = "panelPanels";
            this.panelPanels.Size = new System.Drawing.Size(1057, 470);
            this.panelPanels.TabIndex = 1;
            // 
            // panelOptions
            // 
            this.panelOptions.Controls.Add(this.radioDontDeleteSource);
            this.panelOptions.Controls.Add(this.radioDeleteSource);
            this.panelOptions.Controls.Add(this.label11);
            this.panelOptions.Location = new System.Drawing.Point(35, 244);
            this.panelOptions.Name = "panelOptions";
            this.panelOptions.Padding = new System.Windows.Forms.Padding(20, 30, 20, 30);
            this.panelOptions.Size = new System.Drawing.Size(325, 181);
            this.panelOptions.TabIndex = 4;
            this.panelOptions.Tag = "Additional Options";
            // 
            // radioDontDeleteSource
            // 
            this.radioDontDeleteSource.AutoSize = true;
            this.radioDontDeleteSource.Location = new System.Drawing.Point(24, 143);
            this.radioDontDeleteSource.Name = "radioDontDeleteSource";
            this.radioDontDeleteSource.Size = new System.Drawing.Size(292, 23);
            this.radioDontDeleteSource.TabIndex = 2;
            this.radioDontDeleteSource.TabStop = true;
            this.radioDontDeleteSource.Text = "No, do not delete the files when done.";
            this.radioDontDeleteSource.UseVisualStyleBackColor = true;
            // 
            // radioDeleteSource
            // 
            this.radioDeleteSource.AutoSize = true;
            this.radioDeleteSource.Location = new System.Drawing.Point(24, 114);
            this.radioDeleteSource.Name = "radioDeleteSource";
            this.radioDeleteSource.Size = new System.Drawing.Size(244, 23);
            this.radioDeleteSource.TabIndex = 1;
            this.radioDeleteSource.TabStop = true;
            this.radioDeleteSource.Text = "Yes, delete the files when done.";
            this.radioDeleteSource.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.Dock = System.Windows.Forms.DockStyle.Top;
            this.label11.Location = new System.Drawing.Point(20, 30);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(285, 80);
            this.label11.TabIndex = 0;
            this.label11.Text = "Do you want to delete the original files from their source location once they are" +
    " copied successfully?";
            // 
            // panelStart
            // 
            this.panelStart.Controls.Add(this.label10);
            this.panelStart.Location = new System.Drawing.Point(32, 23);
            this.panelStart.Name = "panelStart";
            this.panelStart.Padding = new System.Windows.Forms.Padding(20, 30, 20, 30);
            this.panelStart.Size = new System.Drawing.Size(201, 171);
            this.panelStart.TabIndex = 3;
            this.panelStart.Tag = "Welcome";
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(20, 30);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(161, 111);
            this.label10.TabIndex = 0;
            this.label10.Text = resources.GetString("label10.Text");
            // 
            // panelDestination
            // 
            this.panelDestination.Controls.Add(this.label8);
            this.panelDestination.Controls.Add(this.pathDestination);
            this.panelDestination.Controls.Add(this.label1);
            this.panelDestination.Location = new System.Drawing.Point(494, 35);
            this.panelDestination.Name = "panelDestination";
            this.panelDestination.Size = new System.Drawing.Size(197, 172);
            this.panelDestination.TabIndex = 1;
            this.panelDestination.Tag = "Destination Base Path";
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
    ", it will be automatically created.\r\n- You can add as many Destinations as you w" +
    "ant in the main editor.";
            // 
            // pathDestination
            // 
            this.pathDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathDestination.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pathDestination.BackColor = System.Drawing.SystemColors.Control;
            this.pathDestination.DialogHeader = "Select Destination Folder";
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
            // panelPattern
            // 
            this.panelPattern.Controls.Add(this.dtSample);
            this.panelPattern.Controls.Add(this.ctlPattern);
            this.panelPattern.Controls.Add(this.lblPatternPreview);
            this.panelPattern.Controls.Add(this.label7);
            this.panelPattern.Controls.Add(this.label6);
            this.panelPattern.Controls.Add(this.label4);
            this.panelPattern.Location = new System.Drawing.Point(697, 15);
            this.panelPattern.Name = "panelPattern";
            this.panelPattern.Size = new System.Drawing.Size(314, 187);
            this.panelPattern.TabIndex = 1;
            this.panelPattern.Tag = "Destination Pattern";
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
            // panelSource
            // 
            this.panelSource.Controls.Add(this.btnFindFiles);
            this.panelSource.Controls.Add(this.label3);
            this.panelSource.Controls.Add(this.label2);
            this.panelSource.Controls.Add(this.lblFileCount);
            this.panelSource.Controls.Add(this.txtFilter);
            this.panelSource.Controls.Add(this.pathSource);
            this.panelSource.Location = new System.Drawing.Point(270, 40);
            this.panelSource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelSource.Name = "panelSource";
            this.panelSource.Size = new System.Drawing.Size(218, 197);
            this.panelSource.TabIndex = 0;
            this.panelSource.Tag = "Source";
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
            this.pathSource.DialogHeader = "Select Source Folder";
            this.pathSource.Location = new System.Drawing.Point(23, 58);
            this.pathSource.Margin = new System.Windows.Forms.Padding(0);
            this.pathSource.Name = "pathSource";
            this.pathSource.ShowExplorerButton = false;
            this.pathSource.Size = new System.Drawing.Size(168, 34);
            this.pathSource.TabIndex = 14;
            // 
            // panelFinish
            // 
            this.panelFinish.Controls.Add(this.label5);
            this.panelFinish.Controls.Add(this.txtName);
            this.panelFinish.Controls.Add(this.label9);
            this.panelFinish.Location = new System.Drawing.Point(707, 237);
            this.panelFinish.Name = "panelFinish";
            this.panelFinish.Padding = new System.Windows.Forms.Padding(20, 30, 20, 30);
            this.panelFinish.Size = new System.Drawing.Size(238, 208);
            this.panelFinish.TabIndex = 1;
            this.panelFinish.Tag = "We\'re done!";
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Location = new System.Drawing.Point(20, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(198, 59);
            this.label5.TabIndex = 0;
            this.label5.Text = "Click Done when finished.";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtName.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(20, 56);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(198, 30);
            this.txtName.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Top;
            this.label9.Location = new System.Drawing.Point(20, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(198, 26);
            this.label9.TabIndex = 2;
            this.label9.Text = "Type a name for your Activity:";
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPageTitle.Font = new System.Drawing.Font("Century Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Padding = new System.Windows.Forms.Padding(26, 0, 0, 0);
            this.lblPageTitle.Size = new System.Drawing.Size(1057, 55);
            this.lblPageTitle.TabIndex = 2;
            this.lblPageTitle.Text = "Title";
            this.lblPageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnDone
            // 
            this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDone.Location = new System.Drawing.Point(926, 18);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(119, 33);
            this.btnDone.TabIndex = 0;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // WizardForm
            // 
            this.AcceptButton = this.btnFindFiles;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1057, 593);
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
            this.panelOptions.ResumeLayout(false);
            this.panelOptions.PerformLayout();
            this.panelStart.ResumeLayout(false);
            this.panelDestination.ResumeLayout(false);
            this.panelDestination.PerformLayout();
            this.panelPattern.ResumeLayout(false);
            this.panelPattern.PerformLayout();
            this.panelSource.ResumeLayout(false);
            this.panelSource.PerformLayout();
            this.panelFinish.ResumeLayout(false);
            this.panelFinish.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.TextBox txtFilter;
        private UserControls.PathBrowser pathSource;
        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.Panel panelPanels;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Panel panelFinish;
        private System.Windows.Forms.Panel panelPattern;
        private System.Windows.Forms.Panel panelDestination;
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
        private System.Windows.Forms.Panel panelStart;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panelOptions;
        private System.Windows.Forms.RadioButton radioDontDeleteSource;
        private System.Windows.Forms.RadioButton radioDeleteSource;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnDone;
    }
}