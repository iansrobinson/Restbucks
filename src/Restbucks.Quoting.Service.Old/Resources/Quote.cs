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
    [UriTemplate("quote", "{id}")]
    public class Quote
    {
        private readonly UriFactory uriFactory;
        private readonly IQuotationEngine quoteEngine;

        public Quote(UriFactory uriFactory, IQuotationEngine quoteEngine)
        {
            this.uriFactory = uriFactory;
            this.quoteEngine = quoteEngine;
        }

        public Quote(IQuotationEngine quoteEngine)
        {
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

            var baseUri = uriFactory.CreateBaseUri<Quote>(request.Uri);

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.CacheControl = new CacheControl {Public = true};
            response.Headers.Expires = quote.CreatedDateTime.AddDays(7.0).UtcDateTime;

            return new Shop(baseUri, quote.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                .AddLink(new Link(uriFactory.CreateRelativeUri<Quote>(quote.Id.ToString("N")), "application/restbucks+xml", LinkRelations.Self))
                .AddLink(new Link(uriFactory.CreateRelativeUri<OrderForm>(quote.Id.ToString("N")), "application/restbucks+xml", LinkRelations.OrderForm));
        }
    }
}