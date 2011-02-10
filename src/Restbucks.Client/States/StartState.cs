using System;
using System.Net.Http;
using Restbucks.Client.Actions;

namespace Restbucks.Client.States
{
    public class StartState : IState
    {
        private readonly ApplicationContext context;
        private readonly HttpResponseMessage response;

        public StartState(ApplicationContext context, HttpResponseMessage response)
        {
            this.context = context;
            this.response = response;
        }

        public IState Apply(IHttpClientProvider clientProvider)
        {
            if (!context.ContainsKey(ApplicationContextKeys.ContextName))
            {
                var result = new Initialize(clientProvider, context.Get<Uri>(ApplicationContextKeys.EntryPointUri)).GetEntryPoint();
                if (result.IsSuccessful)
                {
                    context.Set(ApplicationContextKeys.ContextName, "started");
                    return new StartState(context, result.Response);
                }
            }
            if (context.Get<string>(ApplicationContextKeys.ContextName).Equals("started"))
            {
                var result = new Rfq(clientProvider, response).GetRfq();
                if (result.IsSuccessful)
                {
                    context.Set(ApplicationContextKeys.ContextName, "http://relations.restbucks.com/rfq");
                    return new StartState(context, result.Response);
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