using System;

namespace PicPick.Models.IsDirtySupport
{
    public class IsDirtyException : Exception
    {
        public IsDirtyException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
