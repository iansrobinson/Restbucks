using System;
using System.Net.Http;
using System.Reflection;
using log4net;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.RulesEngine;

namespace Restbucks.Client.States
{
    public class StartedState : IState
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IResponseHandlerProvider responseHandlers;
        private readonly ApplicationContext context;
        private readonly HttpResponseMessage response;

        public StartedState(IResponseHandlerProvider responseHandlers, ApplicationContext context, HttpResponseMessage response)
        {
            this.responseHandlers = responseHandlers;
            this.context = context;
            this.response = response;
        }

        public IState Apply()
        {
            Log.Info("Started...");

            var rules = new Rules(
                When.IsTrue(IsUninitialized)
                    .InvokeHandler<UninitializedResponseHandler>()
                    .UpdateContext(SetSemanticContext(SemanticContext.Started))
                    .ReturnState(NewStartState),
                When.IsTrue(IsStarted)
                    .InvokeHandler<StartedResponseHandler>()
                    .UpdateContext(SetSemanticContext(SemanticContext.Rfq))
                    .ReturnState(NewStartState),
                When.IsTrue(IsRfc)
                    .InvokeHandler<RequestForQuoteFormResponseHandler>()
                    .UpdateContext(ClearSemanticContext())
                    .ReturnState(NewQuoteRequestedState));

            return rules.Evaluate(responseHandlers, context, response);
        }

        private static Action<ApplicationContext> ClearSemanticContext()
        {
            return c => c.Remove(ApplicationContextKeys.SemanticContext);
        }

        private static Action<ApplicationContext> SetSemanticContext(string value)
        {
            return c => c.Set(ApplicationContextKeys.SemanticContext, value);
        }

        private static IState NewStartState(IResponseHandlerProvider h, ApplicationContext c, HttpResponseMessage r)
        {
            return new StartedState(h, c, r);
        }

        private static IState NewQuoteRequestedState(IResponseHandlerProvider h, ApplicationContext c, HttpResponseMessage r)
        {
            return new QuoteRequestedState(h, c, r);
        }

        private bool IsUninitialized()
        {
            return !context.ContainsKey(ApplicationContextKeys.SemanticContext);
        }

        private bool IsStarted()
        {
            return response.IsSuccessStatusCode
                   && context.Get<string>(ApplicationContextKeys.SemanticContext).Equals(SemanticContext.Started);
        }

        private bool IsRfc()
        {
            return response.IsSuccessStatusCode
                   && context.Get<string>(ApplicationContextKeys.SemanticContext).Equals(SemanticContext.Rfq);
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}