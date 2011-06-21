using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Adapters;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("quote", "{id}")]
    public class Quote
    {
        private readonly UriFactory uriFactory;
        private readonly IQuotationEngine quotationEngine;

        public Quote(UriFactory uriFactory, IQuotationEngine quotationEngine)
        {
            this.uriFactory = uriFactory;
            this.quotationEngine = quotationEngine;
        }

        [WebGet]
        public HttpResponseMessage<Shop> Get(string id, HttpRequestMessage request)
        {
            Quotation quotation;
            try
            {
                quotation = quotationEngine.GetQuote(new Guid(id));
            }
            catch (KeyNotFoundException)
            {
                return new HttpResponseMessage<Shop>(HttpStatusCode.NotFound);
            }

            var baseUri = uriFactory.CreateBaseUri<Quote>(request.RequestUri);

            var body = new ShopBuilder(baseUri)
                .AddItems(quotation.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                .AddLink(new Link(uriFactory.CreateRelativeUri<Quote>(quotation.Id), RestbucksMediaType.Value, LinkRelations.Self))
                .AddLink(new Link(uriFactory.CreateRelativeUri<OrderForm>(quotation.Id), RestbucksMediaType.Value, LinkRelations.OrderForm))
                .Build();

            var response = new HttpResponseMessage<Shop>(body) {StatusCode = HttpStatusCode.OK};

            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true};
            response.Content.Headers.Expires = quotation.CreatedDateTime.AddDays(7.0);

            return response;
        }
    }
}