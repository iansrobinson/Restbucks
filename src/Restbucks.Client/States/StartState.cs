using System;
using Restbucks.MediaType;

namespace Restbucks.Client.States
{
    public class StartState : IState
    {
        private readonly ApplicationContext context;

        public StartState(ApplicationContext context)
        {
            this.context = context;
        }

        public IState Execute(IUserAgent userAgent)
        {
            var response = userAgent.Invoke<Shop>(context.Get<Uri>(ApplicationContextKeys.EntryPointUri), null);
            context.Set(ApplicationContextKeys.CurrentEntity, response.EntityBody);
            return new StartState(context);
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