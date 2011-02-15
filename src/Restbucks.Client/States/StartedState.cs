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

        public IState HandleResponse()
        {
            Log.Info("Started...");

            var rules = new Rules(
                When.IsTrue(IsUninitialized)
                    .InvokeHandler<UninitializedResponseHandler>()
                    .UpdateContext(SetContextName(ContextNames.Started))
                    .ReturnState(NewStartState),
                When.IsTrue(IsStarted)
                    .InvokeHandler<StartedResponseHandler>()
                    .UpdateContext(SetContextName(ContextNames.Rfq))
                    .ReturnState(NewStartState),
                When.IsTrue(IsRfc)
                    .InvokeHandler<RequestForQuoteFormResponseHandler>()
                    .UpdateContext(ClearContextName())
                    .ReturnState(NewQuoteRequestedState));

            return rules.Evaluate(responseHandlers, context, response);
        }

        private static Action<ApplicationContext> ClearContextName()
        {
            return c => c.Remove(ApplicationContextKeys.ContextName);
        }

        private static Action<ApplicationContext> SetContextName(string value)
        {
            return c => c.Set(ApplicationContextKeys.ContextName, value);
        }

        private static IState NewStartState(IResponseHandlerProvider h, ApplicationContext c, HttpResponseMessage r)
        {
            return new StartedState(h, c, r);
        }

        private static IState NewQuoteRequestedState(IResponseHandlerProvider h, ApplicationContext c, HttpResponseMessage r)
        {
            return new QuoteRequestedState();
        }

        private bool IsUninitialized()
        {
            return !context.ContainsKey(ApplicationContextKeys.ContextName);
        }

        private bool IsStarted()
        {
            return context.Get<string>(ApplicationContextKeys.ContextName).Equals(ContextNames.Started)
                   && response.IsSuccessStatusCode;
        }

        private bool IsRfc()
        {
            return context.Get<string>(ApplicationContextKeys.ContextName).Equals(ContextNames.Rfq)
                   && response.IsSuccessStatusCode;
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}