﻿using System;
using System.Runtime.Serialization;

namespace RestInPractice.RestToolkit.Hypermedia
{
    public class UriTemplateMissingException : Exception
    {
        public UriTemplateMissingException()
        {
        }

        public UriTemplateMissingException(string message) : base(message)
        {
        }

        public UriTemplateMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UriTemplateMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}