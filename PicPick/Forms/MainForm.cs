using log4net.Appender;
using log4net.Core;
using PicPick.Configuration;
using PicPick.Helpers;
using PicPick.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalUtils;

namespace PicPick.Forms
{
    public partial class MainForm : Form, IAppender
    {
        PicPickConfigTask _currentTask;
        bool _isDirty;
        bool _isLoading;
        PreviewForm _taskForm;

        Dictionary<Level, Color> logColors = new Dictionary<Level, Color>()
        {
            { Level.Error, Color.Red },
            { Level.Info, Color.Green },
            { Level.Warn, Color.Yellow }
        };

        public MainForm()
        {
            InitializeComponent();
            rtbLog.Clear();

            pathSource.Changed += (s, e) => SetDirty(s);
            txtFilter.TextChanged += (s, e) => SetDirty(s);
            lstTasks.ItemCheck += (s, e) => SetDirty(s);

            _taskForm = new PreviewForm();

            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(this);
        }

        

        private void MainForm_Load(object sender, EventArgs e)
        {
            

            LoadFile();
            SetDirty(sender, e, false);
        }

        public override void Refresh()
        {
            base.Refresh();

            ReadSource();
        }


        private void SetDirty(object sender, EventArgs e = null,  bool isDirty = true)
        {
            if (_isLoading) return;

            this.Text = AppInfo.AppName + (isDirty ? " *" : "");
            _isDirty = isDirty;

            if (sender == pathSource) _currentTask.Source.Path = pathSource.Text;
            if (sender == txtFilter) _currentTask.Source.Filter = txtFilter.Text;
            if (sender == pathSource || sender == txtFilter) ReadSource();

                if (isDirty) _currentTask?.SetDirty();
        }

