using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Exceptions
{
    public class NoDestinationsException : Exception
    {
        public NoDestinationsException() : base("Destination(s) not provided for this activity.")
        {
        }

        public NoDestinationsException(string message)
        : base(message)
        {
        }

        public NoDestinationsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DestinationEqualsSourceException : Exception
    {
        public DestinationEqualsSourceException() : base("The destination equals to the source path without a template.")
        {
        }

        public DestinationEqualsSourceException(string message)
        : base(message)
        {
        }

        public DestinationEqualsSourceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class NoSourceException : Exception
    {
        public NoSourceException() : base("Source or source path not provided for this activity.")
        {
        }

        public NoSourceException(string message)
        : base(message)
        {
        }

        public NoSourceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class SourceDirectoryNotFoundException : DirectoryNotFoundException
    {
        public SourceDirectoryNotFoundException() : base("Source path not found.")
        {
        }

        public SourceDirectoryNotFoundException(string message)
        : base(message)
        {
        }

        public SourceDirectoryNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    
}
