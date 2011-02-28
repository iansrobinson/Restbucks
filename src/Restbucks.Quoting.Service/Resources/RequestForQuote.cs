﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("request-for-quote")]
    public class RequestForQuote
    {
        private readonly UriFactory uriFactory;

        public RequestForQuote(UriFactory uriFactory)
        {
            this.uriFactory = uriFactory;
        }

        [WebGet]
        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true, MaxAge = new TimeSpan(24, 0, 0)};
            return new ShopBuilder(uriFactory.CreateBaseUri<RequestForQuote>(request.RequestUri))
                .AddForm(new Form(FormSemantics.Rfq,
                                  uriFactory.CreateRelativeUri<Quotes>(),
                                  "post", RestbucksMediaType.Value,
                                  new Uri("http://schemas.restbucks.com/shop")))
                .Build();
        }
    }
}