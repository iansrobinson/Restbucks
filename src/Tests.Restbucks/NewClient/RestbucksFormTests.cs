using System;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class RestbucksFormTests
    {
        private static readonly Uri ResourceUri = new Uri("http://localhost/rfq");
        private static readonly HttpMethod Method = HttpMethod.Put;
        private static readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
        private static readonly Shop FormBody = new ShopBuilder(null).Build();
        
        [Test]
        public void ShouldReturnCorrectFormInfoForForm()
        {
            var entityBody = CreateEntityBody();

            var form = new RestbucksForm("rfq");
            var formInfo = form.GetFormInfo(entityBody);

            Assert.AreEqual(ResourceUri, formInfo.ResourceUri);
            Assert.AreEqual(Method, formInfo.Method);
            Assert.AreEqual(ContentType, formInfo.ContentType);
        }

        [Test]
        public void ShouldReturnInputAsFormDataWhenFormBodyIsEmpty()
        {
            var input = new Object();
            var entityBody = CreateEntityBody();

            var form = new RestbucksForm("rfq");
            var formData = form.GetFormData(entityBody, input);

            Assert.AreEqual(input, formData);
        }

        [Test]
        public void ShouldReturnFormBodyAsFormDataWhenFormBodyIsNotEmpty()
        {
            var input = new Object();
            var entityBody = CreateEntityBody();

            var form = new RestbucksForm("order");
            var formData = form.GetFormData(entityBody, input);

            Assert.AreEqual(FormBody, formData);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: entityBody")]
        public void ThrowsExceptionIfCallingGetFormInfoWithNullEntityBody()
        {
            var form = new RestbucksForm("order");
            form.GetFormInfo(null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: entityBody")]
        public void ThrowsExceptionIfCallingGetFormDataWithNullEntityBody()
        {
            var form = new RestbucksForm("order");
            form.GetFormData(null, new object());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: input")]
        public void ThrowsExceptionIfCallingGetFormDataWithNullInput()
        {
            var form = new RestbucksForm("order");
            form.GetFormData(CreateEntityBody(), null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(FormNotFoundException), ExpectedMessage = "Could not find form with id 'xyz'.")]
        public void ThrowsExceptionIfFormCannotBeFound()
        {
            var form = new RestbucksForm("xyz");
            form.GetFormInfo(CreateEntityBody());
        }

        private static Shop CreateEntityBody()
        {
            return new ShopBuilder(null)
                .AddForm(new Form("rfq", ResourceUri, Method.ToString(), ContentType.MediaType, null as Shop))
                .AddForm(new Form("order", new Uri("http://localhost/orders"), "post", "application/xml", FormBody))
                .Build();
        }
    }
}