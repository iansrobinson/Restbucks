using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Old.Adapters;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [NewUriTemplate("order-form", "{id}")]
    public class OrderForm
    {
        private static readonly UriFactoryWorker OrdersUriFactoryWorker = new UriFactoryWorker("orders", string.Format("/?c=12345&s={0}", SignedFormPlaceholder));

        public const string SignedFormPlaceholder = "SIGNED_FORM_PLACEHOLDER";

        private readonly NewUriFactory newUriFactory;
        private readonly IQuotationEngine quoteEngine;

        public OrderForm(NewUriFactory newUriFactory, IQuotationEngine quoteEngine)
        {
            this.newUriFactory = newUriFactory;
            this.quoteEngine = quoteEngine;
        }

        [UriTemplate("{id}")]
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

            var baseUri = newUriFactory.CreateBaseUri<OrderForm>(request.Uri);

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.CacheControl = new CacheControl {Public = true};
            response.Headers.Expires = quote.CreatedDateTime.AddDays(7.0).UtcDateTime;
            response.Headers.ContentLocation = newUriFactory.CreateAbsoluteUri<Quote>(baseUri, quote.Id.ToString("N"));

            return new Shop(baseUri)
                .AddForm(new Form(
                             OrdersUriFactoryWorker.CreateAbsoluteUri(new Uri("http://localhost:8081")),
                             "post",
                             "application/restbucks+xml",
                             new Shop(baseUri, quote.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                                 .AddLink(new Link(newUriFactory.CreateRelativeUri<Quote>(quote.Id.ToString("N")), "application/restbucks+xml", LinkRelations.Self))));
        }
    }
}