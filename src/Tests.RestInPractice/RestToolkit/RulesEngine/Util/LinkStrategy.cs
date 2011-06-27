using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Restbucks.RestToolkit.RulesEngine;
using Tests.RestInPractice.RestToolkit.Hacks;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    public class LinkStrategy : ILinkStrategy
    {
        private readonly string rel;

        public LinkStrategy(string rel)
        {
            this.rel = rel;
        }

        public LinkInfo GetLinkInfo(HttpResponseMessage response)
        {
            var entityBody = response.Content.ReadAsObject<ExampleEntityBody>(ExampleMediaType.Instance);
            if (entityBody.Link.Rel.Equals(rel))
            {
                return new LinkInfo(new Uri(entityBody.Link.Uri), new MediaTypeHeaderValue(entityBody.Link.ContentType));
            }
            return null;
        }

        public bool LinkExists(HttpResponseMessage response)
        {
            return GetLinkInfo(response) != null;
        }
    }
}