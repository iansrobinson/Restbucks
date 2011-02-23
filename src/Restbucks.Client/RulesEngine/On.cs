using System;
using System.Net.Http;
using Restbucks.Client.ResponseHandlers;

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

        public Actions ReturnState(Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
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
        Actions ReturnState(Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState);
    }

    public class Actions
    {
        private readonly Action<ApplicationContext> contextAction;
        private readonly Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState;

        public Actions(Action<ApplicationContext> contextAction, Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
        {
            this.contextAction = contextAction;
            this.createState = createState;
        }

        public Action<ApplicationContext> ContextAction
        {
            get { return contextAction; }
        }

        public Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> CreateState
        {
            get { return createState; }
        }
    }
}