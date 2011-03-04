﻿using System;
using System.Net.Http.Headers;

namespace Restbucks.NewClient.RulesEngine
{
    public class LinkInfo
    {
        private readonly Uri resourceUri;
        private readonly MediaTypeHeaderValue contentType;

        public LinkInfo(Uri resourceUri, MediaTypeHeaderValue contentType)
        {
            this.resourceUri = resourceUri;
            this.contentType = contentType;
        }

        public Uri ResourceUri
        {
            get { return resourceUri; }
        }

        public MediaTypeHeaderValue ContentType
        {
            get { return contentType; }
        }
    }
}