using System;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class Actions : IActions
    {
        private readonly ApplicationContext context;
        private readonly HttpClient client;

        public Actions(ApplicationContext context, HttpClient client)
        {
            this.context = context;
            this.client = client;
        }

        public IActionInvoker SubmitForm(IFormStrategy formStrategy)
        {
            return new DeferredActionInvoker(() => new SubmitForm(formStrategy, client));
        }

        public IActionInvoker ClickLink(ILinkStrategy linkStrategy)
        {
            return new DeferredActionInvoker(() => new ClickLink(linkStrategy, client));
        }

        private class DeferredActionInvoker : IActionInvoker
        {
            private readonly Func<IActionInvoker> createAction;

            public DeferredActionInvoker(Func<IActionInvoker> createAction)
            {
                this.createAction = createAction;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse)
            {
                return createAction().Invoke(previousResponse);
            }
        }
    }
}