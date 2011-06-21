using System;
using System.Runtime.Serialization;

namespace Restbucks.NewClient.RulesEngine
{
    public class ControlNotFoundException : Exception
    {
        public ControlNotFoundException()
        {
        }

        public ControlNotFoundException(string message) : base(message)
        {
        }

        public ControlNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ControlNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}