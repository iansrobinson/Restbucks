using System;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;
using Tests.Restbucks.NewClient.Helpers;
using Tests.Restbucks.Util;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class RestbucksFormTests
    {
        [Test]
        public void ShouldReturnCorrectFormInfoForForm()
        {
            var form = RestbucksForm.WithId("order-form");
            var formInfo = form.GetFormInfo(StubResponse.CreateResponse());

            Assert.AreEqual(new MediaTypeHeaderValue(StubResponse.Form.MediaType), formInfo.ContentType);
            Assert.AreEqual(new HttpMethod(StubResponse.Form.Method), formInfo.Method);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ControlNotFoundException), ExpectedMessage = "Could not find form with id 'xyz'.")]
        public void ThrowsExceptionIfFormCannotBeFound()
        {
            var form = RestbucksForm.WithId("xyz");
            form.GetFormInfo(StubResponse.CreateResponse());
        }

        [Test]
        public void ShouldFormatRelativeResourceUriAsAbsoluteUri()
        {
            var form = RestbucksForm.WithId("order-form");
            var formInfo = form.GetFormInfo(StubResponse.CreateResponse());

            Assert.AreEqual(StubResponse.FormAbsoluteUri, formInfo.ResourceUri);
        }

        [Test]
        public void ShouldReturnTrueIfFormExists()
        {
            var form = RestbucksForm.WithId("order-form");
            Assert.IsTrue(form.FormExists(StubResponse.CreateResponse()));
        }

        [Test]
        public void ShouldReturnFalseIfFormDoesNotExist()
        {
            var form = RestbucksForm.WithId("xyz");
            Assert.IsFalse(form.FormExists(StubResponse.CreateResponse()));
        }

        [Test]
        public void ShouldReturnPrepopulatedFormDataStrategyIfFormDataIsNotNull()
        {
            var form = new Form("order-form", new Uri("http://localhost/orders"), "post", RestbucksMediaType.Value, new ShopBuilder(null).Build());
            var dataStrategy = RestbucksForm.CreateDataStrategy(form);

            Assert.IsInstanceOf(typeof(PrepopulatedFormDataStrategy), dataStrategy);
            Assert.AreEqual(form, dataStrategy.GetPrivateFieldValue<Form>("form"));
        }

        [Test]
        public void ShouldReturnApplicationContextFormDataStrategyIfFormDataIsNull()
        {
            var form = new Form("order-form", new Uri("http://localhost/orders"), "post", RestbucksMediaType.Value, null as Shop);
            var dataStrategy = RestbucksForm.CreateDataStrategy(form);

            Assert.IsInstanceOf(typeof(ApplicationContextFormDataStrategy), dataStrategy);
           
        }
    }
}