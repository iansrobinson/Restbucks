using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;
using Restbucks.NewClient;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class HttpResponseMessageTests
    {
        [Test]
        [Ignore("Can't get this to work")]
        public void CanCallReadAsObjectMoreThanOnce()
        {
            var response = CreateResponse();

            var entityBody1 = response.Content.ReadAsObject<Shop>(RestbucksFormatter.Instance);          
            var entityBody2 = response.Content.ReadAsObject<Shop>(RestbucksFormatter.Instance);

            Assert.AreEqual(new Uri("http://localhost/virtual-directory/"), entityBody1.BaseUri);
            Assert.AreEqual(new Uri("http://localhost/virtual-directory/"), entityBody2.BaseUri);
        }

        private static HttpResponseMessage CreateResponse()
        {
            var entityBody = new ShopBuilder(new Uri("http://localhost/virtual-directory/"))
                .AddLink(new Link(
                             new Uri("request-for-quote", UriKind.Relative),
                             RestbucksMediaType.Value,
                             new StringLinkRelation("prefetch")))
                .AddForm(new Form(
                             "order-form",
                             new Uri("orders", UriKind.Relative),
                             "post",
                             RestbucksMediaType.Value,
                             null as Shop))
                .Build();

            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.LoadIntoBuffer();
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

            return new HttpResponseMessage {Content = content};
        }
    }

    public class InternedHttpContent : HttpContent
    {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            throw new NotImplementedException();
        }

        protected override void SerializeToStream(Stream stream, TransportContext context)
        {
            throw new NotImplementedException();
        }

        protected override bool TryComputeLength(out long length)
        {
            throw new NotImplementedException();
        }
    }
}