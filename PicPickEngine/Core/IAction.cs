using PicPick.Helpers;
using PicPick.Models.Interfaces;
using System.Threading.Tasks;

namespace PicPick.Core
{
    public interface IAction
    {
        IActivity Activity { get; }

        //Task ComputeAsync(ProgressInformation progressInfo, int levelUpdate = 0);
    }
}