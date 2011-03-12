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

        public IAction SubmitForm(IFormStrategy formStrategy)
        {
            return new DeferredAction(() => new SubmitForm(formStrategy, client));
        }

        public IAction ClickLink(ILinkStrategy linkStrategy)
        {
            return new DeferredAction(() => new ClickLink(linkStrategy, client));
        }

        private class DeferredAction : IAction
        {
            private readonly Func<IAction> createAction;

            public DeferredAction(Func<IAction> createAction)
            {
                this.createAction = createAction;
            }

            public HttpResponseMessage Execute(HttpResponseMessage previousResponse)
            {
                return createAction().Execute(previousResponse);
            }
        }
    }
}