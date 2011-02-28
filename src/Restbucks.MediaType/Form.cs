using System;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.MediaType
{
    public class Form
    {
        private readonly string id;
        private readonly Uri resource;
        private readonly string method;
        private readonly string mediaType;
        private readonly Uri schema;
        private readonly Shop instance;

        public Form(string id, Uri resource, string method, string mediaType, Uri schema, Shop instance)
        {
            CheckString.Is(Not.NullOrEmptyOrWhitespace, id, "id");
            Check.IsNotNull(resource, "resource");
            CheckString.Is(Not.NullOrEmptyOrWhitespace, method, "method");
            CheckString.Is(Not.NullOrEmptyOrWhitespace, mediaType, "mediaType");

            this.id = id;
            this.resource = resource;
            this.method = method;
            this.mediaType = mediaType;
            this.schema = schema;
            this.instance = instance;
        }

        public Form(string id, Uri resource, string method, string mediaType, Shop instance) : this(id, resource, method, mediaType, null, instance)
        {
        }

        public Form(string id, Uri resource, string method, string mediaType, Uri schema) : this(id, resource, method, mediaType, schema, null)
        {
        }

        public string Id
        {
            get { return id; }
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