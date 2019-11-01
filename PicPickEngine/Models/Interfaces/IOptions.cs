using PicPick.Core;

namespace PicPick.Models.Interfaces
{
    public interface IOptions
    {
        FileExistsResponseEnum FileExistsResponse { get; set; }
    }
}