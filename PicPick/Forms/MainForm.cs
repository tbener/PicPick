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
    public partial class MainForm : Form
    {
        PicPickConfigTask _currentTask;
        bool _isDirty;
        bool _isLoading;
        PreviewForm _taskForm;

        Dictionary<LOG_TYPE, Color> logColors = new Dictionary<LOG_TYPE, Color>()
        {
            { LOG_TYPE.ERROR, Color.Red },
            { LOG_TYPE.INFO, Color.Green },
            { LOG_TYPE.WARNING, Color.Red }
        };

        public MainForm()
        {
            InitializeComponent();
            rtbLog.Clear();

            pathSource.Changed += (s, e) => SetDirty(s);
            txtFilter.TextChanged += (s, e) => SetDirty(s);
            lstTasks.ItemCheck += (s, e) => SetDirty(s);
            LogHandler.OnLog += LogHandler_OnLog;

            _taskForm = new PreviewForm();
        }

        

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadFile();
            SetDirty(sender, e, false);
        }



        private void SetDirty(object sender, EventArgs e = null,  bool isDirty = true)
        {
            if (_isLoading) return;

            this.Text = AppInfo.AppName + (isDirty ? " *" : "");
            _isDirty = isDirty;

            if (sender == pathSource) _currentTask.Source.Path = pathSource.Text;
            if (sender == txtFilter) _currentTask.Source.Filter = txtFilter.Text;
            if (sender == pathSource || sender == txtFilter) SourceUpdated();

                if (isDirty) _currentTask?.SetDirty();
        }

        private void SourceUpdated()
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
                SourceUpdated();

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
                task.Execute();
                LogHandler.Log("Finished");
            }
            catch (Exception ex)
            {
                LogHandler.Log("Finished with errors:", LOG_TYPE.ERROR);
                LogHandler.Log(ex.Message, LOG_TYPE.ERROR);
            }
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

        private void LogHandler_OnLog(string msg, LOG_TYPE logType)
        {
            AppendLog(msg, logColors[logType]);
        }

        private void AppendLog(string text, Color color)
        {
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;

            rtbLog.SelectionColor = color;
            rtbLog.AppendText(text + Environment.NewLine);
            rtbLog.SelectionColor = rtbLog.ForeColor;
        }

    }
}
