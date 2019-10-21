using System;
using System.IO;
using System.Windows.Input;
using PicPick.Commands;
using PicPick.Core;

namespace PicPick.ViewModel
{
    public class FileExistsDialogViewModel : BaseViewModel, IDisposable
    {
        //public ICommand SetResponseCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public FileExistsDialogViewModel(string sourceFile, string destinationFolder)
        {
            FileName = Path.GetFileName(sourceFile);
            DestinationFile = Path.Combine(destinationFolder, FileName);

            CancelCommand = new RelayCommand(CancelOperation);
            ICommand SetResponseCommand = new RelayCommand<FileExistsResponseEnum>(SetResponse);

            ActionButtonsViewModels = new[]
            {
                new ActionButtonViewModel(FileExistsResponseEnum.OVERWRITE, sourceFile, SetResponseCommand),
                new ActionButtonViewModel(FileExistsResponseEnum.SKIP, DestinationFile, SetResponseCommand),
                new ActionButtonViewModel(FileExistsResponseEnum.RENAME, SetResponseCommand)
            };

        }

        public void CancelOperation()
        {
            Cancel = true;
            CloseDialog();
        }

        public void SetResponse(FileExistsResponseEnum action)
        {
            Response = action;
            CloseDialog();
        }
        internal void Refresh()
        {
            OnPropertyChanged("FileName");
            OnPropertyChanged("DestinationFile");
            OnPropertyChanged("DontAskAgain");
            OnPropertyChanged("ActionButtonsViewModels");
        }

        public void Dispose()
        {
            foreach (var item in ActionButtonsViewModels)
            {
                item.Dispose();
            }
            ActionButtonsViewModels = null;
            CancelCommand = null;
            CloseDialog = null;
        }

        #region Properties

        public string FileName { get; set; }
        public string DestinationFile { get; set; }
        public bool DontAskAgain { get; set; }
        public FileExistsResponseEnum Response { get; internal set; }
        public ActionButtonViewModel[] ActionButtonsViewModels { get; set; }
        public Action CloseDialog { get; set; }
        public bool Cancel { get; set; }


        #endregion
    }
}
