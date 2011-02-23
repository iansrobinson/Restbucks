using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Restbucks.Client.ResponseHandlers;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.Client.RulesEngine
{
    public class Rule : IRule
    {
        private readonly Func<bool> condition;
        private readonly Type responseHandlerType;
        private readonly Action<ApplicationContext> contextAction;
        private readonly Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState;

        public Rule(Func<bool> condition, Type responseHandlerType, Action<ApplicationContext> contextAction, Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(contextAction, "contextAction");
            Check.IsNotNull(createState, "createState");

            this.condition = condition;
            this.responseHandlerType = responseHandlerType;
            this.contextAction = contextAction;
            this.createState = createState;
        }

        HandlerResult IRule.Evaluate(MethodInfo getResponseHandler, IResponseHandlerProvider responseHandlers, HttpResponseMessage response, ApplicationContext context)
        {
            if (!condition())
            {
                return new HandlerResult(false, null);
            }
            
            var genericMethod = getResponseHandler.MakeGenericMethod(new[] {responseHandlerType});
            IResponseHandler handler;
            try
            {
                handler = genericMethod.Invoke(responseHandlers, null) as IResponseHandler;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException.GetType().Equals(typeof (KeyNotFoundException)))
                {
                    throw new ResponseHandlerMissingException(string.Format("Response handler missing. Type: [{0}].", responseHandlerType.FullName));
                }
                throw;
            }

            return handler.Handle(response, context);
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