using System;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [UriTemplate("new-shop", "/")]
    public class NewEntryPoint
    {
        private readonly UriFactory uriFactory;

        public NewEntryPoint(UriFactory uriFactory)
        {
            this.uriFactory = uriFactory;
        }

        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControl {Public = true, MaxAge = new TimeSpan(24, 0, 0)};

            var baseUri = uriFactory.CreateBaseUri<NewEntryPoint>(request.Uri);

            response.Headers.CacheControl = new CacheControl { Public = true, MaxAge = new TimeSpan(24, 0, 0) };
            return new ShopBuilder(baseUri)
                .AddLink(new Link(uriFactory.CreateRelativeUri<Search>(), "application/opensearchdescription+xml", new StringLinkRelation("search")))
                .AddForm(new Form(FormSemantics.Rfq,
                                  uriFactory.CreateRelativeUri<Quotes>(),
                                  "post",
                                  RestbucksMediaType.Value,
                                  new Uri("http://schemas.restbucks.com/shop")))
                .Build();
        }
    }
}