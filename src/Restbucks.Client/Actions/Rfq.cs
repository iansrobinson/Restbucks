using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Net.Http;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;

namespace Restbucks.Client.Actions
{
    public class Rfq
    {
        private readonly IHttpClientProvider clientProvider;
        private readonly HttpResponseMessage response;

        public Rfq(IHttpClientProvider clientProvider, HttpResponseMessage response)
        {
            this.clientProvider = clientProvider;
            this.response = response;
        }

        public ActionResult GetRfq()
        {
            var entityBody = response.Content.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance);
            var link = (from l in entityBody.Links
                        where l.Rels.Contains(new UriLinkRelation(new Uri("http://relations.restbucks.com/rfq")), LinkRelationEqualityComparer.Instance)
                        select l).First();

            using (var client = clientProvider.CreateClient(entityBody.BaseUri))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, link.Href);
                var newResponse = client.Send(request);

                return new ActionResult(true, newResponse);
            } 
        }
    }
}