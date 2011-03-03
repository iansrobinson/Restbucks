using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Restbucks.NewClient
{
    public class FormInfo
    {
        private readonly Uri resourceUri;
        private readonly HttpMethod method;
        private readonly MediaTypeHeaderValue contentType;
        private readonly EntityTagHeaderValue etag;

        public FormInfo(Uri resourceUri, HttpMethod method, MediaTypeHeaderValue contentType) : this(resourceUri, method, contentType, null)
        {
        }

        public FormInfo(Uri resourceUri, HttpMethod method, MediaTypeHeaderValue contentType, EntityTagHeaderValue etag)
        {
            this.resourceUri = resourceUri;
            this.method = method;
            this.contentType = contentType;
            this.etag = etag;
        }

        public Uri ResourceUri
        {
            get { return resourceUri; }
        }

        public HttpMethod Method
        {
            get { return method; }
        }

        public MediaTypeHeaderValue ContentType
        {
            get { return contentType; }
        }

        public EntityTagHeaderValue Etag
        {
            get { return etag; }
        }

        public FormInfo WithNewEtag(EntityTagHeaderValue newEtag)
        {
            return new FormInfo(resourceUri, method, contentType, newEtag);
        }
    }
}