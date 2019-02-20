using PicPick.Helpers;
using System.Threading;

namespace PicPick.Interfaces
{
    public interface IFileExistsDialog
    {
        void ShowDialog(string fileName, string imageSource, string imageDest);

        bool DontAskAgain { get; set; }

        FILE_EXISTS_RESPONSE SelectedAction { get; set; }

        CancellationTokenSource CancellationTokenSource { get; set; }
    }
}
