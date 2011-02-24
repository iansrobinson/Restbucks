using System;
using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public class ElseRule : IRule
    {
        private readonly Action<ApplicationContext> contextAction;
        private readonly Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState;

        public ElseRule(Action<ApplicationContext> contextAction, Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState)
        {
            this.contextAction = contextAction;
            this.createState = createState;
        }

        Result<IState> IRule.Evaluate(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            contextAction(context);

            var state = createState(response, context, clientProvider);

            if (state == null)
            {
                throw new NullStateException();
            }

            return new Result<IState>(true, state);
        }
    }
}