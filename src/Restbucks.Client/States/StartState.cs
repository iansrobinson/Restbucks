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
                var result = new RequestEntryPoint(clientProvider, context.Get<Uri>(ApplicationContextKeys.EntryPointUri)).Execute();
                if (result.IsSuccessful)
                {
                    context.Set(ApplicationContextKeys.ContextName, "started");
                    return new StartState(context, result.Response);
                }
            }
            return null;
        }

        public ApplicationContext Context
        {
            get { return context; }
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}