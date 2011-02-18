using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("shop")]
    public class EntryPoint
    {
        private readonly UriFactory uriFactory;

        public EntryPoint(UriFactory uriFactory)
        {
            this.uriFactory = uriFactory;
        }

        [WebGet(UriTemplate = "")]
        public Shop Get(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true, MaxAge = new TimeSpan(24, 0, 0)};

            return new Shop(uriFactory.CreateBaseUri<EntryPoint>(request.RequestUri))
                .AddLink(new Link(uriFactory.CreateRelativeUri<RequestForQuote>(), RestbucksMediaType.Value, LinkRelations.Rfq, LinkRelations.Prefetch));
        }
    }
}