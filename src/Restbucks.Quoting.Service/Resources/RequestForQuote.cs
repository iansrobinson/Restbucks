using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Restbucks.MediaType;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("request-for-quote")]
    public class RequestForQuote
    {
        private readonly UriFactoryCollection uriFactories;

        public RequestForQuote(UriFactoryCollection uriFactories)
        {
            this.uriFactories = uriFactories;
        }

        [WebGet(UriTemplate = "")]
        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true, MaxAge = new TimeSpan(24, 0, 0)};
            return new Shop(uriFactories.For<RequestForQuote>().CreateBaseUri(request.RequestUri))
                .AddForm(new Form(
                             uriFactories.For<Quotes>().CreateRelativeUri(),
                             "post", "application/restbucks+xml",
                             new Uri("http://schemas.restbucks.com/shop.xsd")));
        }
    }
}