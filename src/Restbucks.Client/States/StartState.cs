using System;
using Restbucks.Client.Actions;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Http;

namespace Restbucks.Client.States
{
    public class StartState : IState
    {
        private readonly ApplicationContext context;
        private readonly Response<Shop> response;

        public StartState(ApplicationContext context, Response<Shop> response)
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