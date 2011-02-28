using System;

namespace Restbucks.Client.States
{
    public class TerminalState : IState
    {
        public IState NextState(IResponseHandlers handlers)
        {
            throw new InvalidOperationException();
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}