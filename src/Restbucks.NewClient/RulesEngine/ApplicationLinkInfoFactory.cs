using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class ApplicationLinkInfoFactory : ILinkInfoFactory
    {
        private readonly ILinkStrategy linkStrategy;
        private readonly HttpContentAdapter contentAdapter;

        public ApplicationLinkInfoFactory(ILinkStrategy linkStrategy, HttpContentAdapter contentAdapter)
        {
            this.linkStrategy = linkStrategy;
            this.contentAdapter = contentAdapter;
        }

        public LinkInfo CreateLinkInfo(HttpResponseMessage response)
        {
            return linkStrategy.GetLinkInfo(response, contentAdapter);
        }
    }
}