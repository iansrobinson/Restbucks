using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient
{
    public class RestbucksLink : ILinkStrategy
    {
        public static ILinkStrategy WithRel(LinkRelation rel)
        {
            return new RestbucksLink(rel);
        }
        
        private readonly LinkRelation relation;

        public RestbucksLink(LinkRelation relation)
        {
            this.relation = relation;
        }

        public LinkInfo GetLinkInfo(HttpResponseMessage response, HttpContentAdapter contentAdapter)
        {
            LinkInfo linkInfo;
            var success = TryGetLinkInfo(response, contentAdapter, out linkInfo);

            if (!success)
            {
                throw new ControlNotFoundException(string.Format("Could not find link with link relation '{0}'.", relation.Value));
            }

            return linkInfo;
        }

        public bool TryGetLinkInfo(HttpResponseMessage response, HttpContentAdapter contentAdapter, out LinkInfo linkInfo)
        {
            var entityBody = (Shop)contentAdapter.CreateObject(response.Content);
            var link = (from l in (entityBody).Links
                        where l.Rels.Contains(relation, LinkRelationEqualityComparer.Instance)
                        select l).FirstOrDefault();

            if (link == null)
            {
                linkInfo = null;
                return false;
            }

            var resourceUri = link.Href.IsAbsoluteUri ? link.Href : new Uri(entityBody.BaseUri, link.Href);
            linkInfo = new LinkInfo(resourceUri, new MediaTypeHeaderValue(link.MediaType));

            return true;
        }
    }
}