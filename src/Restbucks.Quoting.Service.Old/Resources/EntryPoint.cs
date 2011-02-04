using System;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [NewUriTemplate("shop")]
    public class EntryPoint
    {
        private readonly NewUriFactory newUriFactory;

        public EntryPoint(NewUriFactory newUriFactory)
        {
            this.newUriFactory = newUriFactory;
        }

        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControl {Public = true, MaxAge = new TimeSpan(24, 0, 0)};
            
            return new Shop(request.Uri)
                .AddLink(new Link(newUriFactory.CreateRelativeUri <RequestForQuote>(), "application/restbucks+xml", LinkRelations.Rfq, LinkRelations.Prefetch));
        }
    }
}