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
using Tests.Restbucks.Quoting.Service.Resources.Util;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class OrderFormTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080/virtual-directory/");

        [Test]
        public void ShouldBaseOrderFormOnQuoteFromQuoteEngine()
        {
            var mockQuoteEngine = MockRepository.GenerateMock<IQuotationEngine>();

            var id = Guid.NewGuid();

            var quote = new Quotation(
                Guid.Empty,
                DateTime.Now,
                new[]
                    {
                        new LineItem("item1", new Quantity("g", 250), new Money("GBP", 2.50)),
                        new LineItem("item2", new Quantity("kg", 2), new Money("GBP", 2.00))
                    });

            mockQuoteEngine.Expect(q => q.GetQuote(id)).Return(quote);

            var request = new HttpRequestMessage {RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<OrderForm>(BaseAddress, id)};

            var orderForm = new OrderForm(DefaultUriFactory.Instance, mockQuoteEngine);
            var response = orderForm.Get(id.ToString("N"), request);

            var entityBody = response.Content.ReadAsOrDefault();

            Assert.True(entityBody.HasForms);
            Assert.True(entityBody.Forms.First().Instance.HasItems);
            Assert.True(Matching.LineItemsMatchItems(quote.LineItems, entityBody.Forms.First().Instance.Items));

            mockQuoteEngine.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturn404NotFoundWhenGettingOrderFormThatDoesNotExist()
        {
            try
            {
                var orderForm = new OrderForm(DefaultUriFactory.Instance, EmptyQuotationEngine.Instance);
                orderForm.Get(Guid.NewGuid().ToString("N"), new HttpRequestMessage());
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
            }
        }

        [Test]
        public void ShouldReturn200Ok()
        {
            var response = ExecuteRequestReturnResponse();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ResponseShouldExpire7DaysFromDateTimeQuoteWasCreated()
        {
            var response = ExecuteRequestReturnResponse();

            Assert.AreEqual("public", response.Headers.CacheControl.ToString());
            Assert.AreEqual(DummyQuotationEngine.Quotation.CreatedDateTime.AddDays(7.00), response.Content.Headers.Expires);
        }

        [Test]
        public void EntityBodyShouldHaveBaseUri()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            Assert.AreEqual(BaseAddress, entityBody.BaseUri);
        }

        [Test]
        public void FormContentsShouldIncludeSelfLinkForQuote()
        {
            var entityBody = ExecuteRequestReturnEntityBody();

            var formContents = entityBody.Forms.First().Instance;
            var selfLink = formContents.Links.Single(l => l.Rels.First().Value.Equals("self"));

            Assert.IsNotNull(selfLink);
            Assert.AreEqual(new Uri("quote/" + DummyQuotationEngine.QuoteId, UriKind.Relative), selfLink.Href.ToString());
        }

        [Test]
        public void FormContentsShouldIncludeBaseUri()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            var formContents = entityBody.Forms.First().Instance;

            Assert.AreEqual(BaseAddress, formContents.BaseUri);
        }

        [Test]
        public void FormSchemaAttributeShouldBeEmpty()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            Assert.IsNull(entityBody.Forms.First().Schema);
        }

        [Test]
        public void FormIdShouldBeOrder()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            Assert.AreEqual("order", entityBody.Forms.First().Id);
        }

        [Test]
        public void FormMethodShouldBePost()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            Assert.AreEqual("post", entityBody.Forms.First().Method);
        }

        [Test]
        public void FormMediaTypeShouldBeRestbucksMediaType()
        {
            var entityBody = ExecuteRequestReturnEntityBody();
            Assert.AreEqual(RestbucksMediaType.Value, entityBody.Forms.First().MediaType);
        }

        [Test]
        public void FormResourceShouldBeOrderingServiceEntryPointUriWithSignedFormValuePlaceholder()
        {
            var entityBody = ExecuteRequestReturnEntityBody();

            Assert.AreEqual(new Uri("http://localhost:8081/orders?c=12345&s=" + OrderForm.SignedFormPlaceholder), entityBody.Forms.First().Resource);
        }

        private static HttpResponseMessage ExecuteRequestReturnResponse()
        {
            var orderForm = new OrderForm(DefaultUriFactory.Instance, DummyQuotationEngine.Instance);
           
            var request = new HttpRequestMessage {RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<OrderForm>(BaseAddress, DummyQuotationEngine.QuoteId)};
            return orderForm.Get(DummyQuotationEngine.QuoteId, request);
        }

        private static Shop ExecuteRequestReturnEntityBody()
        {
            var orderForm = new OrderForm(DefaultUriFactory.Instance, DummyQuotationEngine.Instance);
           
            var request = new HttpRequestMessage {RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<OrderForm>(BaseAddress, DummyQuotationEngine.QuoteId)};
            var response = orderForm.Get(DummyQuotationEngine.QuoteId, request);

            return response.Content.ReadAsOrDefault();
        }
    }
}