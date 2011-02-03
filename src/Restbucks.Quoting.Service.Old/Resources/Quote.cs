using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Old.Adapters;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [NewUriTemplate("quote", "{id}")]
    public class Quote
    {
        private const string QuoteUriTemplate = "{id}";
        private const string RoutePrefix = "quotes";
        
        public static readonly UriFactory QuoteUriFactory = new UriFactory(RoutePrefix, QuoteUriTemplate);

        private readonly NewUriFactory newUriFactory;
        private readonly IQuotationEngine quoteEngine;

        public Quote(NewUriFactory newUriFactory, IQuotationEngine quoteEngine)
        {
            this.newUriFactory = newUriFactory;
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

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.CacheControl = new CacheControl { Public = true };
            response.Headers.Expires = quote.CreatedDateTime.AddDays(7.0).UtcDateTime;

            return CreateEntityBody(quote, request.Uri);
        }

        private static Shop CreateEntityBody(Quotation quote, Uri requestUri)
        {
            var uri = GenerateQuoteUri(requestUri, quote);

            return new Shop(uri, quote.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                .AddLink(new Link(new Uri(uri.PathAndQuery, UriKind.Relative), "application/restbucks+xml", LinkRelations.Self))
                .AddLink(new Link(OrderForm.UriFactory.CreateRelativeUri(quote.Id.ToString("N")), "application/restbucks+xml", LinkRelations.OrderForm));
        }

        private static Uri GenerateQuoteUri(Uri requestUri, Quotation quote)
        {
            return QuoteUriFactory.CreateAbsoluteUri(requestUri, quote.Id.ToString("N"));
        }
    }
}