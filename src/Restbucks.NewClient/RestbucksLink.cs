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
            var entityBody = contentAdapter.CreateObject(response.Content);
            var link = (from l in ((Shop) entityBody).Links
                        where l.Rels.Contains(relation, LinkRelationEqualityComparer.Instance)
                        select l).FirstOrDefault();

            if (link == null)
            {
                throw new FormNotFoundException(string.Format("Could not find link with link relation '{0}'.", relation.Value));
            }

            return new LinkInfo(link.Href, new MediaTypeHeaderValue(link.MediaType));
        }
    }
}