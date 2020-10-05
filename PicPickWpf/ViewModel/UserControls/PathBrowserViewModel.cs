using PicPick.Commands;
using PicPick.Models;
using System;
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


    public class PathAdapter
    {
        public event EventHandler OnAnyPathChanged;

        private readonly PicPickProjectActivityDestination _destination;
        private readonly PicPickProjectActivitySource _source;

        public PathAdapter(PicPickProjectActivitySource source, PicPickProjectActivityDestination destination)
        {
            _source = source;
            _destination = destination;

            _source.PropertyChanged += Any_PropertyChanged;
            if (_destination != null)
                _destination.PropertyChanged += Any_PropertyChanged;
        }

        public PathAdapter(PicPickProjectActivitySource source) : this(source, null)
        { }

        public string Path
        {
            get
            {
                return _destination == null ?
                  _source.Path :
                  _destination.Path;
            }
            set
            {
                if (_destination == null)
                    _source.Path = value;
                else
                    _destination.Path = value;
            }
        }

        public string GetFullNormalizedPath()
        {
            return _destination == null ? _source.Path : _destination.GetFullPath();
        }

        private void Any_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Path")
            {
                OnAnyPathChanged?.Invoke(sender, e);
            }
        }
    }

    public class PathBrowserViewModel : BaseViewModel
    {
        public ICommand BrowseCommand { get; set; }
        public ICommand OpenExplorerCommand { get; set; }

        private PathAdapter _pathAdapter;
        private string _textboxTooltip;

        private PathBrowserViewModel()
        {
            BrowseCommand = new RelayCommand(BrowseFolder);
            OpenExplorerCommand = new RelayCommand(OpenInExplorer);
        }

        public PathBrowserViewModel(PathAdapter pathAdapter) : this()
        {
            _pathAdapter = pathAdapter;
            _pathAdapter.OnAnyPathChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(PathPreviewLink));
                OnPropertyChanged(nameof(LinkTooltip));
            };
        }

        public string Path
        {
            get => _pathAdapter.Path;
            set
            {
                _pathAdapter.Path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        public string PathPreviewLink
        {
            get
            {
                return _pathAdapter.GetFullNormalizedPath();
            }
        }

        public string LinkTooltip
        {
            get => $"{_pathAdapter.GetFullNormalizedPath()} (Click to open in Explorer)";
        }

        public string TextboxTooltip
        {
            get => _textboxTooltip;
            set
            {
                _textboxTooltip = value;
                OnPropertyChanged(nameof(TextboxTooltip));
            }
        }

        
        private void BrowseFolder()
        {
            var dlg = new FolderBrowser2();
            dlg.DirectoryPath = _pathAdapter.GetFullNormalizedPath();
            if (dlg.ShowDialog(null) == DialogResult.OK)
            {
                Path = dlg.DirectoryPath;
            }
        }

        private void OpenInExplorer()
        {
            string path = _pathAdapter.GetFullNormalizedPath();
            if (PathHelper.Exists(path))
                ExplorerHelper.OpenPath(path);
            else
                Msg.Show($"{path} doesn't exist");
        }
    }
}
