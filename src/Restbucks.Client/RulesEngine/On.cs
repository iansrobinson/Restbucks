using System;
using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public class On : IUpdateContextEx, IReturnStateEx
    {
        private Action<ApplicationContext> contextAction;

        public static IUpdateContextEx Success()
        {
            return new On();
        }

        private On()
        {
        }

        public IReturnStateEx UpdateContext(Action<ApplicationContext> contextAction)
        {
            this.contextAction = contextAction;
            return this;
        }

        public Actions ReturnState(Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState)
        {
            return new Actions(contextAction, createState);
        }
    }

    public interface IUpdateContextEx
    {
        IReturnStateEx UpdateContext(Action<ApplicationContext> contextAction);
    }

    public interface IReturnStateEx
    {
        Actions ReturnState(Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState);
    }

    public class Actions
    {
        private readonly Action<ApplicationContext> contextAction;
        private readonly Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState;

        public Actions(Action<ApplicationContext> contextAction, Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> createState)
        {
            this.contextAction = contextAction;
            this.createState = createState;
        }

        public Action<ApplicationContext> ContextAction
        {
            get { return contextAction; }
        }

        public Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> CreateState
        {
            get { return createState; }
        }
    }
}