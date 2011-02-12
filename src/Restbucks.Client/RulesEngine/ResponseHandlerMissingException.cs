using System;
using System.Runtime.Serialization;

namespace Restbucks.Client.RulesEngine
{
    public class ResponseHandlerMissingException : Exception
    {
        public ResponseHandlerMissingException()
        {
        }

        public ResponseHandlerMissingException(string message) : base(message)
        {
        }

        public ResponseHandlerMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ResponseHandlerMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}