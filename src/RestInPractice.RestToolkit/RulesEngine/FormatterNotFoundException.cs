using System;
using System.Runtime.Serialization;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public class FormatterNotFoundException : Exception
    {
        public FormatterNotFoundException()
        {
        }

        public FormatterNotFoundException(string message) : base(message)
        {
        }

        public FormatterNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FormatterNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}