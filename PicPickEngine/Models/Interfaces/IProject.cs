using System.Collections.ObjectModel;

namespace PicPick.Models.Interfaces
{
    public interface IProject
    {
        ObservableCollection<PicPickProjectActivity> ActivityList { get; }
        string Name { get; set; }
        PicPickProject_options Options { get; set; }
        string ver { get; set; }
    }
}