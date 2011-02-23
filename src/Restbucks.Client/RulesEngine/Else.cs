using System;
using System.Net.Http;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.States;

namespace Restbucks.Client.RulesEngine
{
    public static class Else
    {
        public static IElse UpdateContext(Action<ApplicationContext> contextAction)
        {
            return new InnerElse(contextAction);
        }

        public static IRule ReturnState(Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
        {
            return ((IElse) new InnerElse(c => { })).ReturnState(createState);
        }

        private class InnerElse : IElse
        {
            private readonly Action<ApplicationContext> contextAction;

            public InnerElse(Action<ApplicationContext> contextAction)
            {
                this.contextAction = contextAction;
            }

            IRule IElse.ReturnState(Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
            {
                return new ElseRule(contextAction, createState);
            }

            IRule IElse.Terminate()
            {
                return ReturnState((h, c, m) => new TerminalState());
            }
        }
    }

    public interface IElse
    {
        IRule ReturnState(Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState);
        IRule Terminate();
    }
}