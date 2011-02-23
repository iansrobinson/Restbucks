using System;

namespace Restbucks.Client.States
{
    public class TerminalState : IState
    {
        public IState Apply()
        {
            throw new InvalidOperationException();
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}