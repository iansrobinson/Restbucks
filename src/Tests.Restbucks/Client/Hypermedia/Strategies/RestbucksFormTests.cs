using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;
using NUnit.Framework;
using Restbucks.Client.Hypermedia.Strategies;
using Restbucks.MediaType;
using Restbucks.RestToolkit.RulesEngine;
using Tests.Restbucks.Client.Util;

namespace Tests.Restbucks.Client.Hypermedia.Strategies
{
    [TestFixture]
    public class RestbucksFormTests
    {
        [Test]
        public void ShouldReturnCorrectFormInfoForForm()
        {
            var form = RestbucksForm.WithId("request-for-quote");
            var formInfo = form.GetFormInfo(DummyResponse.CreateResponse());

            Assert.AreEqual(new MediaTypeHeaderValue(DummyResponse.EmptyForm.MediaType), formInfo.ContentType);
            Assert.AreEqual(new HttpMethod(DummyResponse.EmptyForm.Method), formInfo.Method);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ControlNotFoundException), ExpectedMessage = "Could not find form with id 'xyz'.")]
        public void ThrowsExceptionIfFormCannotBeFound()
        {
            var form = RestbucksForm.WithId("xyz");
            form.GetFormInfo(DummyResponse.CreateResponse());
        }

        [Test]
        public void ShouldFormatRelativeResourceUriAsAbsoluteUri()
        {
            var form = RestbucksForm.WithId("request-for-quote");
            var formInfo = form.GetFormInfo(DummyResponse.CreateResponse());

            Assert.AreEqual(DummyResponse.EmptyFormAbsoluteUri, formInfo.ResourceUri);
        }

        [Test]
        public void ShouldReturnTrueIfFormExists()
        {
            var form = RestbucksForm.WithId("request-for-quote");
            Assert.IsTrue(form.FormExists(DummyResponse.CreateResponse()));
        }

        [Test]
        public void ShouldReturnFalseIfFormDoesNotExist()
        {
            var form = RestbucksForm.WithId("xyz");
            Assert.IsFalse(form.FormExists(DummyResponse.CreateResponse()));
        }

        [Test]
        public void ShouldReturnPrepopulatedFormDataStrategyIfFormIsPrepopulated()
        {
            var form = RestbucksForm.WithId("order-form");
            var dataStrategy = form.GetFormDataStrategy(DummyResponse.CreateResponse());

            Assert.IsInstanceOf(typeof (PrepopulatedFormDataStrategy), dataStrategy);
        }

        [Test]
        public void ShouldReturnApplicationContextFormDataStrategyIfFormDataIsNull()
        {
            var form = RestbucksForm.WithId("request-for-quote");
            var dataStrategy = form.GetFormDataStrategy(DummyResponse.CreateResponse());

            Assert.IsInstanceOf(typeof (ApplicationStateVariablesFormDataStrategy), dataStrategy);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (InvalidOperationException), ExpectedMessage = "Unable to create a data strategy for empty form with null schema attribute. Id: 'invalid-form'.")]
        public void ThrowsExceptionWhenGettingDataStrategyForFormWithNullFormDataButNoSchema()
        {
            var entityBody = new ShopBuilder(new Uri("http://localhost"))
                .AddForm(new Form("invalid-form", new Uri("http://localhost/orders"), "post", RestbucksMediaType.Value, null as Shop))
                .Build();

            var content = new ObjectContent<Shop>(entityBody, new[] { RestbucksMediaTypeFormatter.Instance });
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

            var response = new HttpResponseMessage {Content = content};

            var form = RestbucksForm.WithId("invalid-form");
            form.GetFormDataStrategy(response);
        }
    }
}