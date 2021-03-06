﻿using PicPick.Commands;
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

        private readonly MessageBoxButton _messageBoxButtons = MessageBoxButton.OK;

        public MessageViewModel(object contentViewModel, string caption, MessageBoxButton button)
        {
            SetResultCommand = new RelayCommand<string>(CloseDialogWithResult);

            CustomContent = contentViewModel;
            Caption = caption;
            _messageBoxButtons = button;
            DialogResult = MessageBoxResult.Cancel;
            ShowDontShowAgain = Visibility.Visible;
        }

        public MessageViewModel(string messageText, string caption, MessageBoxButton button, MessageBoxImage messageIcon = MessageBoxImage.Information, bool showDontShowAgain = false)
        {
            SetResultCommand = new RelayCommand<string>(CloseDialogWithResult);

            Text = messageText;
            Caption = caption;
            _messageBoxButtons = button;
            DialogResult = MessageBoxResult.Cancel;
            Icon icon = GetSystemIcon(messageIcon.ToString());
            MessageIcon = Imaging.CreateBitmapSourceFromHIcon(
                              icon.Handle,
                              Int32Rect.Empty,
                              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            ShowDontShowAgain = showDontShowAgain ? Visibility.Visible : Visibility.Hidden;
        }

        private Icon GetSystemIcon(string icon = "Information")
        {
            try
            {
                return (Icon)typeof(SystemIcons).GetProperty(icon).GetValue(null);
            }
            catch
            {
                return SystemIcons.Information;
            }
        }

        private void CloseDialogWithResult(string result)
        {
            if (Enum.TryParse(result, out MessageBoxResult messageBoxResult))
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
        public Visibility ShowDontShowAgain { get; set; }

        public bool DontShowAgain { get; set; }

        public MessageBoxResult DialogResult { get; set; }
        public Action CloseDialog { get; set; }

        public object CustomContent { get; set; }
    }
}
