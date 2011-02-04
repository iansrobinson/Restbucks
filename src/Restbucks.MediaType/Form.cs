using System;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.MediaType
{
    public class Form
    {
        private readonly Uri resource;
        private readonly string method;
        private readonly string mediaType;
        private readonly Uri schema;
        private readonly Shop instance;

        public Form(Uri resource, string method, string mediaType, Uri schema, Shop instance)
        {
            Check.IsNotNull(resource, "resource");
            CheckString.Is(Not.NullOrEmptyOrWhitespace, method, "method");
            CheckString.Is(Not.NullOrEmptyOrWhitespace, mediaType, "mediaType");

            this.resource = resource;
            this.method = method;
            this.mediaType = mediaType;
            this.schema = schema;
            this.instance = instance;
        }

        public Form(Uri resource, string method, string mediaType, Shop instance) : this(resource, method, mediaType, null, instance)
        {
        }

        public Form(Uri resource, string method, string mediaType, Uri schema) : this(resource, method, mediaType, schema, null)
        {
        }

        public Uri Resource
        {
            get { return resource; }
        }

        public string Method
        {
            get { return method; }
        }

        public string MediaType
        {
            get { return mediaType; }
        }

        public Uri Schema
        {
            get { return schema; }
        }

        public Shop Instance
        {
            get { return instance; }
        }
    }
}