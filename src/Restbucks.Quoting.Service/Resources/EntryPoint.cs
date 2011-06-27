using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Resources.Hypermedia;
using RestInPractice.RestToolkit.Hypermedia;

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

        [WebGet]
        public HttpResponseMessage<Shop> Get(HttpRequestMessage request)
        {
            var body = new ShopBuilder(uriFactory.CreateBaseUri<EntryPoint>(request.RequestUri))
                .AddLink(new Link(uriFactory.CreateRelativeUri<RequestForQuote>(), RestbucksMediaType.Value, LinkRelations.Rfq, LinkRelations.Prefetch))
                .Build();

            var response = new HttpResponseMessage<Shop>(body);
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true, MaxAge = new TimeSpan(24, 0, 0)};

            return response;
        }
    }
}