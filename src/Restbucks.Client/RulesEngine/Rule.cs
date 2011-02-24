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
            Check.IsNotNull(createResponseHandler, "createResponseHandler");
            Check.IsNotNull(contextAction, "contextAction");
            Check.IsNotNull(createState, "createState");

            this.condition = condition;
            this.createResponseHandler = createResponseHandler;
            this.contextAction = contextAction;
            this.createState = createState;
        }

        Result<IState> IRule.Evaluate(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            if (!condition())
            {
                return new Result<IState>(false, null);
            }

            var result = createResponseHandler().Handle(response, context, clientProvider);

            if (!result.IsSuccessful)
            {
                return new Result<IState>(false, null);
            }
            
            contextAction(context);

            var state = createState(result.Value, context, clientProvider);

            if (state == null)
            {
                throw new NullStateException();
            }

            return new Result<IState>(true, state);
        }
    }
}