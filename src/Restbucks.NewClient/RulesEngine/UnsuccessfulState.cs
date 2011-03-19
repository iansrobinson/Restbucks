using System;

namespace Restbucks.NewClient.RulesEngine
{
    public class UnsuccessfulState : IState
    {
        public static readonly IState Instance = new UnsuccessfulState();

        private UnsuccessfulState()
        {
        }

        public IState NextState()
        {
            throw new NotImplementedException();
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}