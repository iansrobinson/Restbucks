using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;
using Restbucks.MediaType;
using Restbucks.Client;

namespace Tests.Restbucks.NewClient.Util
{
    public static class DummyResponse
    {
        private static readonly Uri LinkUri = new Uri("request-for-quote", UriKind.Relative);
        private static readonly Uri EmptyFormUri = new Uri("quotes", UriKind.Relative);
        private static readonly Uri PrepopulatedFormUri = new Uri("order/1234", UriKind.Relative);

        public static readonly Uri BaseUri = new Uri("http://localhost/virtual-directory/");
        public static readonly Uri RestbucksBaseUri = new Uri("http://restbucks.com/");
        public static readonly Uri LinkAbsoluteUri = new Uri(BaseUri, LinkUri);
        public static readonly Uri EmptyFormAbsoluteUri = new Uri(BaseUri, EmptyFormUri);
        public static readonly Uri PrepopulatedFormAbsoluteUri = new Uri(BaseUri, PrepopulatedFormUri);

        public static readonly Link Link = new Link(
            LinkUri,
            RestbucksMediaType.Value,
            new UriLinkRelation(new Uri("http://relations.restbucks.com/rfq")));

        public static readonly Form EmptyForm = new Form(
            "request-for-quote",
            EmptyFormUri,
            "post",
            RestbucksMediaType.Value,
            new Uri("http://schemas/shop"));

        public static readonly Form PrepopulatedForm = new Form(
            "order-form",
            PrepopulatedFormUri,
            "post",
            RestbucksMediaType.Value,
            new ShopBuilder(RestbucksBaseUri).Build());

        public static HttpResponseMessage CreateResponse()
        {
            var entityBody = new ShopBuilder(BaseUri)
                .AddLink(Link)
                .AddForm(EmptyForm)
                .AddForm(PrepopulatedForm)
                .Build();

            var content = new ObjectContent<Shop>(entityBody, new[] {RestbucksFormatter.Instance});
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

            return new HttpResponseMessage {Content = content};
        }
    }
}