using System;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class Actions : IActions
    {
        private readonly HttpContentAdapter contentAdapter;
        private readonly ApplicationContext context;
        private readonly HttpClient client;

        public Actions(HttpContentAdapter contentAdapter, ApplicationContext context, HttpClient client)
        {
            this.contentAdapter = contentAdapter;
            this.context = context;
            this.client = client;
        }

        public IAction SubmitForm(IFormStrategy formStrategy)
        {
            return new DeferredAction(() =>
            {
                var formInfoFactory = new ApplicationFormInfoFactory(formStrategy, contentAdapter, context);
                return new SubmitForm(formInfoFactory, client);
            });
        }

        public IAction ClickLink(ILinkStrategy linkStrategy)
        {
            return new DeferredAction(() =>
            {
                var linkInfoFactory = new ApplicationLinkInfoFactory(linkStrategy, contentAdapter);
                return new ClickLink(linkInfoFactory, client);
            });
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