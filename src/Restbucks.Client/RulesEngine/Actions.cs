using System;
using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public class Actions
    {
        private readonly Action<ApplicationContext> contextAction;
        private readonly Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState;

        public Actions(Action<ApplicationContext> contextAction, Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState)
        {
            this.contextAction = contextAction;
            this.createState = createState;
        }

        public Action<ApplicationContext> ContextAction
        {
            get { return contextAction; }
        }

        public Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> CreateState
        {
            get { return createState; }
        }
    }
}