using PicPick.Core;
using PicPick.Models;
using PicPick.Models.Interfaces;
using PicPick.ViewModel.UserControls.Mapping;
using System.Linq;

namespace PicPick.ViewModel.UserControls
{
    public class MappingResultsViewModel : MappingBaseViewModel
    {
        private enum ErrorLevel
        {
            Success,
            Warning,
            Error
        }

        private ErrorLevel _errorLevel = ErrorLevel.Success;

        public MappingResultsViewModel(IActivity activity) : base(activity, null)
        {
            int processedCount = activity.FileGraph.Files.Where(f => f.Status != FILE_STATUS.NONE).Count();
            int totalCount = activity.FileGraph.Files.Count;
            ProcessedFiles = $"Processed files: {processedCount}/{totalCount}";

            _errorLevel = processedCount == totalCount ? ErrorLevel.Success : ErrorLevel.Warning;
        }

        public string ProcessedFiles { get; private set; }

        public string Icon
        {
            get
            {
                switch (_errorLevel)
                {
                    case ErrorLevel.Warning:
                        return "/PicPickUI;component/Resources/warning.ico";
                    case ErrorLevel.Error:
                        return "/PicPickUI;component/Resources/error.ico";
                    case ErrorLevel.Success:
                    default:
                        return "/PicPickUI;component/Resources/success.ico";
                }
                
            }
        }
    }
}
