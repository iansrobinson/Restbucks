using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Restbucks.MediaType;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("shop")]
    public class EntryPoint
    {
        private readonly UriFactoryCollection uriFactories;

        public EntryPoint(UriFactoryCollection uriFactories)
        {
            this.uriFactories = uriFactories;
        }

        [WebGet(UriTemplate = "")]
        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true, MaxAge = new TimeSpan(24, 0, 0)};
            
            return new Shop(request.RequestUri)
                .AddLink(new Link(uriFactories.For<RequestForQuote>().CreateRelativeUri(), "application/restbucks+xml", LinkRelations.Rfq, LinkRelations.Prefetch));
        }
    }
}