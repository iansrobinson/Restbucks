using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Http;
using NUnit.Framework;
using Restbucks.Quoting;
using Restbucks.Quoting.Service.Old.Resources;
using Rhino.Mocks;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources
{
    [TestFixture]
    public class QuoteTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080");

        [Test]
        public void ShouldGetQuoteFromQuoteEngine()
        {
            var mocks = new MockRepository();
            var quoteEngine = mocks.StrictMock<IQuotationEngine>();

            var id = Guid.NewGuid();

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
                    .Call(quoteEngine.GetQuote(id))
                    .Return(quote);
            }
            mocks.Playback();

            var request = CreateHttpRequestMessage(id);
            var result = new Quotes(quoteEngine).Get(id.ToString("N"), request, new HttpResponseMessage());

            Assert.True(result.HasItems);
            Assert.True(Matching.LineItemsMatchItems(quote.LineItems, result.Items));

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldReturn200Ok()
        {
            var result = ExecuteRequestReturnResult(Guid.NewGuid(), DateTime.Now);

            Assert.AreEqual(HttpStatusCode.OK, result.Response.StatusCode);
        }

        [Test]
        public void ShouldReturn404NotFoundWhenGettingQuoteThatDoesNotExist()
        {
            var mocks = new MockRepository();
            var quoteEngine = mocks.Stub<IQuotationEngine>();

            using (mocks.Record())
            {
                SetupResult.For(quoteEngine.GetQuote(Guid.Empty)).IgnoreArguments().Throw(new KeyNotFoundException());
            }
            mocks.ReplayAll();

            var response = new HttpResponseMessage();
            var quotes = new Quotes(quoteEngine);
            quotes.Get(Guid.NewGuid().ToString("N"), new HttpRequestMessage(), response);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void ResponseShouldExpire7DaysFromDateTimeQuoteWasCreated()
        {
            DateTimeOffset createdDateTime = DateTime.Now;
            var result = ExecuteRequestReturnResult(Guid.NewGuid(), createdDateTime);

            Assert.AreEqual("public", result.Response.Headers.CacheControl.ToString());
            Assert.AreEqual(createdDateTime.AddDays(7.00).UtcDateTime, result.Response.Headers.Expires);
        }

        [Test]
        public void ShouldIncludeSelfLink()
        {
            var id = Guid.NewGuid();
            var result = ExecuteRequestReturnResult(id, DateTime.Now);

            Assert.IsNotNull(result.EntityBody.Links.Single(l => l.Rels.First().Value.Equals("self")));
            Assert.AreEqual(Quotes.QuoteUriFactory.CreateRelativeUri(id.ToString("N")), result.EntityBody.Links.Single(l => l.Rels.First().Value.Equals("self")).Href.ToString());
        }

        [Test]
        public void ShouldIncludeLinkToOrderForm()
        {
            var id = Guid.NewGuid();
            var result = ExecuteRequestReturnResult(id, DateTime.Now);

            Assert.IsNotNull(result.EntityBody.Links.Single(l => l.Rels.First().SerializableValue.Equals("rb:order-form")));
            Assert.AreEqual(OrderForm.UriFactory.CreateRelativeUri(id.ToString("N")), result.EntityBody.Links.Single(l => l.Rels.First().SerializableValue.Equals("rb:order-form")).Href.ToString());
        }

        private static Result ExecuteRequestReturnResult(Guid id, DateTimeOffset createdDateTime)
        {
            var quoteEngine = GetQuoteEngine(id, createdDateTime, new LineItem[] {});
            var response = new HttpResponseMessage();
            var quotes = new Quotes(quoteEngine);
            var entityBody = quotes.Get(id.ToString("N"), CreateHttpRequestMessage(id), response);

            return new Result {EntityBody = entityBody, Response = response};
        }

        private static HttpRequestMessage CreateHttpRequestMessage(Guid id)
        {
            return new HttpRequestMessage {Uri = Quotes.QuoteUriFactory.CreateAbsoluteUri(BaseAddress, id.ToString("N"))};
        }

        private static IQuotationEngine GetQuoteEngine(Guid id, DateTimeOffset createdDateTime, IEnumerable<LineItem> items)
        {
            var mocks = new MockRepository();
            var quoteEngine = mocks.Stub<IQuotationEngine>();

            using (mocks.Record())
            {
                SetupResult.For(quoteEngine.GetQuote(Guid.Empty)).IgnoreArguments().Return(new Quotation(id, createdDateTime, items));
            }
            mocks.ReplayAll();

            return quoteEngine;
        }
    }
}