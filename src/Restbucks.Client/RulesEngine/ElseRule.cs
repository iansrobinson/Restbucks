using System;
using System.Net.Http;
using System.Reflection;
using Restbucks.Client.ResponseHandlers;

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

        HandlerResult IRule.Evaluate(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            return new HandlerResult(true, null);
        }

        IState IRule.CreateNewState(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            contextAction(context);

            var state = createState(response, context, clientProvider);

            if (state == null)
            {
                throw new NullStateException();
            }

            return state;
        }
    }
}