using PicPick.Configuration;
using PicPick.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Helpers
{
    class ProjectRunner
    {
        ProgressForm _progressForm;
        PicPickConfigProjectsProject _project;

        public ProjectRunner(string taskName)
        {
            this.TaskName = taskName;
            _progressForm = new ProgressForm();
            _progressForm.Text = taskName;
        }

        public bool Init()
        {
            _project = ConfigurationHelper.Default.Projects.ProjectByName(TaskName);
            foreach (PicPickConfigTask task in _project.Tasks)
            {
                task.Runner = new TaskRunner(task, _progressForm);
                task.Init();
            }
            return (_project != null && _project.Tasks != null);
        }

        public void Run()
        {
            foreach (PicPickConfigTask task in _project.Tasks)
            {
                task.Runner.Run();
            }
        }

        public string TaskName { get; set; }
    }
}
