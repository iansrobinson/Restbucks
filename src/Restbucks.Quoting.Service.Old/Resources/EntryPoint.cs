using System;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;
using Restbucks.RestToolkit;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [UriTemplate("shop")]
    public class EntryPoint
    {
        private readonly UriFactory uriFactory;

        public EntryPoint(UriFactory uriFactory)
        {
            this.uriFactory = uriFactory;
        }

        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControl {Public = true, MaxAge = new TimeSpan(24, 0, 0)};
            
            return new Shop(uriFactory.CreateBaseUri<EntryPoint>(request.Uri))
                .AddLink(new Link(uriFactory.CreateRelativeUri<RequestForQuote>(), RestbucksMediaType.Value, LinkRelations.Rfq, LinkRelations.Prefetch));
        }
    }
}