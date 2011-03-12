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
            return r => formStrategy.GetFormInfo(r) != null;
        }

        public Func<HttpResponseMessage, bool> LinkExists(ILinkStrategy linkStrategy)
        {
            return r => linkStrategy.GetLinkInfo(r) != null;
        }
    }
}