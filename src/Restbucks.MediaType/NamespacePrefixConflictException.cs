using System;
using System.Runtime.Serialization;

namespace Restbucks.MediaType
{
    public class NamespacePrefixConflictException : Exception
    {
        public NamespacePrefixConflictException()
        {
        }

        public NamespacePrefixConflictException(string message) : base(message)
        {
        }

        public NamespacePrefixConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NamespacePrefixConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}