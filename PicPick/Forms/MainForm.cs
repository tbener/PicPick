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
using System.Threading;
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
        CancellationTokenSource cts = new CancellationTokenSource();

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

            pathSource.Changed += async (s, e) => await SetDirty(s);
            txtFilter.TextChanged += async (s, e) => await SetDirty(s);
            lstTasks.ItemCheck += async (s, e) => await SetDirty(s);

            _taskForm = new PreviewForm();

            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(this);
        }



        private async void MainForm_Load(object sender, EventArgs e)
        {


            LoadFile();
            await SetDirty(sender, e, false);
        }

        public override async void Refresh()
        {
            base.Refresh();

            await ReadSourceAsync();
        }


        private async Task SetDirty(object sender, EventArgs e = null, bool isDirty = true)
        {
            if (_isLoading) return;

            this.Text = AppInfo.AppName + (isDirty ? " *" : "");
            _isDirty = isDirty;

            if (sender == pathSource) _currentTask.Source.Path = pathSource.Text;
            if (sender == txtFilter) _currentTask.Source.Filter = txtFilter.Text;
            if (sender == pathSource || sender == txtFilter)
            {
                await ReadSourceAsync();
            }

            if (isDirty) _currentTask?.SetDirty();
        }

        private async Task<int> CountFilesAsync()
        {
            try
            {
                await _currentTask.ReadFilesAsync(cts.Token);
                return _currentTask.FileCount;
            }
            catch (OperationCanceledException)
            {
                return -1;
            }
        }

        
        async Task ReadSourceAsync()
        {
            try
            {
                // Cancel previous operations
                cts.Cancel();

                // reset state
                btnRun.Enabled = false;
                lblFileCount.Text = "";

                // Create a new cancellations token and await a new task to count files
                cts = new CancellationTokenSource();
                int count = await _currentTask.GetFileCount(cts.Token);
                lblFileCount.Text = $"{count} files found";
                btnRun.Enabled = _currentTask.FileCount > 0;
            }
            catch (OperationCanceledException)
            {
                // operation was canceled
                lblFileCount.Text = "";
            }
            catch (Exception)
            {
                // error in counting files. most probably because folder doesn't exist.
                lblFileCount.Text = "---";
            }
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            await ReadSourceAsync();
        }

        private async Task SetDirty(bool isDirty)
        {
            await SetDirty(null, null, isDirty);
        }
        private async Task SetDirty()
        {
            await SetDirty(true);
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

        private async Task LoadTask(PicPickConfigTask task)
        {
            try
            {
                _isLoading = true;
                _currentTask = task;
                Task readSource = ReadSourceAsync();

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

                _currentTask.DestinationList.ForEach(AddDestinationControl);
                await readSource;

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

        private async Task StartTask(PicPickConfigTask task)
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
            await ReadSourceAsync();
        }

        private void MnuOpen_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            PicPickConfigProjectsProject p = e.ClickedItem.Tag as PicPickConfigProjectsProject;
            if (p != null)
                LoadProject(p);
        }

        // Todo: avoid reload when clicking on the selected task
        private async void lstTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            //_currentTask = ConfigurationHelper.Default.Tasks.FirstOrDefault(t => t.Name == lstTasks.SelectedItem.ToString());
            if (_currentTask != lstTasks.SelectedItem)
                await LoadTask(lstTasks.SelectedItem as PicPickConfigTask);
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            StartTask(_currentTask);


        }

        private async void mnuSave_Click(object sender, EventArgs e)
        {
            ConfigurationHelper.Save();
            await SetDirty(false);
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

        private void button2_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }
    }
}
