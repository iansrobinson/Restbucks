using System.Linq;
using System.Net;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Old.Adapters;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [NewUriTemplate("quotes")]
    public class Quotes
    {
        private readonly NewUriFactory newUriFactory;
        private readonly IQuotationEngine quoteEngine;

        public Quotes(NewUriFactory newUriFactory, IQuotationEngine quoteEngine)
        {
            this.newUriFactory = newUriFactory;
            this.quoteEngine = quoteEngine;
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

            var baseUri = newUriFactory.CreateBaseUri<Quotes>(request.Uri);

            var quote =
                quoteEngine.CreateQuote(
                    new QuotationRequest(
                        shop.Items.Select(
                            i => new QuotationRequestItem(i.Description, new Quantity(i.Amount.Measure, i.Amount.Value)))));

            response.StatusCode = HttpStatusCode.Created;
            response.Headers.Location = newUriFactory.CreateAbsoluteUri<Quote>(baseUri, quote.Id.ToString("N"));
            response.Headers.CacheControl = new CacheControl {NoCache = true, NoStore = true};

            return new Shop(baseUri, quote.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                .AddLink(new Link(newUriFactory.CreateRelativeUri<Quote>(quote.Id.ToString("N")), "application/restbucks+xml", LinkRelations.Self))
                .AddLink(new Link(newUriFactory.CreateRelativeUri<OrderForm>(quote.Id.ToString("N")), "application/restbucks+xml", LinkRelations.OrderForm));
        }
    }
}