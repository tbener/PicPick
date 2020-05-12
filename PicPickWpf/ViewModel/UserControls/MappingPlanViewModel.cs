using PicPick.Models;
using PicPick.ViewModel.UserControls.Mapping;

namespace PicPick.ViewModel.UserControls
{

    public class MappingPlanViewModel : MappingBaseViewModel
    {
        public MappingPlanViewModel(PicPickProjectActivity activity) : base(activity)
        {
            // Source Pane
            SourceFilesDeleteWarning = activity.DeleteSourceFiles ? "The files will be deleted!" : "The files won't be deleted.";
        }

        public string SourceFilesDeleteWarning { get; set; }
    }
}
