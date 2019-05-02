using System;

namespace PicPick.Project.IsDirtySupport
{
    public class IsDirtyException : Exception
    {
        public IsDirtyException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
