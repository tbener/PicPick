﻿using PicPick.Core;
using PicPick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.ViewModel.UserControls.Mapping
{
    public class MappingFolderViewModel
    {
        public MappingFolderViewModel(DestinationFolder destinationFolder, bool isDone = false)
        {
            Folder = PathHelper.GetRelativePath(destinationFolder.BasedOnDestination.Path, destinationFolder.FullPath);
            FullPath = destinationFolder.FullPath;
            FilesCount = $"{destinationFolder.Files.Count} files";
            State = destinationFolder.IsNew ? "New" : "Exists";

            //Files = destinationFolder.Files.Select(f => new MappingFileViewModel(f)).ToList();

            //var g = destinationFolder.Files.GroupBy(f => f.Status, f => f, (key, grp) => new {status = key, files = grp.ToList() });

            List<MappingStatusViewModel> mappingStatuses = new List<MappingStatusViewModel>();
            ILookup<FILE_STATUS, DestinationFile> lookup = destinationFolder.Files.ToLookup(f => f.Status);
            foreach (var item in lookup)
            {
                mappingStatuses.Add(new MappingStatusViewModel(item.Key, item.ToList()));
            }
            SubItems = mappingStatuses;

            
        }

        public string Folder { get; set; }
        public string FullPath { get; set; }
        public string FilesCount { get; set; }
        public string State { get; set; }

        public List<MappingFileViewModel> Files { get; set; }

        public object SubItems { get; set; }
    }
}
