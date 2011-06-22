using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting;
using Restbucks.Quoting.Service.Resources;
using Rhino.Mocks;
using Tests.Restbucks.Quoting.Service.Resources.helpers;
using Tests.Restbucks.Quoting.Service.Resources.Util;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class QuoteTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080/virtual-directory/");

        [Test]
        public void ShouldGetQuoteFromQuoteEngine()
        {
            var mockQuoteEngine = MockRepository.GenerateMock<IQuotationEngine>();

            var id = Guid.NewGuid();

            var expectedQuote = new Quotation(
                Guid.Empty,
                DateTime.Now,
                new[]
                    {
                        new LineItem("item1", new Quantity("g", 250), new Money("GBP", 2.50)),
                        new LineItem("item2", new Quantity("kg", 2), new Money("GBP", 2.00))
                    });

            mockQuoteEngine.Expect(q => q.GetQuote(id)).Return(expectedQuote);

            var request = new HttpRequestMessage {RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<Quote>(BaseAddress, id)};
            var quote = new QuoteBuilder().WithQuotationEngine(mockQuoteEngine).Build();
            var response = quote.Get(id.ToString("N"), request);

            var body = response.Content.ReadAsOrDefault();

            Assert.True(body.HasItems);
            Assert.True(Matching.LineItemsMatchItems(expectedQuote.LineItems, body.Items));

            mockQuoteEngine.VerifyAllExpectations();
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
            var quote = new QuoteBuilder().WithQuotationEngine(EmptyQuotationEngine.Instance).Build();
            try
            {
                quote.Get(Guid.NewGuid().ToString("N"), new HttpRequestMessage());
            }
            catch (HttpResponseException ex)
            {
                 Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);;
            }
        }

        [Test]
        public void ResponseShouldExpire7DaysFromDateTimeQuoteWasCreated()
        {
            var response = ExecuteRequestReturnResponse();

            Assert.AreEqual("public", response.Headers.CacheControl.ToString());
            Assert.AreEqual(DummyQuotationEngine.Quotation.CreatedDateTime.AddDays(7.00), response.Content.Headers.Expires);
        }

        [Test]
        public void ShouldIncludeSelfLink()
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

        private static HttpResponseMessage ExecuteRequestReturnResponse()
        {
            var quote = new QuoteBuilder().WithQuotationEngine(DummyQuotationEngine.Instance).Build();

            var request = new HttpRequestMessage {RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<Quote>(BaseAddress, DummyQuotationEngine.QuoteId)};
            return quote.Get(DummyQuotationEngine.QuoteId, request);
        }

        private static Shop ExecuteRequestReturnEntityBody()
        {
            var quote = new QuoteBuilder().WithQuotationEngine(DummyQuotationEngine.Instance).Build();

            var request = new HttpRequestMessage {RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<Quote>(BaseAddress, DummyQuotationEngine.QuoteId)};
            var response = quote.Get(DummyQuotationEngine.QuoteId, request);

            return response.Content.ReadAsOrDefault();
        }
    }
}