        private void ReadSource()
        {
            try
            {
                if (PathHelper.Exists(_currentTask.Source.Path))
                {
                    lblFileCount.Text = "Reading...";
                    _currentTask.ReadFiles();
                    lblFileCount.Text = $"{_currentTask.FileCount} files found";
                    btnCheck.Enabled = _currentTask.FileCount > 0;
                }
                else
                {
                    lblFileCount.Text = "";
                    btnCheck.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lblFileCount.Text = ex.Message;
                btnCheck.Enabled = false;
            }
        }

        private void SetDirty(bool isDirty)
        {
            SetDirty(null, null, isDirty);
        }
        private void SetDirty()
        {
            SetDirty(true);
        }

        private void LoadFile()
        {
            try
            {
                _isLoading = true;
                mnuOpen.DropDownItems.Clear();
                var projList = ConfigurationHelper.Default.Projects.Project.Select(p => new ToolStripMenuItem(p.Name) { Tag = p }).ToArray();
                if (projList.Length > 0)
                {
                    mnuOpen.DropDownItems.AddRange(projList);
                }
                else
                {
                    mnuOpen.DropDownItems.Add("(No projects)");
                    mnuOpen.DropDownItems[0].Enabled = false;
                }

                lstTasks.Items.AddRange(
                    ConfigurationHelper.Default.Tasks
                    );

                mnuOpen.DropDownItems[0].PerformClick();    // this will invoke LoadProject()
                lstTasks.SetSelected(0, true);              // this will invoke LoadTask()
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error while loading the file.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void LoadProject(PicPickConfigProjectsProject proj)
        {
            try
            {
                _isLoading = true;
                var taskList = proj.TaskRef.Select(t => t.Name).ToArray();
                for (int i = 0; i < lstTasks.Items.Count; i++)
                {
                    lstTasks.SetItemChecked(i, taskList.Contains(lstTasks.Items[i].ToString()));
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error while loading project.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void LoadTask(PicPickConfigTask task)
        {
            try
            {
                _isLoading = true;
                _currentTask = task;

                // clear fields
                lblTaskName.Text = "";
                pathSource.Text = "";
                txtFilter.Text = "";

                foreach (var ctl in pnlDestinations.Controls.Cast<Control>().ToArray())
                {
                    ctl.Dispose();
                }

                if (_currentTask == null) return;

                lblTaskName.Text = _currentTask.Name;
                pathSource.Text = _currentTask.Source.Path;
                txtFilter.Text = _currentTask.Source.Filter;
                ReadSource();

                _currentTask.DestinationList.ForEach(AddDestinationControl);
                
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error while loading task.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void AddDestinationControl(PicPickConfigTaskDestination dest)
        {
            TemplatePath dstPath = new TemplatePath(dest);
            dstPath.Changed += (s, e) => SetDirty();

            Panel pnl = new Panel()
            {
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnDel = new Button()
            {
                Text = "X",
                Size = new Size(25, 25),
                Tag = dest
            };
            btnDel.Click += BtnDel_Click;

            pnl.Controls.Add(dstPath);
            pnl.Controls.Add(btnDel);

            btnDel.Location = new Point(dstPath.Width, 0);

            pnl.Dock = DockStyle.Top;
            pnlDestinations.Controls.Add(pnl);
        }

        private void BtnDel_Click(object sender, EventArgs e)
        {
            // get the actual button
            Button btn = (Button)sender;
            // the Destination object is it's Tag
            PicPickConfigTaskDestination dest = (PicPickConfigTaskDestination)btn.Tag;
            // remove this destination from the list
            _currentTask.DestinationList.Remove(dest);
            // remove the panel from the UI
            btn.Parent.Dispose();

            SetDirty();
        }

        private void btnAddDestination_Click(object sender, EventArgs e)
        {
            PicPickConfigTaskDestination dest = new PicPickConfigTaskDestination();
            _currentTask.DestinationList.Add(dest);
            AddDestinationControl(dest);
        }

        private void StartTask(PicPickConfigTask task)
        {
            //_taskForm.Start(task);
            rtbLog.Clear();
            try
            {
                LogHandler.Log($"Starting Task: {task.Name}");
                task.Execute();
                LogHandler.Log("Finished");
            }
            catch (Exception ex)
            {
                LogHandler.Log("Finished with errors:", Level.Error);
                LogHandler.Log(ex.Message, Level.Error);
            }
            ReadSource();
        }

        private void MnuOpen_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            PicPickConfigProjectsProject p = e.ClickedItem.Tag as PicPickConfigProjectsProject;
            if (p != null)
                LoadProject(p);
        }

        // Todo: avoid reload when clicking on the selected task
        private void lstTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            //_currentTask = ConfigurationHelper.Default.Tasks.FirstOrDefault(t => t.Name == lstTasks.SelectedItem.ToString());
            if (_currentTask != lstTasks.SelectedItem)
                LoadTask(lstTasks.SelectedItem as PicPickConfigTask);
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            StartTask(_currentTask);

            
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            ConfigurationHelper.Save();
            SetDirty(false);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isDirty)
            {
                if (e.CloseReason == CloseReason.WindowsShutDown)
                    ConfigurationHelper.Save(ConfigurationHelper.LoadedFile + ".backup");
                else
                {
                    if (!Msg.ShowQ("Your project is not saved. Do you want to exit without saving?"))
                        e.Cancel = true;
                }
            }

            if (!e.Cancel)
                _taskForm.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _currentTask.ReadFiles();
        }

        private void AppendLog(string text, Color color)
        {
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;

            rtbLog.SelectionColor = color;
            rtbLog.AppendText(text + Environment.NewLine);
            rtbLog.SelectionColor = rtbLog.ForeColor;
        }

        #region IAppender implementation
        public void DoAppend(LoggingEvent loggingEvent)
        {
            if (!logColors.ContainsKey(loggingEvent.Level))
                return;
            AppendLog($"({loggingEvent.Level.Name}) {loggingEvent.MessageObject.ToString()}", logColors[loggingEvent.Level]);
        }
        #endregion

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
