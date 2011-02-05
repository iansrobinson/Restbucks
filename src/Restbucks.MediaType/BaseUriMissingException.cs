using System;
using System.Runtime.Serialization;

namespace Restbucks.MediaType
{
    public class BaseUriMissingException : Exception
    {
        public BaseUriMissingException()
        {
        }

        public BaseUriMissingException(string message) : base(message)
        {
        }

        public BaseUriMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BaseUriMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}