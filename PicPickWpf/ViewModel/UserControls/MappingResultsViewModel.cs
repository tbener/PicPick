﻿using PicPick.Core;
using PicPick.Models;
using PicPick.ViewModel.UserControls.Mapping;
using System.Linq;

namespace PicPick.ViewModel.UserControls
{
    public class MappingResultsViewModel : MappingBaseViewModel
    {
        public MappingResultsViewModel(PicPickProjectActivity activity) : base(activity)
        {
            int processedCount = activity.FilesGraph.Files.Values.Where(f => f.Status != FILE_STATUS.NONE).Count();
            int totalCount = activity.FilesGraph.Files.Count;
            ProcessedFiles = $"Processed files: {processedCount}/{totalCount}";
        }

        public string ProcessedFiles { get; private set; }
    }
}