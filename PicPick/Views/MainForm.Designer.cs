namespace PicPick.Views
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuProject = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.noProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAutoSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTask = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTaskRename = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTaskRun = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTaskDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTaskMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTaskMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.topContainer = new System.Windows.Forms.SplitContainer();
            this.lstTasks = new System.Windows.Forms.ListBox();
            this.contextMenuTask = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.chkDeleteSourceFiles = new System.Windows.Forms.CheckBox();
            this.txtTaskName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlDestinations = new System.Windows.Forms.Panel();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.btnAddDestination = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.pathSource = new PicPick.UserControls.PathBrowser();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.mainContainer = new System.Windows.Forms.SplitContainer();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progCopy = new System.Windows.Forms.ProgressBar();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.mnuTaskAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topContainer)).BeginInit();
            this.topContainer.Panel1.SuspendLayout();
            this.topContainer.Panel2.SuspendLayout();
            this.topContainer.SuspendLayout();
            this.contextMenuTask.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).BeginInit();
            this.mainContainer.Panel1.SuspendLayout();
            this.mainContainer.Panel2.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 736);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip.Size = new System.Drawing.Size(1126, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(1109, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuProject,
            this.mnuTask,
            this.mnuWindow,
            this.toolStripMenuItem1});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1126, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // mnuProject
            // 
            this.mnuProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpen,
            this.mnuSave,
            this.mnuAutoSave});
            this.mnuProject.Name = "mnuProject";
            this.mnuProject.Size = new System.Drawing.Size(56, 20);
            this.mnuProject.Text = "&Project";
            // 
            // mnuOpen
            // 
            this.mnuOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noProjectsToolStripMenuItem});
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.Size = new System.Drawing.Size(152, 22);
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
            this.mnuSave.Size = new System.Drawing.Size(152, 22);
            this.mnuSave.Text = "&Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuAutoSave
            // 
            this.mnuAutoSave.CheckOnClick = true;
            this.mnuAutoSave.Name = "mnuAutoSave";
            this.mnuAutoSave.Size = new System.Drawing.Size(152, 22);
            this.mnuAutoSave.Text = "&Auto save";
            this.mnuAutoSave.ToolTipText = "Save your changes automatically";
            this.mnuAutoSave.Click += new System.EventHandler(this.mnuAutoSave_Click);
            // 
            // mnuTask
            // 
            this.mnuTask.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTaskAdd,
            this.toolStripSeparator3,
            this.mnuTaskRename,
            this.mnuTaskRun,
            this.toolStripSeparator1,
            this.mnuTaskDelete,
            this.toolStripSeparator2,
            this.mnuTaskMoveUp,
            this.mnuTaskMoveDown});
            this.mnuTask.Name = "mnuTask";
            this.mnuTask.Size = new System.Drawing.Size(59, 20);
            this.mnuTask.Text = "&Activity";
            // 
            // mnuTaskRename
            // 
            this.mnuTaskRename.Name = "mnuTaskRename";
            this.mnuTaskRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mnuTaskRename.Size = new System.Drawing.Size(152, 22);
            this.mnuTaskRename.Text = "Re&name...";
            this.mnuTaskRename.Click += new System.EventHandler(this.mnuTaskRename_Click);
            // 
            // mnuTaskRun
            // 
            this.mnuTaskRun.Name = "mnuTaskRun";
            this.mnuTaskRun.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuTaskRun.Size = new System.Drawing.Size(152, 22);
            this.mnuTaskRun.Text = "&Run";
            this.mnuTaskRun.Click += new System.EventHandler(this.mnuTaskRun_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // mnuTaskDelete
            // 
            this.mnuTaskDelete.Name = "mnuTaskDelete";
            this.mnuTaskDelete.Size = new System.Drawing.Size(152, 22);
            this.mnuTaskDelete.Text = "&Delete...";
            this.mnuTaskDelete.Click += new System.EventHandler(this.mnuTaskDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // mnuTaskMoveUp
            // 
            this.mnuTaskMoveUp.Name = "mnuTaskMoveUp";
            this.mnuTaskMoveUp.Size = new System.Drawing.Size(152, 22);
            this.mnuTaskMoveUp.Text = "Move &up";
            this.mnuTaskMoveUp.Click += new System.EventHandler(this.mnuTaskMoveUp_Click);
            // 
            // mnuTaskMoveDown
            // 
            this.mnuTaskMoveDown.Name = "mnuTaskMoveDown";
            this.mnuTaskMoveDown.Size = new System.Drawing.Size(152, 22);
            this.mnuTaskMoveDown.Text = "Move &down";
            this.mnuTaskMoveDown.Click += new System.EventHandler(this.mnuTaskMoveDown_Click);
            // 
            // mnuWindow
            // 
            this.mnuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRefresh});
            this.mnuWindow.Name = "mnuWindow";
            this.mnuWindow.Size = new System.Drawing.Size(63, 20);
            this.mnuWindow.Text = "&Window";
            // 
            // mnuRefresh
            // 
            this.mnuRefresh.Name = "mnuRefresh";
            this.mnuRefresh.Size = new System.Drawing.Size(152, 22);
            this.mnuRefresh.Text = "&Refresh";
            this.mnuRefresh.Click += new System.EventHandler(this.mnuRefresh_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(12, 20);
            // 
            // topContainer
            // 
            this.topContainer.BackColor = System.Drawing.SystemColors.Control;
            this.topContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topContainer.Location = new System.Drawing.Point(0, 0);
            this.topContainer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.topContainer.Name = "topContainer";
            // 
            // topContainer.Panel1
            // 
            this.topContainer.Panel1.Controls.Add(this.lstTasks);
            this.topContainer.Panel1.Controls.Add(this.label1);
            // 
            // topContainer.Panel2
            // 
            this.topContainer.Panel2.Controls.Add(this.chkDeleteSourceFiles);
            this.topContainer.Panel2.Controls.Add(this.txtTaskName);
            this.topContainer.Panel2.Controls.Add(this.label4);
            this.topContainer.Panel2.Controls.Add(this.label3);
            this.topContainer.Panel2.Controls.Add(this.label2);
            this.topContainer.Panel2.Controls.Add(this.pnlDestinations);
            this.topContainer.Panel2.Controls.Add(this.lblFileCount);
            this.topContainer.Panel2.Controls.Add(this.btnAddDestination);
            this.topContainer.Panel2.Controls.Add(this.btnRun);
            this.topContainer.Panel2.Controls.Add(this.txtFilter);
            this.topContainer.Panel2.Controls.Add(this.pathSource);
            this.topContainer.Size = new System.Drawing.Size(1126, 687);
            this.topContainer.SplitterDistance = 243;
            this.topContainer.SplitterWidth = 5;
            this.topContainer.TabIndex = 2;
            // 
            // lstTasks
            // 
            this.lstTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTasks.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lstTasks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstTasks.ContextMenuStrip = this.contextMenuTask;
            this.lstTasks.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstTasks.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lstTasks.FormattingEnabled = true;
            this.lstTasks.ItemHeight = 15;
            this.lstTasks.Location = new System.Drawing.Point(14, 38);
            this.lstTasks.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstTasks.Name = "lstTasks";
            this.lstTasks.Size = new System.Drawing.Size(226, 602);
            this.lstTasks.TabIndex = 2;
            this.lstTasks.SelectedIndexChanged += new System.EventHandler(this.lstTasks_SelectedIndexChanged);
            // 
            // contextMenuTask
            // 
            this.contextMenuTask.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
            this.contextMenuTask.Name = "contextMenuTask";
            this.contextMenuTask.Size = new System.Drawing.Size(181, 48);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem2.Text = "toolStripMenuItem2";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem3.Text = "toolStripMenuItem3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Activities";
            // 
            // chkDeleteSourceFiles
            // 
            this.chkDeleteSourceFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkDeleteSourceFiles.AutoSize = true;
            this.chkDeleteSourceFiles.Location = new System.Drawing.Point(14, 649);
            this.chkDeleteSourceFiles.Name = "chkDeleteSourceFiles";
            this.chkDeleteSourceFiles.Size = new System.Drawing.Size(121, 20);
            this.chkDeleteSourceFiles.TabIndex = 16;
            this.chkDeleteSourceFiles.Text = "Delete source files";
            this.toolTips.SetToolTip(this.chkDeleteSourceFiles, "Delete files that were successfully processed from their original location");
            this.chkDeleteSourceFiles.UseVisualStyleBackColor = true;
            this.chkDeleteSourceFiles.CheckedChanged += new System.EventHandler(this.chkDeleteSourceFiles_CheckedChanged);
            // 
            // txtTaskName
            // 
            this.txtTaskName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTaskName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTaskName.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTaskName.Location = new System.Drawing.Point(13, 9);
            this.txtTaskName.Name = "txtTaskName";
            this.txtTaskName.ReadOnly = true;
            this.txtTaskName.Size = new System.Drawing.Size(836, 27);
            this.txtTaskName.TabIndex = 15;
            this.txtTaskName.DoubleClick += new System.EventHandler(this.txtTaskName_DoubleClick);
            this.txtTaskName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTaskName_KeyPress);
            this.txtTaskName.Leave += new System.EventHandler(this.txtTaskName_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 16);
            this.label4.TabIndex = 14;
            this.label4.Text = "Destinations";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(629, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Filter:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Source path:";
            // 
            // pnlDestinations
            // 
            this.pnlDestinations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDestinations.AutoScroll = true;
            this.pnlDestinations.Location = new System.Drawing.Point(14, 176);
            this.pnlDestinations.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlDestinations.Name = "pnlDestinations";
            this.pnlDestinations.Size = new System.Drawing.Size(835, 440);
            this.pnlDestinations.TabIndex = 0;
            // 
            // lblFileCount
            // 
            this.lblFileCount.AutoSize = true;
            this.lblFileCount.Location = new System.Drawing.Point(16, 102);
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new System.Drawing.Size(70, 16);
            this.lblFileCount.TabIndex = 6;
            this.lblFileCount.Text = "x files found";
            // 
            // btnAddDestination
            // 
            this.btnAddDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddDestination.Location = new System.Drawing.Point(672, 140);
            this.btnAddDestination.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddDestination.Name = "btnAddDestination";
            this.btnAddDestination.Size = new System.Drawing.Size(177, 28);
            this.btnAddDestination.TabIndex = 0;
            this.btnAddDestination.Text = "Add Destination";
            this.btnAddDestination.UseVisualStyleBackColor = true;
            this.btnAddDestination.Click += new System.EventHandler(this.btnAddDestination_Click);
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Location = new System.Drawing.Point(746, 643);
            this.btnRun.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(103, 30);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilter.Location = new System.Drawing.Point(632, 69);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(217, 21);
            this.txtFilter.TabIndex = 3;
            // 
            // pathSource
            // 
            this.pathSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathSource.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pathSource.BackColor = System.Drawing.SystemColors.Control;
            this.pathSource.Location = new System.Drawing.Point(13, 69);
            this.pathSource.Margin = new System.Windows.Forms.Padding(0);
            this.pathSource.Name = "pathSource";
            this.pathSource.Size = new System.Drawing.Size(605, 26);
            this.pathSource.TabIndex = 1;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.Black;
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rtbLog.Location = new System.Drawing.Point(14, 74);
            this.rtbLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(1148, 154);
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
            this.toolStrip.Size = new System.Drawing.Size(1126, 25);
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
            this.mainContainer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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
            this.mainContainer.Panel2.Controls.Add(this.progCopy);
            this.mainContainer.Panel2.Controls.Add(this.rtbLog);
            this.mainContainer.Panel2Collapsed = true;
            this.mainContainer.Size = new System.Drawing.Size(1126, 687);
            this.mainContainer.SplitterDistance = 447;
            this.mainContainer.SplitterWidth = 5;
            this.mainContainer.TabIndex = 4;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.BackColor = System.Drawing.SystemColors.Control;
            this.lblProgress.Location = new System.Drawing.Point(10, 14);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 16);
            this.lblProgress.TabIndex = 11;
            // 
            // progCopy
            // 
            this.progCopy.Location = new System.Drawing.Point(14, 33);
            this.progCopy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progCopy.Name = "progCopy";
            this.progCopy.Size = new System.Drawing.Size(1149, 30);
            this.progCopy.Step = 1;
            this.progCopy.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progCopy.TabIndex = 9;
            this.progCopy.Value = 30;
            // 
            // mnuTaskAdd
            // 
            this.mnuTaskAdd.Name = "mnuTaskAdd";
            this.mnuTaskAdd.Size = new System.Drawing.Size(152, 22);
            this.mnuTaskAdd.Text = "Add new";
            this.mnuTaskAdd.Click += new System.EventHandler(this.mnuTaskAdd_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1126, 758);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.topContainer.Panel1.ResumeLayout(false);
            this.topContainer.Panel1.PerformLayout();
            this.topContainer.Panel2.ResumeLayout(false);
            this.topContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topContainer)).EndInit();
            this.topContainer.ResumeLayout(false);
            this.contextMenuTask.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.Button btnAddDestination;
        private System.Windows.Forms.Panel pnlDestinations;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.SplitContainer mainContainer;
        private System.Windows.Forms.ToolStripMenuItem mnuWindow;
        private System.Windows.Forms.ToolStripMenuItem mnuRefresh;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ProgressBar progCopy;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ListBox lstTasks;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuTask;
        private System.Windows.Forms.ToolStripMenuItem mnuTaskRename;
        private System.Windows.Forms.ToolStripMenuItem mnuTaskRun;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuTaskDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuTaskMoveUp;
        private System.Windows.Forms.ToolStripMenuItem mnuTaskMoveDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTaskName;
        private System.Windows.Forms.CheckBox chkDeleteSourceFiles;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.ContextMenuStrip contextMenuTask;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mnuAutoSave;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem mnuTaskAdd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}