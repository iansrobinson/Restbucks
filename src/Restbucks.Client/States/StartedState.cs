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

        private readonly IHttpClientProvider clientProvider;
        private readonly ApplicationContext context;
        private readonly HttpResponseMessage response;

        public StartedState(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            this.response = response;
            this.context = context;
            this.clientProvider = clientProvider;
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

            return rules.Evaluate(response, context, clientProvider);
        }

        private static Action<ApplicationContext> ClearSemanticContext()
        {
            return c => c.Remove(ApplicationContextKeys.SemanticContext);
        }

        private static Action<ApplicationContext> SetSemanticContext(string value)
        {
            return c => c.Set(ApplicationContextKeys.SemanticContext, value);
        }

        private static IState NewStartState(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            return new StartedState(response, context, clientProvider);
        }

        private static IState NewQuoteRequestedState(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            return new QuoteRequestedState(response, context, clientProvider);
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