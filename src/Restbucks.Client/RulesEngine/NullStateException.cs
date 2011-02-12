using System;
using System.Runtime.Serialization;

namespace Restbucks.Client.RulesEngine
{
    public class NullStateException : Exception
    {
        public NullStateException()
        {
        }

        public NullStateException(string message) : base(message)
        {
        }

        public NullStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NullStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}