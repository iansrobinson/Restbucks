﻿using System;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.RestToolkit
{
    public class UriFactoryWorker
    {
        private readonly string routePrefix;
        private readonly UriTemplate uriTemplate;
        private readonly Uri dummyBaseAddress;

        private static readonly Uri Localhost = new Uri("http://localhost");

        public UriFactoryWorker(string routePrefix) : this(routePrefix, string.Empty)
        {
        }

        public UriFactoryWorker(string routePrefix, string uriTemplateValue)
        {
            CheckString.Is(Not.NullOrEmptyOrWhitespace, routePrefix, "routePrefix");
            CheckString.Is(Not.Null | Not.Whitespace, uriTemplateValue, "uriTemplateValue");
            
            this.routePrefix = routePrefix;
            uriTemplate = new UriTemplate(uriTemplateValue, !(uriTemplateValue.StartsWith("/") || uriTemplateValue.EndsWith("/")));

            dummyBaseAddress = new Uri(Localhost, routePrefix);
        }

        public string RoutePrefix
        {
            get { return routePrefix; }
        }

        public string UriTemplateValue
        {
            get { return uriTemplate.ToString(); }
        }

        public Uri CreateRelativeUri(params string[] values)
        {
            return new Uri(uriTemplate.BindByPosition(dummyBaseAddress, values).PathAndQuery, UriKind.RelativeOrAbsolute);
        }

        public Uri CreateAbsoluteUri(Uri baseUri, params string[] values)
        {
            return uriTemplate.BindByPosition(new Uri(baseUri, routePrefix), values);
        }

        public Uri CreateBaseUri(Uri uri)
        {
            var index = uri.ToString().IndexOf(routePrefix, StringComparison.InvariantCultureIgnoreCase);
            if (index < 0)
            {
                throw new ArgumentException(string.Format("Supplied URI does not contain route prefix. Uri: [{0}], Route prefix: [{1}].", uri, routePrefix));
            }
            return new Uri(uri.ToString().Substring(0, index), UriKind.Absolute);
        }
    }
}