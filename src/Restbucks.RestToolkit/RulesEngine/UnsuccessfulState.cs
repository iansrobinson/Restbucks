using System;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class UnsuccessfulState : IState
    {
        public static readonly IState Instance = new UnsuccessfulState();

        private UnsuccessfulState()
        {
        }

        public IState NextState(IClientCapabilities clientCapabilities)
        {
            throw new NotImplementedException();
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}