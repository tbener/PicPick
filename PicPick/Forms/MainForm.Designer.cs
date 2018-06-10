namespace PicPick.Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuProject = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.noProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.taskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.topContainer = new System.Windows.Forms.SplitContainer();
            this.lstTasks = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lblFilter = new System.Windows.Forms.Label();
            this.pnlDestinations = new System.Windows.Forms.Panel();
            this.btnAddDestination = new System.Windows.Forms.Button();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lblTaskName = new System.Windows.Forms.Label();
            this.pathSource = new PicPick.UserControls.PathBrowser();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.mainContainer = new System.Windows.Forms.SplitContainer();
            this.lblProgress = new System.Windows.Forms.Label();
            this.btnStopCopy = new System.Windows.Forms.Button();
            this.progCopy = new System.Windows.Forms.ProgressBar();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topContainer)).BeginInit();
            this.topContainer.Panel1.SuspendLayout();
            this.topContainer.Panel2.SuspendLayout();
            this.topContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).BeginInit();
            this.mainContainer.Panel1.SuspendLayout();
            this.mainContainer.Panel2.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 594);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1009, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuProject,
            this.taskToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1009, 24);
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
            // taskToolStripMenuItem
            // 
            this.taskToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRefresh});
            this.taskToolStripMenuItem.Name = "taskToolStripMenuItem";
            this.taskToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.taskToolStripMenuItem.Text = "&Window";
            // 
            // mnuRefresh
            // 
            this.mnuRefresh.Name = "mnuRefresh";
            this.mnuRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuRefresh.Size = new System.Drawing.Size(132, 22);
            this.mnuRefresh.Text = "&Refresh";
            this.mnuRefresh.Click += new System.EventHandler(this.mnuRefresh_Click);
            // 
            // topContainer
            // 
            this.topContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topContainer.Location = new System.Drawing.Point(0, 0);
            this.topContainer.Name = "topContainer";
            // 
            // topContainer.Panel1
            // 
            this.topContainer.Panel1.Controls.Add(this.lstTasks);
            this.topContainer.Panel1.Controls.Add(this.label1);
            // 
            // topContainer.Panel2
            // 
            this.topContainer.Panel2.Controls.Add(this.button2);
            this.topContainer.Panel2.Controls.Add(this.button1);
            this.topContainer.Panel2.Controls.Add(this.lblFilter);
            this.topContainer.Panel2.Controls.Add(this.pnlDestinations);
            this.topContainer.Panel2.Controls.Add(this.btnAddDestination);
            this.topContainer.Panel2.Controls.Add(this.lblFileCount);
            this.topContainer.Panel2.Controls.Add(this.btnRun);
            this.topContainer.Panel2.Controls.Add(this.txtFilter);
            this.topContainer.Panel2.Controls.Add(this.lblTaskName);
            this.topContainer.Panel2.Controls.Add(this.pathSource);
            this.topContainer.Size = new System.Drawing.Size(1009, 355);
            this.topContainer.SplitterDistance = 219;
            this.topContainer.TabIndex = 2;
            // 
            // lstTasks
            // 
            this.lstTasks.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstTasks.FormattingEnabled = true;
            this.lstTasks.Location = new System.Drawing.Point(12, 31);
            this.lstTasks.Name = "lstTasks";
            this.lstTasks.Size = new System.Drawing.Size(169, 277);
            this.lstTasks.TabIndex = 2;
            this.lstTasks.SelectedIndexChanged += new System.EventHandler(this.lstTasks_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(592, 31);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(103, 31);
            this.button2.TabIndex = 11;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(470, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(103, 31);
            this.button1.TabIndex = 10;
            this.button1.Text = "Read";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(12, 130);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(32, 13);
            this.lblFilter.TabIndex = 9;
            this.lblFilter.Text = "Filter:";
            // 
            // pnlDestinations
            // 
            this.pnlDestinations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDestinations.AutoScroll = true;
            this.pnlDestinations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDestinations.Location = new System.Drawing.Point(12, 201);
            this.pnlDestinations.Name = "pnlDestinations";
            this.pnlDestinations.Size = new System.Drawing.Size(762, 136);
            this.pnlDestinations.TabIndex = 0;
            // 
            // btnAddDestination
            // 
            this.btnAddDestination.Location = new System.Drawing.Point(12, 169);
            this.btnAddDestination.Name = "btnAddDestination";
            this.btnAddDestination.Size = new System.Drawing.Size(152, 23);
            this.btnAddDestination.TabIndex = 0;
            this.btnAddDestination.Text = "Add Destination";
            this.btnAddDestination.UseVisualStyleBackColor = true;
            this.btnAddDestination.Click += new System.EventHandler(this.btnAddDestination_Click);
            // 
            // lblFileCount
            // 
            this.lblFileCount.Location = new System.Drawing.Point(214, 123);
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new System.Drawing.Size(560, 20);
            this.lblFileCount.TabIndex = 6;
            this.lblFileCount.Text = "x files found";
            this.lblFileCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 48);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(88, 24);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(50, 123);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(132, 20);
            this.txtFilter.TabIndex = 3;
            // 
            // lblTaskName
            // 
            this.lblTaskName.AutoSize = true;
            this.lblTaskName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTaskName.Location = new System.Drawing.Point(8, 15);
            this.lblTaskName.Name = "lblTaskName";
            this.lblTaskName.Size = new System.Drawing.Size(43, 20);
            this.lblTaskName.TabIndex = 2;
            this.lblTaskName.Text = "Task";
            // 
            // pathSource
            // 
            this.pathSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathSource.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pathSource.Location = new System.Drawing.Point(12, 89);
            this.pathSource.Margin = new System.Windows.Forms.Padding(0);
            this.pathSource.Name = "pathSource";
            this.pathSource.Size = new System.Drawing.Size(762, 27);
            this.pathSource.TabIndex = 1;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.Black;
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rtbLog.Location = new System.Drawing.Point(12, 60);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(985, 126);
            this.rtbLog.TabIndex = 8;
            this.rtbLog.Text = "Text text text";
            this.rtbLog.WordWrap = false;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1009, 25);
            this.toolStrip.TabIndex = 3;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Refresh";
            // 
            // mainContainer
            // 
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 49);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainContainer.Panel1
            // 
            this.mainContainer.Panel1.Controls.Add(this.topContainer);
            // 
            // mainContainer.Panel2
            // 
            this.mainContainer.Panel2.Controls.Add(this.lblProgress);
            this.mainContainer.Panel2.Controls.Add(this.btnStopCopy);
            this.mainContainer.Panel2.Controls.Add(this.progCopy);
            this.mainContainer.Panel2.Controls.Add(this.rtbLog);
            this.mainContainer.Size = new System.Drawing.Size(1009, 545);
            this.mainContainer.SplitterDistance = 355;
            this.mainContainer.TabIndex = 4;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.BackColor = System.Drawing.SystemColors.Control;
            this.lblProgress.Location = new System.Drawing.Point(9, 11);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 11;
            // 
            // btnStopCopy
            // 
            this.btnStopCopy.Location = new System.Drawing.Point(909, 27);
            this.btnStopCopy.Name = "btnStopCopy";
            this.btnStopCopy.Size = new System.Drawing.Size(88, 24);
            this.btnStopCopy.TabIndex = 10;
            this.btnStopCopy.Text = "Stop";
            this.btnStopCopy.UseVisualStyleBackColor = true;
            // 
            // progCopy
            // 
            this.progCopy.Location = new System.Drawing.Point(12, 27);
            this.progCopy.Name = "progCopy";
            this.progCopy.Size = new System.Drawing.Size(883, 24);
            this.progCopy.Step = 1;
            this.progCopy.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progCopy.TabIndex = 9;
            this.progCopy.Value = 30;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 616);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.topContainer.Panel1.ResumeLayout(false);
            this.topContainer.Panel1.PerformLayout();
            this.topContainer.Panel2.ResumeLayout(false);
            this.topContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topContainer)).EndInit();
            this.topContainer.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.mainContainer.Panel1.ResumeLayout(false);
            this.mainContainer.Panel2.ResumeLayout(false);
            this.mainContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).EndInit();
            this.mainContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.SplitContainer topContainer;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuProject;
        private System.Windows.Forms.ToolStripMenuItem mnuOpen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem noProjectsToolStripMenuItem;
        private UserControls.PathBrowser pathSource;
        private System.Windows.Forms.Label lblTaskName;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.CheckedListBox lstTasks;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.Button btnAddDestination;
        private System.Windows.Forms.Panel pnlDestinations;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.SplitContainer mainContainer;
        private System.Windows.Forms.ToolStripMenuItem taskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuRefresh;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnStopCopy;
        private System.Windows.Forms.ProgressBar progCopy;
        private System.Windows.Forms.Label lblProgress;
    }
}