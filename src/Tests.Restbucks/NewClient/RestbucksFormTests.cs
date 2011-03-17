using System;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;
using Tests.Restbucks.NewClient.Util;
using Tests.Restbucks.Util;

namespace Tests.Restbucks.NewClient
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


//        [Test]
//        public void ShouldReturnPrepopulatedFormDataStrategyIfFormDataIsNotNull()
//        {
//            var form = new Form("order-form", new Uri("http://localhost/orders"), "post", RestbucksMediaType.Value, new ShopBuilder(null).Build());
//            var dataStrategy = RestbucksForm.CreateDataStrategy(form);
//
//            Assert.IsInstanceOf(typeof(PrepopulatedFormDataStrategy), dataStrategy);
//            Assert.AreEqual(form, dataStrategy.GetPrivateFieldValue<Form>("form"));
//            Assert.AreEqual(new MediaTypeHeaderValue(form.MediaType), dataStrategy.GetPrivateFieldValue<MediaTypeHeaderValue>("contentType"));
//        }
//
//        [Test]
//        public void ShouldReturnApplicationContextFormDataStrategyIfFormDataIsNull()
//        {
//            var form = new Form("order-form", new Uri("http://localhost/orders"), "post", RestbucksMediaType.Value, new Uri("http://schemas/shop"));
//            var dataStrategy = RestbucksForm.CreateDataStrategy(form);
//
//            var expectedKey = new EntityBodyKey(form.Id, new MediaTypeHeaderValue(form.MediaType), form.Schema);
//
//            Assert.IsInstanceOf(typeof(ApplicationContextFormDataStrategy), dataStrategy);
//            Assert.AreEqual(expectedKey, dataStrategy.GetPrivateFieldValue<EntityBodyKey>("key"));
//            Assert.AreEqual(new MediaTypeHeaderValue(form.MediaType), dataStrategy.GetPrivateFieldValue<MediaTypeHeaderValue>("contentType"));
//        }
//
//        [Test]
//        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Unable to create a data strategy for empty form with null schema attribute. Id: 'order-form'.")]
//        public void ThrowsExceptionWhenGettingDataStrategyForFormWithNullFormDataButNoSchema()
//        {
//            var form = new Form("order-form", new Uri("http://localhost/orders"), "post", RestbucksMediaType.Value, null as Shop);
//            RestbucksForm.CreateDataStrategy(form);
//        }
    }
}