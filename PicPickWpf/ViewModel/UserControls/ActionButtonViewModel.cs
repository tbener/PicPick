using PicPick.Core;
using System;
using System.Windows;
using System.Windows.Input;

namespace PicPick.ViewModel.UserControls
{
    public class ActionButtonViewModel : BaseViewModel, IDisposable
    {
        #region Private members

        private FileExistsResponseEnum _action;

        public ICommand SetResponseCommand { get; set; }

        #endregion

        #region CTOR

        public ActionButtonViewModel(FileExistsResponseEnum action, ICommand command)
        {
            Action = action;
            ImageInfoVisibility = Visibility.Collapsed;
            SetResponseCommand = command;
        }
        public ActionButtonViewModel(FileExistsResponseEnum action, string imagePath, ICommand command)
        {
            Action = action;
            ImageInfoVisibility = Visibility.Visible;
            ImageInfoViewModel = new ImageInfoViewModel(imagePath);
            SetResponseCommand = command;
        }

        #endregion

        #region Properties

        public FileExistsResponseEnum Action
        {
            get => _action;
            set
            {
                _action = value;
                var actionProperties = FileExistsResponseAttribute.GetAttribute(_action);
                ActionText = actionProperties.Description;
                ActionDetails = actionProperties.Details;
            }
        }


        public string ActionText { get; set; }

        public string ActionDetails { get; set; }

        public Visibility ImageInfoVisibility { get; set; }

        public ImageInfoViewModel ImageInfoViewModel { get; set; }

        #endregion

        public void Dispose()
        {
            ImageInfoViewModel?.Dispose();
            ImageInfoViewModel = null;
            //throw new NotImplementedException();
        }
    }
}
