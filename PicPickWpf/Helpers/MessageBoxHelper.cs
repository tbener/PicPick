using PicPick.View.Dialogs;
using PicPick.ViewModel.Dialogs;
using PicPick.ViewModel.UserControls.Mapping;
using System;
using System.Windows;

namespace PicPick.Helpers
{
    public class MessageBoxHelper
    {
        
        
        private static MessageViewModel DisplayMessage(string messageText, string caption, MessageBoxButton button, MessageBoxImage messageIcon, bool displayDontShowAgain)
        {
            MessageViewModel messageViewModel = new MessageViewModel(messageText, caption, button, messageIcon, displayDontShowAgain);
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

        private static MessageBoxResult Show(string text, string caption, MessageBoxButton button, MessageBoxImage icon, bool displayDontShowAgain, out bool dontShowAgainValue)
        {
            MessageViewModel messageViewModel = DisplayMessage(text, caption, button, icon, displayDontShowAgain);
            dontShowAgainValue = messageViewModel.DontShowAgain;
            return messageViewModel.DialogResult;
        }

        /// <summary>
        /// Display a message with selected buttons and icon, without Dont Show Again checkbox.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        internal static MessageBoxResult Show(string text, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            var vm = DisplayMessage(text, caption, button, icon, false);
            return vm.DialogResult;
        }

        /// <summary>
        /// Display an error message with OK button
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="caption"></param>
        public static void Show(Exception ex, string caption = "Error")
        {
            Show(ex.Message + " (check the log for more details)", caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display a message with Dont Show Again check box
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="dontShowAgain"></param>
        /// <returns></returns>
        internal static MessageBoxResult Show(string text, string caption, MessageBoxButton button, MessageBoxImage icon, out bool dontShowAgain)
        {
            return Show(text, caption, button, icon, true, out dontShowAgain);
        }

        /// <summary>
        /// Display an info message with OK button
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        internal static void Show(string text, string caption)
        {
            Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        internal static MessageBoxResult Show(MappingBaseViewModel vm, string caption, MessageBoxButton button, bool displayDontShowAgain, out bool dontShowAgain)
        {
            MessageViewModel viewModel = new MessageViewModel(vm, caption, button);
            viewModel.ShowDontShowAgain = displayDontShowAgain ? Visibility.Visible : Visibility.Hidden;
            MessageView messageView = new MessageView();
            messageView.Height = 450;
            messageView.DataContext = viewModel;

            viewModel.CloseDialog = () =>
            {
                messageView.Close();
            };

            messageView.Owner = System.Windows.Application.Current.MainWindow;
            messageView.ShowDialog();

            dontShowAgain = viewModel.DontShowAgain;
            return viewModel.DialogResult;
        }
    }
}
