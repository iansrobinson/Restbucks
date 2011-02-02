using System;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;

namespace Restbucks.Quoting.Service.Old.Resources
{
    public class EntryPoint
    {
        public static readonly UriFactory UriFactory = new UriFactory("shop");
        
        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControl {Public = true, MaxAge = new TimeSpan(24, 0, 0)};
            
            return new Shop(request.Uri)
                .AddLink(new Link(RequestForQuote.UriFactory.CreateRelativeUri(), "application/restbucks+xml", LinkRelations.Rfq, LinkRelations.Prefetch));
        }
    }
}