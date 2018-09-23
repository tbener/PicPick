using PicPick.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicPick.Views
{
    public partial class WizardForm : Form
    {
        int _currentPageIndex = 0;
        Panel[] _panelPages;
        PicPickConfigTask _originalTask;
        CancellationTokenSource cts = new CancellationTokenSource();

        PicPickConfigTaskDestination _activeDestination;

        bool _isNew;
        bool _openSourceDialogDirectly;
        bool _openDestinationDialogDirectly;

        private void WizardForm_Load(object sender, EventArgs e)
        {
            // set initial layout properties
            //_panelPages.ToList().ForEach(p => p.Dock = DockStyle.Fill);

            this.Size = new Size(750, 500);

            lblFileCount.Text = "";

            Refresh();
        }

        public WizardForm(PicPickConfigTask task)
        {
            InitializeComponent();

            if (task == null)
            {
                // NEW TASK
                _isNew = true;
                _openSourceDialogDirectly = true;
                //_openDestinationDialogDirectly = true;    // this is found to be a bit confusing. plus, it can be left empty.

                this.Text = "New Activity Wizard";

                CurrentTask = new PicPickConfigTask();
                CurrentTask.Name = "My Activity";
            }
            else {
                // set the Task in context
                _originalTask = task;
                CurrentTask = (PicPickConfigTask)task.Clone();

                this.Text = $"Activity Wizard - {task.Name}";
            }

            
            if (CurrentTask.Source == null)
                CurrentTask.Source = new PicPickConfigTaskSource();
            if (CurrentTask.DestinationList.Count == 0)
                CurrentTask.DestinationList.Add(new PicPickConfigTaskDestination());

            _activeDestination = CurrentTask.DestinationList[0];

            CurrentTask.Source.PropertyChanged += Source_PropertyChanged;

            // bind the controls to the task properties
            BindControls();

            ctlPattern.TextChanged += (s, e) => RefreshPatternPreview();

            _panelPages = new Panel[] { panelStart, panelSource, panelDestination, panelPattern, panelOptions, panelFinish };

        }

        void BindControls()
        {
            txtName.DataBindings.Add("Text", CurrentTask, "Name");
            pathSource.DataBindings.Add("Text", CurrentTask.Source, "Path", false, DataSourceUpdateMode.OnPropertyChanged);
            txtFilter.DataBindings.Add("Text", CurrentTask.Source, "Filter");
            pathDestination.DataBindings.Add("Text", _activeDestination, "Path", false, DataSourceUpdateMode.OnPropertyChanged);
            ctlPattern.DataBindings.Add("Text", _activeDestination, "Template", false, DataSourceUpdateMode.OnPropertyChanged);

            // Bind radio buttons
            Binding bind = new Binding("Checked", CurrentTask, "DeleteSourceFiles");
            bind.Format += (s1, e1) => e1.Value = !(bool)e1.Value;
            bind.Parse += (s1, e1) => e1.Value = !(bool)e1.Value;
            // add the negetive binding
            radioDontDeleteSource.DataBindings.Add(bind);
            // add the normal binding
            radioDeleteSource.DataBindings.Add("Checked", CurrentTask, "DeleteSourceFiles");
        }


        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // count files?
        }

        private void RefreshPatternPreview()
        {
            _activeDestination.Template = ctlPattern.Text;
            lblPatternPreview.Text = _activeDestination.GetFullPath(dtSample.Value);
        }

        async Task ReadFilesAsync()
        {
            try
            {
                // Cancel previous operations
                cts.Cancel();

                // reset state
                lblFileCount.Text = "";

                // Create a new cancellations token and await a new task to count files
                cts = new CancellationTokenSource();
                int count = await CurrentTask.GetFileCount(cts.Token);
                lblFileCount.Text = $"{count} files found";
            }
            catch (OperationCanceledException)
            {
                // operation was canceled
                lblFileCount.Text = "";
            }
            catch (Exception ex)
            {
                // error in counting files. most probably because folder doesn't exist.
                lblFileCount.Text = ex.Message;
            }
        }

        

        

        ResourceManager res = new ResourceManager("PicPick.Properties.Resources", typeof(Properties.Resources).Assembly);
        bool _isLastPage;

        public override void Refresh()
        {
            base.Refresh();

            _isLastPage = _currentPageIndex == _panelPages.Length - 1;

            btnNext.Text = _isLastPage ? "Done" : "Next >";
            btnBack.Enabled = _currentPageIndex > 0;

            Panel activePanel = _panelPages[_currentPageIndex];

            activePanel.Dock = DockStyle.Fill;
            activePanel.BringToFront();
            lblPageTitle.Text = activePanel.Tag.ToString();  // res.GetString($"WIZARD_TITLE_{_currentPageIndex}");

            if (activePanel == panelSource)
            {
                if (_openSourceDialogDirectly)
                {
                    _openSourceDialogDirectly = false;
                    pathSource.ShowOpenFolderDialog();
                }
            }
            if (activePanel == panelDestination)
            {
                if (_openDestinationDialogDirectly)
                {
                    _openDestinationDialogDirectly = false;
                    pathDestination.ShowOpenFolderDialog();
                }
            }
        }

        private void SaveChanges()
        {
            if (_originalTask == null)
                return;

            if (_originalTask.Source == null)
                _originalTask.Source = new PicPickConfigTaskSource();
            if (_originalTask.DestinationList.Count == 0)
                _originalTask.DestinationList.Add(new PicPickConfigTaskDestination());

            _originalTask.Source.Path = CurrentTask.Source.Path;
            _originalTask.Source.Filter = CurrentTask.Source.Filter;
            _originalTask.DestinationList[0].Path = _activeDestination.Path;
            _originalTask.DestinationList[0].Template = _activeDestination.Template;

            _originalTask.DeleteSourceFiles = CurrentTask.DeleteSourceFiles;
            _originalTask.Name = CurrentTask.Name;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_isLastPage)
            {
                SaveChanges();
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                _currentPageIndex++;
                Refresh();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            _currentPageIndex--;
            Refresh();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }


        public PicPickConfigTask CurrentTask { get; private set; }

        private async void btnFindFiles_Click(object sender, EventArgs e)
        {
            await ReadFilesAsync();
        }

        private void dtSample_ValueChanged(object sender, EventArgs e)
        {
            RefreshPatternPreview();
        }
    }
}
