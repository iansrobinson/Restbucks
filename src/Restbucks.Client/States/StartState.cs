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
            userAgent.Invoke<Shop>(context.Get<Uri>(ApplicationContextKeys.EntryPointUri), null);
            return null;
        }

        public ApplicationContext Context
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}