using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using log4net;
using Microsoft.Net.Http;
using Restbucks.Client.Formatters;
using Restbucks.Client.RulesEngine;
using Restbucks.MediaType;

namespace Restbucks.Client.ResponseHandlers
{
    public class StartedResponseHandler : IResponseHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IHttpClientProvider clientProvider;

        public StartedResponseHandler(IHttpClientProvider clientProvider)
        {
            this.clientProvider = clientProvider;
        }

        public Result<HttpResponseMessage> Handle(HttpResponseMessage response, ApplicationContext context)
        {
            Log.Debug("  Getting request-for-quote form...");
            
            var entityBody = response.Content.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance);
            var link = (from l in entityBody.Links
                        where l.Rels.Contains(new UriLinkRelation(new Uri("http://relations.restbucks.com/rfq")), LinkRelationEqualityComparer.Instance)
                        select l).First();

            using (var client = clientProvider.CreateClient(entityBody.BaseUri))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, link.Href);
                var newResponse = client.Send(request);

                return new Result<HttpResponseMessage>(true, newResponse);
            } 
        }
    }
}