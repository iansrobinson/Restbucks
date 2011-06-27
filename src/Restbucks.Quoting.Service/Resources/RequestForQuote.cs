using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Resources.Hypermedia;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("request-for-quote")]
    public class RequestForQuote
    {
        private readonly UriFactory uriFactory;

        public RequestForQuote(UriFactory uriFactory)
        {
            this.uriFactory = uriFactory;
        }

        [WebGet]
        public HttpResponseMessage<Shop> Get(HttpRequestMessage request)
        {
            var body = new ShopBuilder(uriFactory.CreateBaseUri<RequestForQuote>(request.RequestUri))
                .AddForm(new Form(FormSemantics.Rfq,
                                  uriFactory.CreateRelativeUri<Quotes>(),
                                  "post", RestbucksMediaType.ContentType.MediaType,
                                  new Uri("http://schemas.restbucks.com/shop")))
                .Build();

            var response = new HttpResponseMessage<Shop>(body);
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true, MaxAge = new TimeSpan(24, 0, 0)};

            return response;
        }
    }
}