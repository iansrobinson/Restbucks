using System;
using System.Net.Http;
using Restbucks.Client.ResponseHandlers;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.Client.RulesEngine
{
    public class Rule : IRule
    {
        private readonly Func<bool> condition;
        private readonly Func<IResponseHandler> createResponseHandler;
        private readonly Action<ApplicationContext> contextAction;
        private readonly Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState;

        public Rule(Func<bool> condition, Func<IResponseHandler> createResponseHandler, Action<ApplicationContext> contextAction, Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(contextAction, "contextAction");
            Check.IsNotNull(createState, "createState");

            this.condition = condition;
            this.createResponseHandler = createResponseHandler;
            this.contextAction = contextAction;
            this.createState = createState;
        }

        HandlerResult IRule.Evaluate(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            if (!condition())
            {
                return new HandlerResult(false, null);
            }

            var handler = createResponseHandler();
            return handler.Handle(response, context, clientProvider);
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