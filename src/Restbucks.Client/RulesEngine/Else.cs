using System;
using System.Net.Http;
using Restbucks.Client.States;

namespace Restbucks.Client.RulesEngine
{
    public static class Else
    {
        public static IElse UpdateContext(Action<ApplicationContext> contextAction)
        {
            return new InnerElse(contextAction);
        }

        public static ElseRule ReturnState(Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState)
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

            ElseRule IElse.ReturnState(Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState)
            {
                return new ElseRule(contextAction, createState);
            }

            ElseRule IElse.Terminate()
            {
                return ReturnState((m, c, p) => new TerminalState());
            }
        }
    }

    public interface IElse
    {
        ElseRule ReturnState(Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState);
        ElseRule Terminate();
    }
}