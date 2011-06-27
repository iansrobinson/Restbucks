using System;
using System.Net.Http;
using Restbucks.RestToolkit.RulesEngine;
using Tests.RestInPractice.RestToolkit.Hacks;
using Tests.RestInPractice.RestToolkit.Utils;

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
            if (entityBody.LinkRel.Equals(rel))
            {
                return new LinkInfo(new Uri("http://localhost/resource-uri"), DummyMediaType.ContentType);
            }
            return null;
        }

        public bool LinkExists(HttpResponseMessage response)
        {
            return GetLinkInfo(response) != null;
        }
    }
}