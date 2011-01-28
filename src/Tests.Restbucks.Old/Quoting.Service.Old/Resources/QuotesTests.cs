﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting;
using Restbucks.Quoting.Service.Old.Resources;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources
{
    [TestFixture]
    public class QuotesTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080");

        [Test]
        public void ShouldReturnNewQuoteFromQuoteEngine()
        {
            var mocks = new MockRepository();
            var quoteEngine = mocks.StrictMock<IQuotationEngine>();

            var shop = new Shop()
                .AddItem(new Item("item1", new Amount("g", 250)))
                .AddItem(new Item("item2", new Amount("kg", 2)));

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

            var result = new Quotes(quoteEngine).Post(shop, new HttpRequestMessage {Uri = new Uri("http://localhost:8080/quotes")}, new HttpResponseMessage());

            Assert.True(result.HasItems);
            Assert.True(Matching.LineItemsMatchItems(quote.LineItems, result.Items));
          
            mocks.VerifyAll();
        }

        

        [Test]
        public void ShouldReturn201Created()
        {
            var result = ExecuteRequestReturnResult(Guid.Empty, DateTime.Now);

            Assert.AreEqual(HttpStatusCode.Created, result.Response.StatusCode);
        }

        [Test]
        public void ShouldReturnLocationHeaderWithAddressOfNewQuote()
        {
            var id = Guid.NewGuid();
            var result = ExecuteRequestReturnResult(id, DateTime.Now);

            Assert.AreEqual(Quotes.QuoteUriFactory.CreateAbsoluteUri(BaseAddress, id.ToString("N")), result.Response.Headers.Location);
        }

        [Test]
        public void ResponseShouldNotBeCacheable()
        {
            var result = ExecuteRequestReturnResult(Guid.Empty, DateTime.Now);

            Assert.AreEqual("no-cache, no-store", result.Response.Headers.CacheControl.ToString());
        }

        [Test]
        public void ShouldIncludeSelfLinkWithSameValueAsLocationHeader()
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

        [Test]
        public void ShouldReturn400BadRequestWhenShopIsNull()
        {
            var quoteEngine = GetQuoteEngine(Guid.Empty, DateTime.Now, new LineItem[] {});
            var quotes = new Quotes(quoteEngine);
            var response = new HttpResponseMessage();
            quotes.Post(null, new HttpRequestMessage {Uri = new Uri("http://localhost:8080/quotes")}, response);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("no-cache, no-store", response.Headers.CacheControl.ToString());
            Assert.AreEqual("text/plain", response.Headers.ContentType);
            Assert.AreEqual("Bad request: empty or malformed data.", response.Content.ReadAsString());
        }

        private static Result ExecuteRequestReturnResult(Guid id, DateTimeOffset createdDateTime)
        {
            var quoteEngine = GetQuoteEngine(id, createdDateTime, new LineItem[] {});
            var response = new HttpResponseMessage();
            var quotes = new Quotes(quoteEngine);
            var entityBody = quotes.Post(new Shop(), new HttpRequestMessage {Uri = new Uri("http://localhost:8080/quotes")}, response);

            return new Result {EntityBody = entityBody, Response = response};
        }

        private static IQuotationEngine GetQuoteEngine(Guid id, DateTimeOffset createdDateTime, IEnumerable<LineItem> items)
        {
            var mocks = new MockRepository();
            var quoteEngine = mocks.Stub<IQuotationEngine>();

            using (mocks.Record())
            {
                SetupResult.For(quoteEngine.CreateQuote(null)).IgnoreArguments().Return(new Quotation(id, createdDateTime, items));
            }
            mocks.ReplayAll();

            return quoteEngine;
        }
    }
}