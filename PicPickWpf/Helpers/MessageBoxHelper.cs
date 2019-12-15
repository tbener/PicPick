using PicPick.View.Dialogs;
using PicPick.ViewModel.Dialogs;
using PicPick.ViewModel.UserControls;
using System;
using System.Windows;

namespace PicPick.Helpers
{
    public class MessageBoxHelper
    {
        
        
        private static MessageViewModel DisplayMessage(string messageText, string caption, MessageBoxButton button, MessageBoxImage messageIcon, bool showDontShowAgain)
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

        private static MessageBoxResult Show(string text, string caption, MessageBoxButton button, MessageBoxImage icon, bool showDontShowAgain, out bool dontShowAgainValue)
        {
            MessageViewModel messageViewModel = DisplayMessage(text, caption, button, icon, showDontShowAgain);
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
        public static void Show(Exception ex, string caption = "PicPick Error")
        {
            Show(ex.Message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
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

        internal static MessageBoxResult Show(MappingPlanViewModel vm, string caption, MessageBoxButton button, out bool dontShowAgain)
        {
            MessageViewModel viewModel = new MessageViewModel(vm, caption, button);
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
