using System;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;

namespace Restbucks.Quoting.Service.Old.Resources
{
    public class RequestForQuote
    {
        public static readonly UriFactory UriFactory = new UriFactory("request-for-quote");

        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControl {Public = true, MaxAge = new TimeSpan(24, 0, 0)};
            return new Shop(request.Uri)
                .AddForm(new Form(
                             Quotes.QuotesUriFactory.CreateRelativeUri(),
                             "post", "application/restbucks+xml",
                             new Uri("http://schemas.restbucks.com/shop.xsd")));
        }
    }
}