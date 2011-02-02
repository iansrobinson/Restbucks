using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;
using Rhino.Mocks;
using Tests.Restbucks.Quoting.Service.Resources.helpers;
using Tests.Restbucks.Quoting.Service.Resources.Helpers;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class QuoteTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080/virtual-directory/");

        [Test]
        public void ShouldGetQuoteFromQuoteEngine()
        {
            var mocks = new MockRepository();
            var quoteEngine = mocks.StrictMock<IQuotationEngine>();

            var id = Guid.NewGuid();

            var expectedQuote = new Quotation(
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
                    .Call(quoteEngine.GetQuote(id))
                    .Return(expectedQuote);
            }
            mocks.Playback();

            var request = new HttpRequestMessage {RequestUri = DefaultUriFactoryCollection.Instance.For<Quote>().CreateAbsoluteUri(BaseAddress, id.ToString("N"))};
            var quote = new QuoteBuilder().WithQuotationEngine(quoteEngine).Build();
            var result = quote.Get(id.ToString("N"), request, new HttpResponseMessage());

            Assert.True(result.HasItems);
            Assert.True(Matching.LineItemsMatchItems(expectedQuote.LineItems, result.Items));

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldReturn200Ok()
        {
            var response = ExecuteRequestReturnResponse();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ShouldReturn404NotFoundWhenGettingQuoteThatDoesNotExist()
        {
            var response = new HttpResponseMessage();
            var quote = new QuoteBuilder().WithQuotationEngine(EmptyQuotationEngine.Instance).Build();
            quote.Get(Guid.NewGuid().ToString("N"), new HttpRequestMessage(), response);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void ResponseShouldExpire7DaysFromDateTimeQuoteWasCreated()
        {
            var response = ExecuteRequestReturnResponse();

            Assert.AreEqual("public", response.Headers.CacheControl.ToString());
            Assert.AreEqual(StubQuotationEngine.Quotation.CreatedDateTime.AddDays(7.00), response.Content.Headers.Expires);
        }

        [Test]
        public void ShouldIncludeSelfLink()
        {
            var entityBody = ExecuteRequestReturnEntityBody();

            Assert.IsNotNull(entityBody.Links.Single(l => l.Rels.First().Value.Equals("self")));
            Assert.AreEqual(DefaultUriFactoryCollection.Instance.For<Quote>().CreateRelativeUri(StubQuotationEngine.QuoteId), entityBody.Links.Single(l => l.Rels.First().Value.Equals("self")).Href.ToString());
        }

        [Test]
        public void ShouldIncludeLinkToOrderForm()
        {
            var entityBody = ExecuteRequestReturnEntityBody();

            Assert.IsNotNull(entityBody.Links.Single(l => l.Rels.First().SerializableValue.Equals("rb:order-form")));
            Assert.AreEqual(DefaultUriFactoryCollection.Instance.For<OrderForm>().CreateRelativeUri(StubQuotationEngine.QuoteId), entityBody.Links.Single(l => l.Rels.First().SerializableValue.Equals("rb:order-form")).Href.ToString());
        }

        [Test]
        public void EntityBodyShouldIncludeBaseUri()
        {
            var entityBody = ExecuteRequestReturnEntityBody();

            Assert.AreEqual(BaseAddress, entityBody.BaseUri);
        }

        private static HttpResponseMessage ExecuteRequestReturnResponse()
        {
            var quote = new QuoteBuilder().WithQuotationEngine(StubQuotationEngine.Instance).Build();

            var request = new HttpRequestMessage {RequestUri = DefaultUriFactoryCollection.Instance.For<Quote>().CreateAbsoluteUri(BaseAddress, StubQuotationEngine.QuoteId)};
            var response = new HttpResponseMessage();

            quote.Get(StubQuotationEngine.QuoteId, request, response);

            return response;
        }

        private static Shop ExecuteRequestReturnEntityBody()
        {
            var quote = new QuoteBuilder().WithQuotationEngine(StubQuotationEngine.Instance).Build();

            var request = new HttpRequestMessage { RequestUri = DefaultUriFactoryCollection.Instance.For<Quote>().CreateAbsoluteUri(BaseAddress, StubQuotationEngine.QuoteId) };
            var response = new HttpResponseMessage();

            return quote.Get(StubQuotationEngine.QuoteId, request, response);
        }
    }
}