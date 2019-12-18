using PicPick.Core;
using PicPick.Models;
using PicPick.ViewModel.UserControls.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.ViewModel.UserControls
{
    public class MappingBaseViewModel
    {
        public MappingBaseViewModel(PicPickProjectActivity activity)
        {
            // Source Pane
            PicPickProjectActivitySource source = activity.Source;
            string subFolders = source.IncludeSubFolders ? "including sub-folders" : "not including sub-folders";
            SourceDisplay = $"{source.Path} ({source.Filter}), {subFolders}";
            SourceFoundFiles = $"{source.FileList.Count} files found";

            // Destinations Pane
            var lookup = activity.FileMapping.DestinationFolders.Values.ToLookup(df => df.BasedOnDestination);
            DestinationList = lookup.Select(item => new MappingDestinationViewModel(item.Key, item.ToList(), activity)).ToList();
        }

        public List<MappingDestinationViewModel> DestinationList { get; set; }

        public string SourceDisplay { get; set; }
        public string SourceFoundFiles { get; set; }
    }

    public class MappingPlanViewModel : MappingBaseViewModel
    {
        public MappingPlanViewModel(PicPickProjectActivity activity) : base(activity)
        {
            // Source Pane
            SourceFilesDeleteWarning = activity.DeleteSourceFiles ? "The files will be deleted!" : "The files won't be deleted.";
        }

        public string SourceFilesDeleteWarning { get; set; }
    }

    public class MappingResultsViewModel : MappingBaseViewModel
    {
        public MappingResultsViewModel(PicPickProjectActivity activity) : base(activity)
        {
            int processedCount = activity.FileMapping.SourceFiles.Values.Where(f => f.Status != FILE_STATUS.NONE).Count();
            int totalCount = activity.FileMapping.SourceFiles.Count;
            ProcessedFiles = $"Processed files: {processedCount}/{totalCount}";
        }

        public string ProcessedFiles { get; private set; }
    }
}
