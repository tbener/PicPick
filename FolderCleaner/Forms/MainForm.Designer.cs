namespace FolderCleaner.Forms
{
    partial class MainForm
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuProject = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.noProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.lstTasks = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCheck = new System.Windows.Forms.Button();
            this.pnlDestinations = new System.Windows.Forms.GroupBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lblTaskName = new System.Windows.Forms.Label();
            this.pathSource = new FolderCleaner.UserControls.PathBrowser();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 531);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(978, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuProject});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(978, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // mnuProject
            // 
            this.mnuProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpen,
            this.mnuSave});
            this.mnuProject.Name = "mnuProject";
            this.mnuProject.Size = new System.Drawing.Size(56, 20);
            this.mnuProject.Text = "&Project";
            // 
            // mnuOpen
            // 
            this.mnuOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noProjectsToolStripMenuItem});
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.Size = new System.Drawing.Size(138, 22);
            this.mnuOpen.Text = "&Open";
            this.mnuOpen.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MnuOpen_DropDownItemClicked);
            // 
            // noProjectsToolStripMenuItem
            // 
            this.noProjectsToolStripMenuItem.Name = "noProjectsToolStripMenuItem";
            this.noProjectsToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.noProjectsToolStripMenuItem.Text = "(No projects)";
            // 
            // mnuSave
            // 
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuSave.Size = new System.Drawing.Size(138, 22);
            this.mnuSave.Text = "&Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 24);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.lstTasks);
            this.splitContainerMain.Panel1.Controls.Add(this.label1);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.lblFileCount);
            this.splitContainerMain.Panel2.Controls.Add(this.btnCheck);
            this.splitContainerMain.Panel2.Controls.Add(this.pnlDestinations);
            this.splitContainerMain.Panel2.Controls.Add(this.txtFilter);
            this.splitContainerMain.Panel2.Controls.Add(this.lblTaskName);
            this.splitContainerMain.Panel2.Controls.Add(this.pathSource);
            this.splitContainerMain.Size = new System.Drawing.Size(978, 507);
            this.splitContainerMain.SplitterDistance = 325;
            this.splitContainerMain.TabIndex = 2;
            // 
            // lstTasks
            // 
            this.lstTasks.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstTasks.FormattingEnabled = true;
            this.lstTasks.Location = new System.Drawing.Point(29, 100);
            this.lstTasks.Name = "lstTasks";
            this.lstTasks.Size = new System.Drawing.Size(254, 340);
            this.lstTasks.TabIndex = 2;
            this.lstTasks.SelectedIndexChanged += new System.EventHandler(this.lstTasks_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(12, 80);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(88, 24);
            this.btnCheck.TabIndex = 5;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // pnlDestinations
            // 
            this.pnlDestinations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDestinations.Location = new System.Drawing.Point(25, 220);
            this.pnlDestinations.Name = "pnlDestinations";
            this.pnlDestinations.Size = new System.Drawing.Size(595, 274);
            this.pnlDestinations.TabIndex = 4;
            this.pnlDestinations.TabStop = false;
            this.pnlDestinations.Text = "Destinations";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(357, 50);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(132, 20);
            this.txtFilter.TabIndex = 3;
            // 
            // lblTaskName
            // 
            this.lblTaskName.AutoSize = true;
            this.lblTaskName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTaskName.Location = new System.Drawing.Point(8, 25);
            this.lblTaskName.Name = "lblTaskName";
            this.lblTaskName.Size = new System.Drawing.Size(43, 20);
            this.lblTaskName.TabIndex = 2;
            this.lblTaskName.Text = "Task";
            // 
            // pathSource
            // 
            this.pathSource.Location = new System.Drawing.Point(12, 37);
            this.pathSource.Name = "pathSource";
            this.pathSource.Size = new System.Drawing.Size(303, 46);
            this.pathSource.TabIndex = 1;
            // 
            // toolStrip
            // 
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(978, 25);
            this.toolStrip.TabIndex = 3;
            this.toolStrip.Text = "toolStrip1";
            // 
            // lblFileCount
            // 
            this.lblFileCount.AutoSize = true;
            this.lblFileCount.Location = new System.Drawing.Point(117, 86);
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new System.Drawing.Size(10, 13);
            this.lblFileCount.TabIndex = 6;
            this.lblFileCount.Text = "-";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 553);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel1.PerformLayout();
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuProject;
        private System.Windows.Forms.ToolStripMenuItem mnuOpen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem noProjectsToolStripMenuItem;
        private UserControls.PathBrowser pathSource;
        private System.Windows.Forms.Label lblTaskName;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.GroupBox pnlDestinations;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.CheckedListBox lstTasks;
        private System.Windows.Forms.Label lblFileCount;
    }
}