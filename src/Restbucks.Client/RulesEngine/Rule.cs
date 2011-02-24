using System;
using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.Client.RulesEngine
{
    public class Rule : IRule
    {
        private readonly Func<bool> condition;
        private readonly Func<IResponseHandler> createResponseHandler;
        private readonly Action<ApplicationContext> contextAction;
        private readonly Func<HttpResponseMessage, ApplicationContext, IState> createState;

        public Rule(Func<bool> condition, Func<IResponseHandler> createResponseHandler, Action<ApplicationContext> contextAction, Func<HttpResponseMessage, ApplicationContext, IState> createState)
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

        Result<IState> IRule.Evaluate(HttpResponseMessage response, ApplicationContext context)
        {
            if (!condition())
            {
                return new Result<IState>(false, null);
            }

            var result = createResponseHandler().Handle(response, context);

            if (!result.IsSuccessful)
            {
                return new Result<IState>(false, null);
            }

            contextAction(context);

            var state = createState(result.Value, context);

            if (state == null)
            {
                throw new NullStateException();
            }

            return new Result<IState>(true, state);
        }
    }
}