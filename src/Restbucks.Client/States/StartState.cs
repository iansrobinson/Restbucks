using System.Net.Http;
using System.Reflection;
using log4net;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.RulesEngine;

namespace Restbucks.Client.States
{
    public class StartState : IState
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IResponseHandlerProvider responseHandlers;
        private readonly ApplicationContext context;
        private readonly HttpResponseMessage response;

        public StartState(IResponseHandlerProvider responseHandlers, ApplicationContext context, HttpResponseMessage response)
        {
            this.responseHandlers = responseHandlers;
            this.context = context;
            this.response = response;
        }

        public IState HandleResponse()
        {
            Log.Info("Start...");

            var rules = new Rules(
                When.IsTrue(IsUninitialized)
                    .InvokeHandler<UninitializedResponseHandler>()
                    .SetContext("started")
                    .ReturnState(NewStartState),
                When.IsTrue(IsStarted)
                    .InvokeHandler<StartedResponseHandler>()
                    .SetContext("http://relations.restbucks.com/rfq")
                    .ReturnState(NewStartState));

            return rules.Evaluate(responseHandlers, context, response);
        }

        private static StartState NewStartState(IResponseHandlerProvider h, ApplicationContext c, HttpResponseMessage r)
        {
            return new StartState(h, c, r);
        }

        private bool IsStarted()
        {
            return context.Get<string>(ApplicationContextKeys.ContextName).Equals("started");
        }

        private bool IsUninitialized()
        {
            return !context.ContainsKey(ApplicationContextKeys.ContextName);
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}