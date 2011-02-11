using System.Net.Http;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.States
{
    public class StartState : IState
    {
        private readonly IResponseHandlerProvider responseHandlers;
        private readonly ApplicationContext context;
        private readonly HttpResponseMessage response;

        public StartState(IResponseHandlerProvider responseHandlers, ApplicationContext context, HttpResponseMessage response)
        {
            this.responseHandlers = responseHandlers;
            this.context = context;
            this.response = response;
        }

        public IState Apply()
        {
            if (!context.ContainsKey(ApplicationContextKeys.ContextName))
            {
                var handler = responseHandlers.GetFor<InitializedResponseHandler>();
                var result = handler.Handle(response, context);
                if (result.IsSuccessful)
                {
                    context.Set(ApplicationContextKeys.ContextName, "started");
                    return new StartState(responseHandlers, context, result.Response);
                }
            }

            if (context.Get<string>(ApplicationContextKeys.ContextName).Equals("started"))
            {
                var handler = responseHandlers.GetFor<StartedResponseHandler>();
                var result = handler.Handle(response, context);
                if (result.IsSuccessful)
                {
                    context.Set(ApplicationContextKeys.ContextName, "http://relations.restbucks.com/rfq");
                    return new StartState(responseHandlers, context, result.Response);
                }
            }

            return null;
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}