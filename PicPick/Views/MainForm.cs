﻿using log4net.Appender;
using log4net.Core;
using PicPick.Models;
using PicPick.Configuration;
using PicPick.Helpers;
using PicPick.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalUtils;

namespace PicPick.Views
{
    public partial class MainForm : Form, IAppender
    {
        PicPickConfigTask _currentTask;
        bool _isDirty;
        bool _isLoading;
        CancellationTokenSource cts = new CancellationTokenSource();
        private BindingSource _bindingSource;

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

            InitDatabindings();
            EnableTaskNameEdit(false);

            pathSource.Changed += async (s, e) => await SetDirty(s);
            txtFilter.TextChanged += async (s, e) => await SetDirty(s);

            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(this);

            ResetProgress();

            mnuAutoSave.Checked = Properties.Settings.Default.AutoSave;

            ShareTaskContextMenu_TempWorkaround();

        }

        private void InitDatabindings()
        {
            _bindingSource = new BindingSource { DataSource = typeof(PicPickConfigTask) };

            _bindingSource.DataMemberChanged += _bindingSource_DataMemberChanged;
            _bindingSource.CurrentItemChanged += _bindingSource_CurrentItemChanged;

            txtTaskName.DataBindings.Add("Text", _bindingSource, "Name");
            pathSource.DataBindings.Add("Text", _bindingSource, "Source.Path", false, DataSourceUpdateMode.OnPropertyChanged);
            txtFilter.DataBindings.Add("Text", _bindingSource, "Source.Filter");
            chkDeleteSourceFiles.DataBindings.Add("Checked", _bindingSource, "DeleteSourceFiles");

            pathSource.Changed += PathSource_Changed;
        }

        private void PathSource_Changed(object sender, EventArgs e)
        {
            //_currentTask.Source.Path = pathSource.Text;
            Debug.Print("PathSource_Changed");
        }

