using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;
using Tests.Restbucks.NewClient.Helpers;

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
    }
}