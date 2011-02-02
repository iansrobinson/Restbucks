using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Adapters;

namespace Restbucks.Quoting.Service.Resources
{
    [ServiceContract]
    [UriTemplate("quotes")]
    public class Quotes
    {
        private readonly UriFactoryCollection uriFactories;
        private readonly IQuotationEngine quotationEngine;

        public Quotes(UriFactoryCollection uriFactories, IQuotationEngine quotationEngine)
        {
            this.uriFactories = uriFactories;
            this.quotationEngine = quotationEngine;
        }

        [WebInvoke(Method = "POST", UriTemplate = "")]
        public Shop Post(Shop shop, HttpRequestMessage request, HttpResponseMessage response)
        {
            if (shop == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};
                response.Content = new StringContent("Bad request: empty or malformed data.");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                return null;
            }

            var quote = quotationEngine.CreateQuote(new QuotationRequest(shop.Items.Select(i => new QuotationRequestItem(i.Description, new Quantity(i.Amount.Measure, i.Amount.Value)))));
            var baseUri = uriFactories.For<Quotes>().CreateBaseUri(request.RequestUri);

            response.StatusCode = HttpStatusCode.Created;
            response.Headers.Location = uriFactories.For<Quote>().CreateAbsoluteUri(baseUri, quote.Id.ToString("N"));
            response.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};

            return new Shop(baseUri, quote.LineItems.Select(li => new LineItemToItem(li).Adapt()))
                .AddLink(new Link(uriFactories.For<Quote>().CreateRelativeUri(quote.Id.ToString("N")), "application/restbucks+xml", LinkRelations.Self))
                .AddLink(new Link(uriFactories.For<OrderForm>().CreateRelativeUri(quote.Id.ToString("N")), "application/restbucks+xml", LinkRelations.OrderForm));
        }
    }
}