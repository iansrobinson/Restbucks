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
using RestInPractice.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("quotes")]
    public class Quotes
    {
        private readonly UriFactory uriFactory;
        private readonly IQuotationEngine quotationEngine;

        public Quotes(UriFactory uriFactory, IQuotationEngine quotationEngine)
        {
            this.uriFactory = uriFactory;
            this.quotationEngine = quotationEngine;
        }

        [WebInvoke(Method = "POST")]
        public HttpResponseMessage<Shop> Post(Shop shop, HttpRequestMessage<Shop> request)
        {
            if (shop == null)
            {
                var errorResponse = new HttpResponseMessage {StatusCode = HttpStatusCode.BadRequest};

                errorResponse.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};
                errorResponse.Content = new StringContent("Bad request: empty or malformed data.");
                errorResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

                throw new HttpResponseException(errorResponse);
            }

            var quote = quotationEngine.CreateQuote(new QuotationRequest(shop.Items.Select(i => new QuotationRequestItem(i.Description, new Quantity(i.Amount.Measure, i.Amount.Value)))));
            var baseUri = uriFactory.CreateBaseUri<Quotes>(request.RequestUri);

            var body = new ShopBuilder(baseUri)
                .AddItems(quote.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                .AddLink(new Link(uriFactory.CreateRelativeUri<Quote>(quote.Id), RestbucksMediaType.Value, LinkRelations.Self))
                .AddLink(new Link(uriFactory.CreateRelativeUri<OrderForm>(quote.Id), RestbucksMediaType.Value, LinkRelations.OrderForm))
                .Build();

            var response = new HttpResponseMessage<Shop>(body, HttpStatusCode.Created);

            response.Headers.Location = uriFactory.CreateAbsoluteUri<Quote>(baseUri, quote.Id);
            response.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};

            return response;
        }
    }
}