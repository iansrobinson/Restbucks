using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Restbucks.NewClient.RulesEngine
{
    public class FormInfo
    {
        private readonly Uri resourceUri;
        private readonly HttpMethod method;
        private readonly MediaTypeHeaderValue contentType;
        private readonly HttpContent formData;

        public FormInfo(Uri resourceUri, HttpMethod method, MediaTypeHeaderValue contentType, HttpContent formData)
        {
            this.resourceUri = resourceUri;
            this.method = method;
            this.contentType = contentType;
            this.formData = formData;
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

        public HttpContent FormData
        {
            get { return formData; }
        }
    }
}