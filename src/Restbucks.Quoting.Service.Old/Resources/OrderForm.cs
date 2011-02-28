using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Old.Adapters;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [UriTemplate("order-form", "{id}")]
    public class OrderForm
    {
        private static readonly UriFactoryWorker OrdersUriFactoryWorker = new UriFactoryWorker("orders", string.Format("/?c=12345&s={0}", SignedFormPlaceholder));

        public const string SignedFormPlaceholder = "SIGNED_FORM_PLACEHOLDER";

        private readonly UriFactory uriFactory;
        private readonly IQuotationEngine quoteEngine;

        public OrderForm(UriFactory uriFactory, IQuotationEngine quoteEngine)
        {
            this.uriFactory = uriFactory;
            this.quoteEngine = quoteEngine;
        }

        public Shop Get(string id, HttpRequestMessage request, HttpResponseMessage response)
        {
            Quotation quote;
            try
            {
                quote = quoteEngine.GetQuote(new Guid(id));
            }
            catch (KeyNotFoundException)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return null;
            }

            var baseUri = uriFactory.CreateBaseUri<OrderForm>(request.Uri);

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.CacheControl = new CacheControl {Public = true};
            response.Headers.Expires = quote.CreatedDateTime.AddDays(7.0).UtcDateTime;

            return new ShopBuilder(baseUri)
                .AddForm(new Form(FormSemantics.Order,
                                  OrdersUriFactoryWorker.CreateAbsoluteUri(new Uri("http://localhost:8081/")),
                                  "post",
                                  RestbucksMediaType.Value,
                                  new ShopBuilder(baseUri)
                                      .AddItems(quote.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                                      .AddLink(new Link(uriFactory.CreateRelativeUri<Quote>(quote.Id), RestbucksMediaType.Value, LinkRelations.Self))
                                      .Build()))
                .Build();
        }
    }
}