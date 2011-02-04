using System;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [NewUriTemplate("request-for-quote")]
    public class RequestForQuote
    {
        private readonly NewUriFactory newUriFactory;

        public RequestForQuote(NewUriFactory newUriFactory)
        {
            this.newUriFactory = newUriFactory;
        }

        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            var baseUri = newUriFactory.CreateBaseUri<RequestForQuote>(request.Uri);
            
            response.Headers.CacheControl = new CacheControl {Public = true, MaxAge = new TimeSpan(24, 0, 0)};
            return new Shop(baseUri)
                .AddForm(new Form(
                             newUriFactory.CreateRelativeUri<Quotes>(),
                             "post", "application/restbucks+xml",
                             new Uri("http://schemas.restbucks.com/shop.xsd")));
        }
    }
}