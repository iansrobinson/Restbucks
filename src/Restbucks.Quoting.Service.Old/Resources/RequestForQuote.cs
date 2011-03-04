﻿using System;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [UriTemplate("request-for-quote", "/")]
    public class RequestForQuote
    {
        private readonly UriFactory uriFactory;

        public RequestForQuote(UriFactory uriFactory)
        {
            this.uriFactory = uriFactory;
        }

        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            var baseUri = uriFactory.CreateBaseUri<RequestForQuote>(request.Uri);

            response.Headers.CacheControl = new CacheControl {Public = true, MaxAge = new TimeSpan(24, 0, 0)};
            return new ShopBuilder(baseUri)
                .AddForm(new Form(FormSemantics.Rfq,
                                  uriFactory.CreateRelativeUri<Quotes>(),
                                  "post", 
                                  RestbucksMediaType.Value,
                                  new Uri("http://schemas.restbucks.com/shop")))
                .Build();
        }
    }
}