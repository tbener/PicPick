using PicPick.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace PicPick.ViewModel.Dialogs
{
    public class MessageViewModel
    {
        public ICommand SetResultCommand { get; set; }

        private MessageBoxButton _messageBoxButtons = MessageBoxButton.OK;

        public MessageViewModel(string messageText, string caption, MessageBoxButton button, MessageBoxImage icon = MessageBoxImage.Information, bool showDontShowAgain = false)
        {
            SetResultCommand = new RelayCommand<string>(SetDialogResult);

            Text = messageText;
            Caption = caption;
            _messageBoxButtons = button;
            MessageIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                              SystemIcons.Information.Handle,
                              System.Windows.Int32Rect.Empty,
                              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

        }

        private void SetDialogResult(string result)
        {
            MessageBoxResult messageBoxResult;
            if (Enum.TryParse<MessageBoxResult>(result, out messageBoxResult))
                DialogResult = messageBoxResult;
            CloseDialog();
        }

        private Visibility ButtonVisibility(string button)
        {
            if (_messageBoxButtons.ToString().Contains(button))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public string Caption { get; set; }
        public string Text { get; set; }

        public ImageSource MessageIcon { get; set; }

        public Visibility CancelButtonVisibility => ButtonVisibility("Cancel");
        public Visibility OkButtonVisibility => ButtonVisibility("OK");
        public Visibility YesButtonVisibility => ButtonVisibility("Yes");
        public Visibility NoButtonVisibility => ButtonVisibility("No");

        public MessageBoxResult DialogResult { get; set; }
        public Action CloseDialog { get; set; }
    }
}
