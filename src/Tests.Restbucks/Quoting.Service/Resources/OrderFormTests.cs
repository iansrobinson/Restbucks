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

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class OrderFormTests
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:8080/virtual-directory/");

        [Test]
        public void ShouldBaseOrderFormOnQuoteFromQuoteEngine()
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

            var request = new HttpRequestMessage { RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<OrderForm>(BaseAddress, id) };

            var orderForm = new OrderFormBuilder().WithQuotationEngine(quoteEngine).Build();
            var entityBody = orderForm.Get(id.ToString("N"), request, new HttpResponseMessage());

            Assert.True(entityBody.HasForms);
            Assert.True(entityBody.Forms.First().Instance.HasItems);
            Assert.True(Matching.LineItemsMatchItems(quote.LineItems, entityBody.Forms.First().Instance.Items));

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldReturn404NotFoundWhenGettingOrderFormThatDoesNotExist()
        {
            var orderForm = new OrderFormBuilder().WithQuotationEngine(EmptyQuotationEngine.Instance).Build();

            var response = new HttpResponseMessage();
            orderForm.Get(Guid.NewGuid().ToString("N"), new HttpRequestMessage(), response);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
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
            Assert.AreEqual(StubQuotationEngine.Quotation.CreatedDateTime.AddDays(7.00), response.Content.Headers.Expires);
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
            Assert.AreEqual(new Uri("/quote/" + StubQuotationEngine.QuoteId, UriKind.Relative), selfLink.Href.ToString());
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

        [Test]
        public void ShouldIncludeContentLocationHeaderPointingToUnderlyingQuoteResource()
        {
            var response = ExecuteRequestReturnResponse();

            var expectedUriValue = new Uri(BaseAddress + "quote/" + StubQuotationEngine.QuoteId);
            Assert.AreEqual(expectedUriValue, response.Content.Headers.ContentLocation.ToString());
        }

        private static HttpResponseMessage ExecuteRequestReturnResponse()
        {
            var orderForm = new OrderFormBuilder().WithQuotationEngine(StubQuotationEngine.Instance).Build();

            var request = new HttpRequestMessage { RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<OrderForm>(BaseAddress, StubQuotationEngine.QuoteId) };
            var response = new HttpResponseMessage();

            orderForm.Get(StubQuotationEngine.QuoteId, request, response);

            return response;
        }

        private static Shop ExecuteRequestReturnEntityBody()
        {
            var orderForm = new OrderFormBuilder().WithQuotationEngine(StubQuotationEngine.Instance).Build();

            var request = new HttpRequestMessage { RequestUri = DefaultUriFactory.Instance.CreateAbsoluteUri<OrderForm>(BaseAddress, StubQuotationEngine.QuoteId) };
            var response = new HttpResponseMessage();

            return orderForm.Get(StubQuotationEngine.QuoteId, request, response);
        }
    }
}