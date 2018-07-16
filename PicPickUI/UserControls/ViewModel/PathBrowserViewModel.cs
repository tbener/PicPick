using PicPickUI.Commands;
using PicPickUI.ViewModel;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TalUtils;

namespace PicPickUI.UserControls.ViewModel
{
    public class PathBrowserViewModel : BaseViewModel
    {
        public static EventHandler OnPathChanged;

        public bool AllowPathNotExists = true;  // not yet in use

        public ICommand BrowseCommand { get; set; }
        public string BasePath { get; set; }

        public PathBrowserViewModel()
        {
            BrowseCommand = new RelayCommand(BrowseFolder);
        }


        private void BrowseFolder()
        {
            var dlg = new FolderBrowser2();
            dlg.DirectoryPath = PathHelper.GetFullPath(BasePath, Path);
            if (dlg.ShowDialog(null) == DialogResult.OK)
            {
                Path = string.IsNullOrEmpty(BasePath) ? dlg.DirectoryPath : PathHelper.GetInnerPath(BasePath, dlg.DirectoryPath);
            }
        }

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value);
                OnPathChanged?.Invoke(this, null);
            }
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(PathBrowserViewModel), 
                new FrameworkPropertyMetadata(""),
                new ValidateValueCallback(CheckPath));

      

        public static bool CheckPath(object value)
        {
            return true;

            //if (value == null) return true;
            //string newPath = value as string;
            //if (newPath == "") return true;
            //if (PathHelper.Exists(newPath))
            //    return true;
            //return false;
        }
    }
}
