using System;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class Conditions
    {
        private readonly HttpContentAdapter contentAdapter;
        private readonly ApplicationContext context;

        public Conditions(HttpContentAdapter contentAdapter, ApplicationContext context)
        {
            this.contentAdapter = contentAdapter;
            this.context = context;
        }

        public Func<HttpResponseMessage, bool> FormExists(IFormStrategy formStrategy)
        {
            FormInfo formInfo;
            return r => formStrategy.TryGetFormInfo(r, contentAdapter, context, out formInfo);
        }

        public Func<HttpResponseMessage, bool> LinkExists(ILinkStrategy linkStrategy)
        {
            LinkInfo linkInfo;
            return r => linkStrategy.TryGetLinkInfo(r, out linkInfo);
        }
    }
}