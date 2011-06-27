using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Restbucks.Client.Hacks;
using Restbucks.RestToolkit.RulesEngine;
using Restbucks.MediaType;

namespace Restbucks.Client.Hypermedia.Strategies
{
    public class RestbucksLink : ILink
    {
        public static ILink WithRel(LinkRelation rel)
        {
            return new RestbucksLink(rel);
        }

        public static ILink WithRel(string rel)
        {
            return WithRel(new StringLinkRelation(rel));
        }

        public static ILink WithRel(Uri rel)
        {
            return WithRel(new UriLinkRelation(rel));
        }

        private readonly LinkRelation relation;

        private RestbucksLink(LinkRelation relation)
        {
            this.relation = relation;
        }

        public LinkInfo GetLinkInfo(HttpResponseMessage response)
        {
            LinkInfo linkInfo;
            var success = TryGetLinkInfo(response, out linkInfo);

            if (!success)
            {
                throw new ControlNotFoundException(string.Format("Could not find link with link relation '{0}'.", relation.Value));
            }

            return linkInfo;
        }

        public bool LinkExists(HttpResponseMessage response)
        {
            LinkInfo linkInfo;
            return TryGetLinkInfo(response, out linkInfo);
        }

        private bool TryGetLinkInfo(HttpResponseMessage response, out LinkInfo linkInfo)
        {
            var entityBody = response.Content.ReadAsObject<Shop>(new[] {RestbucksMediaTypeFormatter.Instance});
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