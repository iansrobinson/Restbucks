using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Restbucks.RestToolkit.RulesEngine;
using Tests.RestInPractice.RestToolkit.Hacks;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    public class DummyLinkStrategy : ILinkStrategy
    {
        private readonly string rel;

        public DummyLinkStrategy(string rel)
        {
            this.rel = rel;
        }

        public LinkInfo GetLinkInfo(HttpResponseMessage response)
        {
            var entityBody = response.Content.ReadAsObject<DummyEntityBody>(DummyMediaType.Instance);
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