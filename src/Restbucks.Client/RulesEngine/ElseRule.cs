using System;
using System.Net.Http;
using System.Reflection;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public class ElseRule : IRule
    {
        private readonly Action<ApplicationContext> contextAction;
        private readonly Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState;

        public ElseRule(Action<ApplicationContext> contextAction, Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
        {
            this.contextAction = contextAction;
            this.createState = createState;
        }

        bool IRule.IsApplicable
        {
            get { return true; }
        }

        HandlerResult IRule.Evaluate(MethodInfo getResponseHandler, IResponseHandlerProvider responseHandlers, HttpResponseMessage response, ApplicationContext context)
        {
            return new HandlerResult(true, response);
        }

        IState IRule.CreateNewState(IResponseHandlerProvider responseHandlers, ApplicationContext context, HttpResponseMessage response)
        {
            contextAction(context);

            var state = createState(responseHandlers, context, response);

            if (state == null)
            {
                throw new NullStateException();
            }

            return state;
        }
    }
}