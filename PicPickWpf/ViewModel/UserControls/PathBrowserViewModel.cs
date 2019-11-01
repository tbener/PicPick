using PicPick.Commands;
using PicPick.Interfaces;
using PicPick.Models;
using PicPick.ViewModel;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TalUtils;

namespace PicPick.ViewModel.UserControls
{
    enum PATH_TYPE_ENUM
    {
        Source,
        Destination
    }

    public class PathBrowserViewModel : BaseViewModel
    {
        // This is the class that holds the Path property.
        // We can't use interface because this is coming from another assembly.
        // We also can't use the class type explicitely because there is more than one class. We just know they have the property "Path".
        //private object PathClass { get; set; }

        public ICommand BrowseCommand { get; set; }
        public ICommand OpenExplorerCommand { get; set; }

        public static EventHandler OnPathChanged;
        public bool AllowPathNotExists = true;  // not yet in use
        private string textboxTooltip;

        public string BasePath { get; set; }

        private PicPickProjectActivityDestination _destination;
        private PicPickProjectActivitySource _source;
        private IPath _pathClass;


        private PathBrowserViewModel()
        {
            BrowseCommand = new RelayCommand(BrowseFolder);
            OpenExplorerCommand = new RelayCommand(OpenInExplorer);
        }

        /// <summary>
        /// Provides a Path view for a Destination, relative to the Source
        /// </summary>
        /// <param name="destination">The Destination object to provide the path for</param>
        /// <param name="source">The Source object that the Destination is related to</param>
        public PathBrowserViewModel(PicPickProjectActivityDestination destination, PicPickProjectActivitySource source) : this()
        {
            _destination = destination;
            _source = source;

            OnPropertyChanged(nameof(Path));
            OnPropertyChanged(nameof(PathToDisplay));
        }

        /// <summary>
        /// Provides a Path view for a Source object
        /// </summary>
        /// <param name="source"></param>
        public PathBrowserViewModel(PicPickProjectActivitySource source) : this()
        {
            _source = source;

            OnPropertyChanged(nameof(Path));
            OnPropertyChanged(nameof(PathToDisplay));
        }


        private void BrowseFolder()
        {
            var dlg = new FolderBrowser2();
            dlg.DirectoryPath = PathHelper.GetFullPath(BasePath, Path);
            if (dlg.ShowDialog(null) == DialogResult.OK)
            {
                Path = string.IsNullOrEmpty(BasePath) ? dlg.DirectoryPath : PathHelper.GetRelativePath(BasePath, dlg.DirectoryPath);
            }
        }

        private void OpenInExplorer()
        {
            if (PathExists())
                ExplorerHelper.OpenPath(this.Path);
            else
                Msg.Show($"{Path} doesn't exist");
        }

        private bool PathExists()
        {
            return PathHelper.Exists(this.Path);
        }

        private PATH_TYPE_ENUM PathType => _destination == null ? PATH_TYPE_ENUM.Source : PATH_TYPE_ENUM.Destination;

        public Visibility PreviewVisibility => PathType == PATH_TYPE_ENUM.Destination ? Visibility.Visible : Visibility.Visible;


        public string PathToDisplay
        {
            get
            {
                if (PathType == PATH_TYPE_ENUM.Source)
                    return Path;
                return PathHelper.GetRelativePath(_source.Path, Path);
            }
            set
            {
                Path = PathHelper.GetFullPath(_source.Path, value);
                //if (System.IO.Path.IsPathRooted(value) || PathType == PATH_TYPE_ENUM.Source)
                //    Path = value;
                //else
                //    Path = System.IO.Path.Combine(_source.Path, value);
            }
        }


        public string Path
        {
            get
            {
                return PathType == PATH_TYPE_ENUM.Source ? _source.Path : PathHelper.GetFullPath(_source.Path, _destination.Path); 
            }
            set
            {
                if (PathType == PATH_TYPE_ENUM.Source)
                    _source.Path = value;
                else
                    _destination.Path = PathHelper.GetRelativePath(_source.Path, value);

                OnPropertyChanged(nameof(Path));
                OnPropertyChanged(nameof(PathToDisplay));
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

        public string TextboxTooltip
        {
            get => textboxTooltip; set
            {
                textboxTooltip = value;
                OnPropertyChanged(nameof(TextboxTooltip));
            }
        }
    }
}
