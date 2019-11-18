using MahApps.Metro.Controls;
using System;

namespace PicPick.View.Dialogs
{
    /// <summary>
    /// Interaction logic for FileExistsDialogView.xaml
    /// </summary>
    public partial class FileExistsDialogView : MetroWindow
    {
        public FileExistsDialogView()
        {
            InitializeComponent();
            Activated += FileExistsDialogView_Activated;
        }

        private void FileExistsDialogView_Activated(object sender, EventArgs e)
        {
            
        }
    }
}
