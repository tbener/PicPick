using PicPick.Commands;
using PicPick.Models;
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

        private PicPickProjectActivityDestination _destination;
        private PicPickProjectActivitySource _source;

        string _sourcePreviousPath = "";

        private PathBrowserViewModel()
        {
            BrowseCommand = new RelayCommand(BrowseFolder);
            OpenExplorerCommand = new RelayCommand(OpenInExplorer);
        }

        /// <summary>
        /// Provides a Path view for a Destination, which can be relative to the Source
        /// </summary>
        /// <param name="destination">The Destination object to provide the path for</param>
        /// <param name="source">The Source object that the Destination is related to</param>
        public PathBrowserViewModel(PicPickProjectActivityDestination destination, PicPickProjectActivitySource source) : this()
        {
            _destination = destination;
            _source = source;
            SetPath(PathHelper.GetFullPath(_source.Path, _destination.Path));

            _sourcePreviousPath = _source.Path;

            _source.PropertyChanged += Source_PropertyChanged;
        }

        /// <summary>
        /// Provides a Path view for a Source object
        /// </summary>
        /// <param name="source"></param>
        public PathBrowserViewModel(PicPickProjectActivitySource source) : this()
        {
            _source = source;
            SetPath(_source.Path);
        }

        private void Source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // if source path changed, and the PathToDisplay is not rooted - we need to do some manipulation
            if (e.PropertyName == "Path")
            {
                if (!System.IO.Path.IsPathRooted(PathToDisplay))
                    if (_destination.KeepAbsolute)
                        // the path should stay as it was
                        PathToDisplay = PathHelper.GetFullPath(_sourcePreviousPath, PathToDisplay);
                    else
                        // the full path should change according to the new source
                        FullPath = PathHelper.GetFullPath(_source.Path, PathToDisplay);
            }
        }


        private void BrowseFolder()
        {
            var dlg = new FolderBrowser2();
            dlg.DirectoryPath = PathHelper.GetFullPath(_source.Path, FullPath);
            if (dlg.ShowDialog(null) == DialogResult.OK)
            {
                SetPath(dlg.DirectoryPath);
            }
        }

        private void OpenInExplorer()
        {
            if (PathExists())
                ExplorerHelper.OpenPath(FullPath);
            else
                Msg.Show($"{FullPath} doesn't exist");
        }

        private bool PathExists()
        {
            return PathHelper.Exists(FullPath);
        }

        private PATH_TYPE_ENUM PathType => _destination == null ? PATH_TYPE_ENUM.Source : PATH_TYPE_ENUM.Destination;

        public Visibility PreviewVisibility => PathType == PATH_TYPE_ENUM.Destination ? Visibility.Visible : Visibility.Visible;

        private string _pathToDisplay;

        /// <summary>
        /// Given a full path, the PathToDisplay backing field(!) is being set accordingly.
        /// On a normal flow the PathToDisplay property is changed and then changes the full path.
        /// This function is called only on initialize and on setting the folder through the Brose button.
        /// </summary>
        /// <param name="fullPath">The full path to set and that will affect the PathToDisplay</param>
        private void SetPath(string fullPath)
        {
            FullPath = fullPath;
            if (PathType == PATH_TYPE_ENUM.Source || _destination.KeepAbsolute)
                _pathToDisplay = fullPath;
            else
                _pathToDisplay = PathHelper.GetRelativePath(_source.Path, fullPath);
            OnPropertyChanged(nameof(PathToDisplay));
        }

        public string PathToDisplay
        {
            get { return _pathToDisplay; }
            set
            {
                _pathToDisplay = value;
                if (PathType == PATH_TYPE_ENUM.Source)
                    FullPath = _pathToDisplay;
                else
                    FullPath = PathHelper.GetFullPath(_source.Path, _pathToDisplay);
                OnPropertyChanged(nameof(PathToDisplay));
            }
        }

        private string _fullPath;

        public string FullPath
        {
            get
            {
                if (PathType == PATH_TYPE_ENUM.Source)
                    return _source.Path;
                else
                    return _destination.Path;
            }
            set
            {
                _fullPath = value;
                if (PathType == PATH_TYPE_ENUM.Source)
                    _source.Path = _fullPath;
                else
                    _destination.Path = _fullPath;
                OnPropertyChanged(nameof(FullPath));
            }
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
