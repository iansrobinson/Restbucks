using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;
using Rhino.Mocks;
using Tests.Restbucks.Quoting.Service.Resources.Helpers;
using Is = Rhino.Mocks.Constraints.Is;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class QuotesTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080/virtual-directory/");

        [Test]
        public void ShouldReturnNewQuoteFromQuoteEngine()
        {
            var mocks = new MockRepository();
            var quoteEngine = mocks.StrictMock<IQuotationEngine>();

            var shop = new ShopBuilder(new Uri("http://localhost/"))
                .AddItem(new Item("item1", new Amount("g", 250)))
                .AddItem(new Item("item2", new Amount("kg", 2)))
                .Build();

            var quote = new Quotation(
                Guid.Empty,
                DateTime.Now,
                new[]
                    {
                        new LineItem("item1", new Quantity("g", 250), new Money("GBP", 2.50)),
                        new LineItem("item2", new Quantity("kg", 2), new Money("GBP", 2.00))
                    });

            using (mocks.Record())
            {
                Expect
                    .Call(quoteEngine.CreateQuote(null))
                    .Constraints(Is.Matching<QuotationRequest>(qr => Matching.QuoteRequestItemsMatchItems(qr.Items, shop.Items)))
                    .Return(quote);
            }
            mocks.Playback();

            var quotes = new QuotesBuilder().WithQuotationEngine(quoteEngine).Build();
            var result = quotes.Post(shop, new HttpRequestMessage {RequestUri = new Uri("http://localhost:8080/quotes")}, new HttpResponseMessage());

            Assert.True(result.HasItems);
            Assert.True(Matching.LineItemsMatchItems(quote.LineItems, result.Items));

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldReturn201Created()
        {
            var response = ExecuteRequestReturnResponse();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public void ShouldReturnLocationHeaderWithAddressOfNewQuote()
        {
            var response = ExecuteRequestReturnResponse();
            Assert.AreEqual(new Uri(BaseAddress + "quote/" + StubQuotationEngine.QuoteId), response.Headers.Location);
        }

        [Test]
        public void ResponseShouldNotBeCacheable()
        {
            var response = ExecuteRequestReturnResponse();
            Assert.AreEqual("no-store, no-cache", response.Headers.CacheControl.ToString());
        }

        [Test]
        public void ShouldIncludeSelfLinkWithSameValueAsLocationHeader()
        {
            var entityBody = ExecuteRequestReturnEntityBody();

            Assert.IsNotNull(entityBody.Links.Single(l => l.Rels.First().Value.Equals("self")));
            Assert.AreEqual(new Uri("quote/" + StubQuotationEngine.QuoteId, UriKind.Relative), entityBody.Links.Single(l => l.Rels.First().Value.Equals("self")).Href.ToString());
        }

        [Test]
        public void ShouldIncludeLinkToOrderForm()
        {
            var entityBody = ExecuteRequestReturnEntityBody();

            Assert.IsNotNull(entityBody.Links.Single(l => l.Rels.First().SerializableValue.Equals("rb:order-form")));
            Assert.AreEqual(new Uri("order-form/" + StubQuotationEngine.QuoteId, UriKind.Relative), entityBody.Links.Single(l => l.Rels.First().SerializableValue.Equals("rb:order-form")).Href.ToString());
        }

        [Test]
        public void EntityBodyShouldIncludeBaseUri()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            Assert.AreEqual(BaseAddress, entityBody.BaseUri);
        }

        [Test]
        public void ShouldReturn400BadRequestWhenShopIsNull()
        {
            var quotes = new QuotesBuilder().WithQuotationEngine(StubQuotationEngine.Instance).Build();
            var response = new HttpResponseMessage();

            quotes.Post(null, new HttpRequestMessage {RequestUri = new Uri("http://localhost:8080/quotes")}, response);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("no-store, no-cache", response.Headers.CacheControl.ToString());
            Assert.AreEqual("text/plain", response.Content.Headers.ContentType.MediaType);
            Assert.AreEqual("Bad request: empty or malformed data.", response.Content.ReadAsString());
        }

        private static HttpResponseMessage ExecuteRequestReturnResponse()
        {
            var quotes = new QuotesBuilder().WithQuotationEngine(StubQuotationEngine.Instance).Build();

            var request = new HttpRequestMessage {RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<Quotes>(BaseAddress)};
            var response = new HttpResponseMessage();

            quotes.Post(new ShopBuilder(new Uri("http://localhost")).Build(), request, response);

            return response;
        }

        private static Shop ExecuteRequestReturnEntityBody()
        {
            var quotes = new QuotesBuilder().WithQuotationEngine(StubQuotationEngine.Instance).Build();

            var request = new HttpRequestMessage {RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<Quotes>(BaseAddress)};
            var response = new HttpResponseMessage();

            return quotes.Post(new ShopBuilder(new Uri("http://localhost")).Build(), request, response);
        }
    }
}