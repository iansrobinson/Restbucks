using System;
using System.Runtime.Serialization;

namespace Restbucks.NewClient.RulesEngine
{
    public class FormNotFoundException : Exception
    {
        public FormNotFoundException()
        {
        }

        public FormNotFoundException(string message) : base(message)
        {
        }

        public FormNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FormNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}