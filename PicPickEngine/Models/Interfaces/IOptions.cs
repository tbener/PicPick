using PicPick.Core;

namespace PicPick.Models
{
    public interface IOptions
    {
        FileExistsResponseEnum FileExistsResponse { get; set; }
    }
}