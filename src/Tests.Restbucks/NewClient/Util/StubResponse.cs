using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;

namespace Tests.Restbucks.NewClient.Util
{
    public static class StubResponse
    {
        private static readonly Uri LinkUri = new Uri("request-for-quote", UriKind.Relative);
        private static readonly Uri FormUri = new Uri("orders", UriKind.Relative);
              
        public static readonly Uri BaseUri = new Uri("http://localhost/virtual-directory/");
        public static readonly Uri LinkAbsoluteUri = new Uri(BaseUri, LinkUri);
        public static readonly Uri FormAbsoluteUri = new Uri(BaseUri, FormUri);
        
        public static readonly Link Link = new Link(
            LinkUri,
            RestbucksMediaType.Value,
            new UriLinkRelation(new Uri("http://relations.restbucks.com/rfq")));

        public static readonly Form Form = new Form(
            "order-form",
            new Uri("orders", UriKind.Relative),
            "post",
            RestbucksMediaType.Value,
            new Uri("http://schemas/shop"));
        
        public static HttpResponseMessage CreateResponse()
        {
            var entityBody = new ShopBuilder(BaseUri)
                .AddLink(Link)
                .AddForm(Form)
                .Build();

            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

            return new HttpResponseMessage {Content = content};
        }
    }
}