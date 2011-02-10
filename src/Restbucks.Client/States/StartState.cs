using System;
using System.Net.Http;
using Microsoft.Net.Http;
using Restbucks.Client.Actions;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;

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
            if (context.Get<string>(ApplicationContextKeys.ContextName).Equals("started"))
            {
                var entityBody = response.Content.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance);

            }
            return null;
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}