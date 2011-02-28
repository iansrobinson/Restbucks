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
using Tests.Restbucks.Old.Quoting.Service.Old.Resources.Helpers;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources
{
    [TestFixture]
    public class OrderFormTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080/virtual-directory/");
        
        [Test]
        public void ShouldBaseOrderFormOnQuoteFromQuoteEngine()
        {
            var quoteEngine = MockRepository.GenerateMock<IQuotationEngine>();

            var id = Guid.NewGuid();

            var quote = new Quotation(
                Guid.Empty,
                DateTime.Now,
                new[]
                    {
                        new LineItem("item1", new Quantity("g", 250), new Money("GBP", 2.50)),
                        new LineItem("item2", new Quantity("kg", 2), new Money("GBP", 2.00))
                    });

            quoteEngine.Expect(q => q.GetQuote(id)).Return(quote);
            
            var request = CreateHttpRequestMessage(id);
            var result = new OrderForm(DefaultUriFactory.Instance, quoteEngine).Get(id.ToString("N"), request, new HttpResponseMessage());

            Assert.True(result.HasForms);
            Assert.True(result.Forms.First().Instance.HasItems);
            Assert.True(Matching.LineItemsMatchItems(quote.LineItems, result.Forms.First().Instance.Items));
            
            quoteEngine.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturn404NotFoundWhenGettingOrderFormThatDoesNotExist()
        {
            var mocks = new MockRepository();
            var quoteEngine = mocks.Stub<IQuotationEngine>();

            using (mocks.Record())
            {
                SetupResult.For(quoteEngine.GetQuote(Guid.Empty)).IgnoreArguments().Throw(new KeyNotFoundException());
            }
            mocks.ReplayAll();

            var response = new HttpResponseMessage();
            var orderForm = new OrderForm(DefaultUriFactory.Instance, quoteEngine);
            orderForm.Get(Guid.NewGuid().ToString("N"), new HttpRequestMessage(), response);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void ShouldReturn200Ok()
        {
            var result = ExecuteRequestReturnResult(Guid.NewGuid(), DateTime.Now);

            Assert.AreEqual(HttpStatusCode.OK, result.Response.StatusCode);
        }

        [Test]
        public void EntityBodyShouldHaveBaseUri()
        {
            var result = ExecuteRequestReturnResult(Guid.NewGuid(), DateTime.Now);

            Assert.AreEqual(BaseAddress, result.EntityBody.BaseUri);
        }

        [Test]
        public void FormContentsShouldHaveBaseUri()
        {
            var result = ExecuteRequestReturnResult(Guid.NewGuid(), DateTime.Now);
            var formContents = result.EntityBody.Forms.First().Instance;

            Assert.AreEqual(BaseAddress, formContents.BaseUri);
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
        public void FormContentsShouldIncludeSelfLinkForQuote()
        {
            var id = Guid.NewGuid();
            var result = ExecuteRequestReturnResult(id, DateTime.Now);

            var formContents = result.EntityBody.Forms.First().Instance;

            Assert.IsNotNull(formContents.Links.Single(l => l.Rels.First().Value.Equals("self")));
            Assert.AreEqual(new Uri("quote/" + id.ToString("N"), UriKind.Relative), formContents.Links.Single(l => l.Rels.First().Value.Equals("self")).Href.ToString());
        }

        [Test]
        public void FormSchemaAttributeShouldBeEmpty()
        {
            var result = ExecuteRequestReturnResult(Guid.NewGuid(), DateTime.Now);
            Assert.IsNull(result.EntityBody.Forms.First().Schema);
        }

        [Test]
        public void FormIdShouldBeOrder()
        {
            var result = ExecuteRequestReturnResult(Guid.NewGuid(), DateTime.Now);
            Assert.AreEqual("order", result.EntityBody.Forms.First().Id);
        }

        [Test]
        public void FormMethodShouldBePost()
        {
            var result = ExecuteRequestReturnResult(Guid.NewGuid(), DateTime.Now);
            Assert.AreEqual("post", result.EntityBody.Forms.First().Method);
        }

        [Test]
        public void FormMediaTypeShouldBeRestbucksMediaType()
        {
            var result = ExecuteRequestReturnResult(Guid.NewGuid(), DateTime.Now);
            Assert.AreEqual(RestbucksMediaType.Value, result.EntityBody.Forms.First().MediaType);
        }

        [Test]
        public void FormResourceShouldBeOrderingServiceEntryPointUriWithSignedFormValuePlaceholder()
        {
            var result = ExecuteRequestReturnResult(Guid.NewGuid(), DateTime.Now);
            Assert.AreEqual(new Uri("http://localhost:8081/orders/?c=12345&s=" + OrderForm.SignedFormPlaceholder), result.EntityBody.Forms.First().Resource);
        }

        private static Result ExecuteRequestReturnResult(Guid id, DateTimeOffset createdDateTime)
        {
            var quoteEngine = GetQuoteEngine(id, createdDateTime, new LineItem[] {});
            var response = new HttpResponseMessage();
            var orderForm = new OrderForm(DefaultUriFactory.Instance, quoteEngine);
            var entityBody = orderForm.Get(id.ToString("N"), CreateHttpRequestMessage(id), response);

            return new Result {EntityBody = entityBody, Response = response};
        }

        private static HttpRequestMessage CreateHttpRequestMessage(Guid id)
        {
            return new HttpRequestMessage { Uri = DefaultUriFactory.Instance.CreateAbsoluteUri<OrderForm>(BaseAddress, id) };
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