        private void _bindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            Debug.Print("_bindingSource_CurrentItemChanged");
        }

        private void _bindingSource_DataMemberChanged(object sender, EventArgs e)
        {
            Debug.Print("_bindingSource_DataMemberChanged");
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {

            LoadFile();
            await SetDirty(sender, e, false);

        }

        #region Share Task Context Menu - Temporary Workaround

        ToolStripItem[] items;

        private void ShareTaskContextMenu_TempWorkaround()
        {
            items = new ToolStripItem[mnuTask.DropDownItems.Count];
            mnuTask.DropDownItems.CopyTo(items, 0);

            contextMenuTask.Items.Clear();
            lstTasks.MouseDown += LstTasks_MouseDown;
            contextMenuTask.Closed += ContextMenuTask_Closed;

            contextMenuTask.Opening += ContextMenuTask_Opening;
        }

        private void ContextMenuTask_Opening(object sender, CancelEventArgs e)
        {
            //items[0].Enabled = !items[0].Enabled;
        }

        private void ContextMenuTask_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            mnuTask.DropDownItems.AddRange(items);
        }


        private void LstTasks_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                if (contextMenuTask.Items.Count == 0)
                    contextMenuTask.Items.AddRange(items);
        }


        #endregion

        public override async void Refresh()
        {
            base.Refresh();

            await ReadSourceAsync();

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
                btnRun.Enabled = count > 0;
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

        #region SetDirty

        /// <summary>
        /// This function should be called on every property that was changed and not saved.
        /// If the Autosave feature is on, then call save()
        /// Otherwise, mark with star (*)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="isDirty"></param>
        /// <returns></returns>
        private async Task SetDirty(object sender, EventArgs e = null, bool isDirty = true)
        {
            if (_isLoading) return;

            SetStatus("");

            if (Properties.Settings.Default.AutoSave && isDirty)
            {
                await Save();
                return;
            }

            this.Text = AppInfo.AppName + (isDirty ? " *" : "");
            _isDirty = isDirty;

            if (isDirty)
            {
                //if (sender == pathSource) _currentTask.Source.Path = pathSource.Text;
                //if (sender == txtFilter) _currentTask.Source.Filter = txtFilter.Text;
                //if (sender == pathSource || sender == txtFilter)
                //{
                //    await ReadSourceAsync();
                //}

                _currentTask?.SetDirty();

            }
        }

        private async Task SetDirty(bool isDirty)
        {
            await SetDirty(null, null, isDirty);
        }

        private async Task SetDirty()
        {
            await SetDirty(true);
        }

        #endregion

        private async Task Save()
        {
            ConfigurationHelper.Save();
            await SetDirty(false);
            SetStatus("Saved");
        }

        private void SetStatus(string s)
        {
            toolStripStatusLabel.Text = s;
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

                // 
                mnuOpen.Visible = false;

                LoadTasks();

                //mnuOpen.DropDownItems[0].PerformClick();    // this will invoke LoadProject()
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

        private void LoadTasks(PicPickConfigTask activeTask = null)
        {
            if (activeTask == null)
                // if there are existing tasks in the list, try saving the selected one to select it later
                if (lstTasks.Items.Count > 0)
                    activeTask = lstTasks.SelectedItem as PicPickConfigTask;

            // clear and fill the tasks
            lstTasks.Items.Clear();
            lstTasks.Items.AddRange(
                    ConfigurationHelper.Default.TaskList.ToArray()
                    );

            // select a task
            if (activeTask != null)
            {
                if (ConfigurationHelper.Default.TaskList.Contains(activeTask))
                    lstTasks.SelectedItem = activeTask;
                else
                    lstTasks.SetSelected(0, true);              // this will invoke LoadTask()
            }
            else
                lstTasks.SetSelected(0, true);              // this will invoke LoadTask()
        }

        // At this point we don't use project.
        // There is one project that includes all tasks.

        //private void LoadProject(PicPickConfigProjectsProject proj)
        //{
        //    try
        //    {
        //        _isLoading = true;
        //        var taskList = proj.TaskRef.Select(t => t.Name).ToArray();
        //        //lstTasks.Items.AddRange(taskList);
        //        lstTasks.SetSelected(0, true);              // this will invoke LoadTask()
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.Handle(ex, "Error while loading project.");
        //    }
        //    finally
        //    {
        //        _isLoading = false;
        //    }
        //}

        private async Task LoadTask(PicPickConfigTask task)
        {
            try
            {
                _isLoading = true;
                _currentTask = task;
                Task readSource = ReadSourceAsync();

                foreach (var ctl in pnlDestinations.Controls.Cast<Control>().ToArray())
                {
                    ctl.Dispose();
                }

                if (_currentTask == null) return;

                _bindingSource.DataSource = _currentTask;

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
            // Create new destination control
            DestinationControl dstCtl = new DestinationControl(dest);
            dstCtl.Changed += (s, e) => SetDirty();
            dstCtl.RemoveButtonClicked += RemoveDestination_Click;
            dstCtl.Dock = DockStyle.Top;

            // Add it to the list
            pnlDestinations.Controls.Add(dstCtl);
            dstCtl.BringToFront();  // to add it last
        }

        private void RemoveDestination_Click(object sender, EventArgs e)
        {
            // get the destination control
            DestinationControl dstCtl = (DestinationControl)sender;
            // get the Destination object 
            PicPickConfigTaskDestination dest = dstCtl.Destination;
            // remove this destination from the list
            _currentTask.DestinationList.Remove(dest);
            // remove the destination control from the UI
            dstCtl.Dispose();

            SetDirty();
        }

        private void btnAddDestination_Click(object sender, EventArgs e)
        {
            PicPickConfigTaskDestination dest = new PicPickConfigTaskDestination();
            _currentTask.DestinationList.Add(dest);
            AddDestinationControl(dest);
            SetDirty();
        }

        private async Task StartTask(PicPickConfigTask task)
        {
            SetStatus($"Running {task.Name}");

            ResetProgress();
            ProgressInformation progressInfo = new ProgressInformation();
            //Progress<ProgressInformation> progress = new Progress<ProgressInformation>();

            // show progress dialog
            ProgressForm pForm = new ProgressForm();
            pForm.Progress = (Progress<ProgressInformation>)progressInfo.Progress;
            pForm.Show(this);
            pForm.Refresh();

            progressInfo.MainOperation = "Initializing...";
            progressInfo.Report();

            this.Enabled = false;
            pForm.FormClosed += delegate { this.Enabled = true; };

            //progress.ProgressChanged += Progress_ProgressChanged;
            rtbLog.Clear();
            try
            {
                //SetProgress("Initializing...", 0);
                await task.MapFilesAsync(cts.Token);
                pForm.Max = task.CountTotal;
                progCopy.Maximum = task.CountTotal;

                LogHandler.Log($"Starting Task: {task.Name}");
                AskWhatToDoForm.CancellationTokenSource = cts;
                await task.ExecuteAsync(progressInfo, cts.Token);
                LogHandler.Log("Finished");
                //SetProgress("Finished");
                SetStatus("Finished");
            }
            catch (OperationCanceledException)
            {
                pForm.Close();
                SetStatus("Canceled by user");
                LogHandler.Log("Canceled by user");
                SetProgress("Canceled by user");
            }
            catch (Exception ex)
            {
                SetStatus("Finished with errors: " + ex.Message);
                LogHandler.Log("Finished with errors:", Level.Error);
                LogHandler.Log(ex.Message, Level.Error);
                SetProgress("Finished with errors");
            }
            finally
            {
                await ReadSourceAsync();
            }
        }

        private void ResetProgress()
        {
            lblProgress.Text = "";
            progCopy.Maximum = 0;
            progCopy.Value = 0;
            progCopy.Text = "";
        }

        private void SetProgress(string progressHeader, int progressValue = -1)
        {
            lblProgress.Text = progressHeader;
            if (progressValue > -1)
                progCopy.Value = progressValue;
        }

        private void SetProgress(ProgressInformation progressInfo)
        {
            lblProgress.Text = progressInfo.CurrentOperation;
            progCopy.Value = progressInfo.CountDone;
        }

        private void Progress_ProgressChanged(object sender, ProgressInformation info)
        {
            //SetProgress($"Copying to {info.DestinationFolder}", $"{info.CountDone} of {progCopy.Maximum}", info.CountDone);
            SetProgress(info);
        }

        private void MnuOpen_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            //PicPickConfigProjectsProject p = e.ClickedItem.Tag as PicPickConfigProjectsProject;
            //if (p != null)
            //    LoadProject(p);
        }

        // Todo: avoid reload when clicking on the selected task
        private async void lstTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            //_currentTask = ConfigurationHelper.Default.Tasks.FirstOrDefault(t => t.Name == lstTasks.SelectedItem.ToString());
            if (_currentTask != lstTasks.SelectedItem)
                await LoadTask(lstTasks.SelectedItem as PicPickConfigTask);
        }

        private async void btnCheck_Click(object sender, EventArgs e)
        {
            await StartTask(_currentTask);
        }

        private async void mnuSave_Click(object sender, EventArgs e)
        {
            await Save();
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

        private void txtTaskName_DoubleClick(object sender, EventArgs e)
        {
            EnableTaskNameEdit(true);
        }

        bool _taskRename = false;

        private void EnableTaskNameEdit(bool enable)
        {
            Debug.Print("EnableTaskNameEdit start");
            if (!enable)
            {
                _taskRename = false;
                txtTaskName.Enabled = false;
                txtTaskName.Enabled = true;
            }
            txtTaskName.ReadOnly = !enable;
            txtTaskName.BorderStyle = enable ? BorderStyle.FixedSingle : BorderStyle.None;

            //if (!enable)
            //    pathSource.Focus();
            if (enable) _taskRename = true;
            Debug.Print("EnableTaskNameEdit end");
        }



        private bool TryRenameTask(string name)
        {
            if (name != string.Empty)
            {
                _currentTask.Name = name;
                lstTasks.Items[lstTasks.SelectedIndex] = _currentTask;
                SetDirty();

                return true;
            }

            // undo
            txtTaskName.Text = _currentTask.Name;
            return false;
        }



        private void txtTaskName_Leave(object sender, EventArgs e)
        {
            if (_taskRename)
            {
                Debug.Print("Leave");
                if (!TryRenameTask(txtTaskName.Text))
                {
                    txtTaskName.Focus();
                    return;
                }
                EnableTaskNameEdit(false);
            }

        }

        private void txtTaskName_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                //ENTER key pressed
                case '\r':
                    TryRenameTask(txtTaskName.Text);
                    EnableTaskNameEdit(false);
                    break;
                case (char)27:
                    txtTaskName.Text = _currentTask.Name;
                    EnableTaskNameEdit(false);
                    break;
                default:
                    Debug.Print(e.KeyChar.ToString());
                    break;
            }

        }

        private void chkDeleteSourceFiles_CheckedChanged(object sender, EventArgs e)
        {
            _currentTask.DeleteSourceFiles = chkDeleteSourceFiles.Checked;
            SetDirty();
        }

        private void mnuAutoSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoSave = mnuAutoSave.Checked;
            Properties.Settings.Default.Save();
            SetStatus("Auto save is " + (Properties.Settings.Default.AutoSave ? "on" : "off"));
        }

        private void mnuTaskRename_Click(object sender, EventArgs e)
        {
            EnableTaskNameEdit(true);
            txtTaskName.Focus();
        }

        private async void mnuTaskRun_Click(object sender, EventArgs e)
        {
            await StartTask(_currentTask);
        }

        private void mnuTaskDelete_Click(object sender, EventArgs e)
        {
            if (Msg.ShowQ($"Are you sure you want to delete {_currentTask.Name}?"))
            {
                PicPickConfigTask taskToDelete = lstTasks.SelectedItem as PicPickConfigTask;
                ConfigurationHelper.Default.TaskList.Remove(taskToDelete);
                LoadTasks();
            }
        }

        
        private void mnuTaskMoveUp_Click(object sender, EventArgs e)
        {
            if (_currentTask == null)
                return;

            int index = ConfigurationHelper.Default.TaskList.IndexOf(_currentTask);
            if (index > 0)
            {
                ConfigurationHelper.Default.TaskList.Remove(_currentTask);
                ConfigurationHelper.Default.TaskList.Insert(index - 1, _currentTask);
                LoadTasks(_currentTask);
            }
        }

        private void mnuTaskMoveDown_Click(object sender, EventArgs e)
        {
            if (_currentTask == null)
                return;

            int index = ConfigurationHelper.Default.TaskList.IndexOf(_currentTask);
            if (index < (ConfigurationHelper.Default.TaskList.Count-1))
            {
                ConfigurationHelper.Default.TaskList.Remove(_currentTask);
                ConfigurationHelper.Default.TaskList.Insert(index + 1, _currentTask);
                LoadTasks(_currentTask);
            }
        }

        private void mnuTaskWizard_Click(object sender, EventArgs e)
        {
            WizardForm wiz = new WizardForm(_currentTask);
            wiz.ShowDialog();
        }

        private void mnuTaskAdd_Click(object sender, EventArgs e)
        {
            // open the wizard for new task
            WizardForm wiz = new WizardForm(null);
            if (wiz.ShowDialog() == DialogResult.OK)
            {
                // display the new task
                PicPickConfigTask newTask = wiz.CurrentTask;
                ConfigurationHelper.Default.TaskList.Add(newTask);
                LoadTasks(newTask);
            }
        }
    }
}
