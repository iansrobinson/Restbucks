using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Restbucks.Client.Adapters;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Http;

namespace Restbucks.Client
{
    public class Start
    {
        public void Go()
        {
            var startLink = new Link(new Uri("http://localhost:8080/restbucks/shop/"), RestbucksMediaType.Value);
            var entryPoint = startLink.Click(Client).EntityBody;

            var prefetcheableLinks = (from l in entryPoint.Links
                                      where l.Rels.Contains(new StringLinkRelation("prefetch"), LinkRelationEqualityComparer.Instance)
                                      select l);
            foreach (var l in prefetcheableLinks)
            {
                l.Prefetch(Client);
            }
        }

        public Response<Shop> Client(Uri uri, Response<Shop> previousResponse)
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(RestbucksMediaType.Value));

            var response = client.Send(request);

            using (response)
            {
                return new HttpResponseMessageToResponse<Shop>(new RestbucksMediaTypeFormatter()).Adapt(response);
            }
        }
    }
}