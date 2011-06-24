using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;
using Rhino.Mocks;
using Tests.Restbucks.Quoting.Service.Resources.Util;
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
            var mockQuoteEngine = MockRepository.GenerateMock<IQuotationEngine>();

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

            mockQuoteEngine.Expect(q => q.CreateQuote(null))
                .Constraints(Is.Matching<QuotationRequest>(qr => Matching.QuoteRequestItemsMatchItems(qr.Items, shop.Items)))
                .Return(quote);

            var quotes = new Quotes(DefaultUriFactory.Instance, mockQuoteEngine);
            var response = quotes.Post(shop, new HttpRequestMessage<Shop>{ RequestUri = new Uri("http://localhost:8080/quotes") });

            var entityBody = response.Content.ReadAsOrDefault();

            Assert.True(entityBody.HasItems);
            Assert.True(Matching.LineItemsMatchItems(quote.LineItems, entityBody.Items));

            mockQuoteEngine.VerifyAllExpectations();
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
            Assert.AreEqual(new Uri(BaseAddress + "quote/" + DummyQuotationEngine.QuoteId), response.Headers.Location);
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
            Assert.AreEqual(new Uri("quote/" + DummyQuotationEngine.QuoteId, UriKind.Relative), entityBody.Links.Single(l => l.Rels.First().Value.Equals("self")).Href.ToString());
        }

        [Test]
        public void ShouldIncludeLinkToOrderForm()
        {
            var entityBody = ExecuteRequestReturnEntityBody();

            Assert.IsNotNull(entityBody.Links.Single(l => l.Rels.First().DisplayValue.Equals("rb:order-form")));
            Assert.AreEqual(new Uri("order-form/" + DummyQuotationEngine.QuoteId, UriKind.Relative), entityBody.Links.Single(l => l.Rels.First().DisplayValue.Equals("rb:order-form")).Href.ToString());
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
            try
            {
                var quotes = new Quotes(DefaultUriFactory.Instance, DummyQuotationEngine.Instance);
                quotes.Post(null, new HttpRequestMessage<Shop> { RequestUri = new Uri("http://localhost:8080/quotes") });
                Assert.Fail();

            }
            catch (HttpResponseException ex)
            {
                var response = ex.Response;
                
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.AreEqual("no-store, no-cache", response.Headers.CacheControl.ToString());
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.MediaType);
                Assert.AreEqual("Bad request: empty or malformed data.", response.Content.ReadAsString());
            }
        }

        private static HttpResponseMessage ExecuteRequestReturnResponse()
        {
            var quotes = new Quotes(DefaultUriFactory.Instance, DummyQuotationEngine.Instance);
            
            var request = new HttpRequestMessage<Shop> {RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<Quotes>(BaseAddress)};
            return quotes.Post(new ShopBuilder(new Uri("http://localhost")).Build(), request);
        }

        private static Shop ExecuteRequestReturnEntityBody()
        {
            var quotes = new Quotes(DefaultUriFactory.Instance, DummyQuotationEngine.Instance);

            var request = new HttpRequestMessage<Shop> {RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<Quotes>(BaseAddress)};
            var response = quotes.Post(new ShopBuilder(new Uri("http://localhost")).Build(), request);

            return response.Content.ReadAsOrDefault();
        }
    }
}