using PicPick.Commands;
using PicPick.ViewModel;
using System;
using System.Windows.Forms;
using System.Windows.Input;
using TalUtils;

namespace PicPick.ViewModel.UserControls
{
    public class GenericPathClass<T>
    {

    }
    public class PathBrowserViewModel : BaseViewModel
    {
        // This is the class that holds the Path property.
        // We can't use interface because this is coming from another assembly.
        // We also can't use the class type explicitely because there is more than one class. We just know they have the property "Path".
        private object PathClass { get; set; }

        public static EventHandler OnPathChanged;

        public bool AllowPathNotExists = true;  // not yet in use

        public ICommand BrowseCommand { get; set; }
        public string BasePath { get; set; }

        public PathBrowserViewModel(object pathClass)
        {
            BrowseCommand = new RelayCommand(BrowseFolder);
            PathClass = pathClass;
            BasePath = "";
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
            get
            {
                return PathClass.GetType().GetProperty("Path").GetValue(PathClass, null).ToString();
            }
            set
            {
                PathClass.GetType().GetProperty("Path").SetValue(PathClass, value);
                OnPropertyChanged("Path");
            }
        }


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
