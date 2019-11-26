using PicPick.View.Dialogs;
using PicPick.ViewModel.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace PicPick.Helpers
{
    public class MessageBoxHelper
    {
        
        
        private static MessageViewModel Show(string messageText, string caption = "PicPick", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage messageIcon = MessageBoxImage.Information, bool showDontShowAgain = false)
        {
            MessageViewModel messageViewModel = new MessageViewModel(messageText, caption, button, messageIcon, showDontShowAgain);
            MessageView messageView = new MessageView();
            messageView.DataContext = messageViewModel;

            messageViewModel.CloseDialog = () =>
            {
                messageView.Close();
            };

            messageView.Owner = System.Windows.Application.Current.MainWindow;
            messageView.ShowDialog();

            return messageViewModel;
        }

        public static MessageBoxResult Show(string text, string caption, MessageBoxButton button, MessageBoxImage icon, bool showDontShowAgain, out bool dontShowAgainValue)
        {
            MessageViewModel messageViewModel = Show(text, caption, button, icon, showDontShowAgain);
            dontShowAgainValue = messageViewModel.DontShowAgain;
            return messageViewModel.DialogResult;
        }

        public static void ShowError(Exception ex, string caption = "PicPick Error")
        {
            Show(ex.Message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal static MessageBoxResult Show(string text, string caption, MessageBoxButton button, MessageBoxImage icon, out bool dontShowAgain)
        {
            return Show(text, caption, button, icon, true, out dontShowAgain);
        }

        internal static void Show(string text, string caption)
        {
            Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        internal static MessageBoxResult Show(string text, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            var vm = Show(text, caption, button, icon, false);
            return vm.DialogResult;
        }


        // public static MessageBoxResult Show(string messageText, string caption, MessageBoxButton button, MessageBoxImage messageIcon, bool showDontShowAgain, out bool dontShowAgainValue)
    }
}
