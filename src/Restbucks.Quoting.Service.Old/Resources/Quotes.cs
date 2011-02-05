using System.Linq;
using System.Net;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Old.Adapters;
using Restbucks.RestToolkit;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [UriTemplate("quotes")]
    public class Quotes
    {
        private readonly UriFactory uriFactory;
        private readonly IQuotationEngine quoteEngine;

        public Quotes(UriFactory uriFactory, IQuotationEngine quoteEngine)
        {
            this.uriFactory = uriFactory;
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

            var baseUri = uriFactory.CreateBaseUri<Quotes>(request.Uri);

            var quote =
                quoteEngine.CreateQuote(
                    new QuotationRequest(
                        shop.Items.Select(
                            i => new QuotationRequestItem(i.Description, new Quantity(i.Amount.Measure, i.Amount.Value)))));

            response.StatusCode = HttpStatusCode.Created;
            response.Headers.Location = uriFactory.CreateAbsoluteUri<Quote>(baseUri, quote.Id);
            response.Headers.CacheControl = new CacheControl {NoCache = true, NoStore = true};

            return new Shop(baseUri, quote.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                .AddLink(new Link(uriFactory.CreateRelativeUri<Quote>(quote.Id), RestbucksMediaType.Value, LinkRelations.Self))
                .AddLink(new Link(uriFactory.CreateRelativeUri<OrderForm>(quote.Id), RestbucksMediaType.Value, LinkRelations.OrderForm));
        }
    }
}