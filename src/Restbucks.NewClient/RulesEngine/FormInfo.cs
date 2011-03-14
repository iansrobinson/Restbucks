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
        private readonly IFormDataStrategy dataStrategy;
        
        public FormInfo(Uri resourceUri, HttpMethod method, MediaTypeHeaderValue contentType, IFormDataStrategy dataStrategy)
        {
            this.resourceUri = resourceUri;
            this.method = method;
            this.contentType = contentType;
            this.dataStrategy = dataStrategy;
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

        public IFormDataStrategy DataStrategy
        {
            get { return dataStrategy; }
        }
    }
}