using System;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [UriTemplate("request-for-quote")]
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
            return new Shop(baseUri)
                .AddForm(new Form(
                             uriFactory.CreateRelativeUri<Quotes>(),
                             "post", "application/restbucks+xml",
                             new Uri("http://schemas.restbucks.com/shop.xsd")));
        }
    }
}