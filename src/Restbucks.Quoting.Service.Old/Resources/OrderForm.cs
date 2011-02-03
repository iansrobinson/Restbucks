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
    [NewUriTemplate("order-form", OrderFormUriTemplate)]
    public class OrderForm
    {
        private const string OrderFormUriTemplate = "{id}";

        private static readonly UriFactory OrdersUriFactory = new UriFactory("orders",
                                                                             string.Format("?c=12345&s={0}",
                                                                                           SignedFormPlaceholder));

        public static readonly UriFactory UriFactory = new UriFactory("order-forms", OrderFormUriTemplate);
        public const string SignedFormPlaceholder = "SIGNED_FORM_PLACEHOLDER";

        private readonly NewUriFactory newUriFactory;
        private readonly IQuotationEngine quoteEngine;

        public OrderForm(NewUriFactory newUriFactory, IQuotationEngine quoteEngine)
        {
            this.newUriFactory = newUriFactory;
            this.quoteEngine = quoteEngine;
        }

        public OrderForm(IQuotationEngine quoteEngine)
        {
            this.quoteEngine = quoteEngine;
        }

        [UriTemplate(OrderFormUriTemplate)]
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

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.CacheControl = new CacheControl {Public = true};
            response.Headers.Expires = quote.CreatedDateTime.AddDays(7.0).UtcDateTime;
            response.Headers.ContentLocation = Quotes.QuoteUriFactory.CreateAbsoluteUri(request.Uri,
                                                                                        quote.Id.ToString("N"));

            return new Shop(request.Uri)
                .AddForm(new Form(
                             OrdersUriFactory.CreateAbsoluteUri(new Uri("http://localhost:8081")),
                             "post",
                             "application/restbucks+xml",
                             new Shop(request.Uri, quote.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                                 .AddLink(new Link(Quotes.QuoteUriFactory.CreateRelativeUri(quote.Id.ToString("N")),"application/restbucks+xml", LinkRelations.Self))));
        }
    }
}