using System;

namespace Restbucks.Client.States
{
    public class QuoteRequestedState : IState
    {
        public IState HandleResponse()
        {
            throw new NotImplementedException();
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}