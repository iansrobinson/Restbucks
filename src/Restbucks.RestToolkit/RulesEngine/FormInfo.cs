using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class FormInfo
    {
        private readonly Uri resourceUri;
        private readonly HttpMethod method;
        private readonly MediaTypeHeaderValue contentType;
        
        public FormInfo(Uri resourceUri, HttpMethod method, MediaTypeHeaderValue contentType)
        {
            this.resourceUri = resourceUri;
            this.method = method;
            this.contentType = contentType;
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
    }
}