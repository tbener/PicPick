using PicPick.Configuration;
using PicPick.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.Helpers
{
    public class TaskRunner
    {
        ProgressForm _progressForm;
        PicPickConfigTask _task;

        public TaskRunner(PicPickConfigTask task, ProgressForm progressForm)
        {
            _task = task;
            _progressForm = progressForm;
        }

        public void Run()
        {
            foreach (PicPickConfigTaskDestination dst in _task.Destination)
            {
                foreach (var kv in dst.Mapping)
                {
                    // get the full path and CREATE it if not exists
                    string fullPath = PathHelper.GetFullPath(dst.Path, kv.Key, true);
                    CopyFilesHandler info = kv.Value;

                    // The operation is done on a banch of files at once!
                    if (dst.Move)
                    {
                        Debug.Print("Moving {0} files to {1}", info.FileList.Count(), fullPath);
                        ShellFileOperation.MoveItems(info.FileList, fullPath);
                    }
                    else
                    {
                        Debug.Print("Copying {0} files to {1}", info.FileList.Count(), fullPath);
                        ShellFileOperation.CopyItems(info.FileList, fullPath);
                    }
                }
            }

        }

    }
}
