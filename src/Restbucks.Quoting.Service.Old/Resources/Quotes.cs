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
    public class Quotes
    {
        private const string QuoteUriTemplate = "{id}";
        private const string RoutePrefix = "quotes";

        public static readonly UriFactory QuotesUriFactory = new UriFactory(RoutePrefix);
        public static readonly UriFactory QuoteUriFactory = new UriFactory(RoutePrefix, QuoteUriTemplate);

        private readonly IQuotationEngine quoteEngine;

        public Quotes(IQuotationEngine quoteEngine)
        {
            this.quoteEngine = quoteEngine;
        }

        [UriTemplate(QuoteUriTemplate)]
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

            return CreateEntityBody(quote, request.Uri);
        }

        public Shop Post(Shop shop, HttpRequestMessage request, HttpResponseMessage response)
        {
            if (shop == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Headers.CacheControl = new CacheControl {NoCache = true, NoStore = true};
                response.Headers.ContentType = "text/plain";
                response.Content = HttpContent.Create("Bad request: empty or malformed data.");
                return null;
            }

            var quote =
                quoteEngine.CreateQuote(
                    new QuotationRequest(
                        shop.Items.Select(
                            i => new QuotationRequestItem(i.Description, new Quantity(i.Amount.Measure, i.Amount.Value)))));

            response.StatusCode = HttpStatusCode.Created;
            response.Headers.Location = GenerateQuoteUri(request.Uri, quote);
            response.Headers.CacheControl = new CacheControl {NoCache = true, NoStore = true};

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