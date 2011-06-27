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
using Restbucks.Quoting.Service.Resources.Hypermedia;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("order-form", "{id}")]
    public class OrderForm
    {
        public const string SignedFormPlaceholder = "SIGNED_FORM_PLACEHOLDER";

        private static readonly UriFactoryWorker OrdersUriFactoryWorker = new UriFactoryWorker("orders", string.Format("?c=12345&s={0}", SignedFormPlaceholder));

        private readonly UriFactory uriFactory;
        private readonly IQuotationEngine quotationEngine;

        public OrderForm(UriFactory uriFactory, IQuotationEngine quotationEngine)
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
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var baseUri = uriFactory.CreateBaseUri<OrderForm>(request.RequestUri);

            var body = new ShopBuilder(baseUri)
                .AddForm(new Form(FormSemantics.Order,
                                  OrdersUriFactoryWorker.CreateAbsoluteUri(new Uri("http://localhost:8081")),
                                  "post",
                                  RestbucksMediaType.ContentType.MediaType,
                                  new ShopBuilder(baseUri).AddItems(quotation.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                                      .AddLink(new Link(uriFactory.CreateRelativeUri<Quote>(quotation.Id), RestbucksMediaType.ContentType.MediaType, LinkRelations.Self))
                                      .Build()))
                .Build();

            var response = new HttpResponseMessage<Shop>(body) {StatusCode = HttpStatusCode.OK};

            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true};
            response.Content.Headers.Expires = quotation.CreatedDateTime.AddDays(7.0);

            return response;
        }
    }
}