using System;
using System.Net.Http;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public class When : IInvokeHandler, IUpdateContext, IReturnState
    {
        public static IInvokeHandler IsTrue(Func<bool> condition)
        {
            return new When(condition);
        }

        private readonly Func<bool> condition;
        private Type responseHandlerType;
        private Action<ApplicationContext> contextAction;

        private When(Func<bool> condition)
        {
            this.condition = condition;
        }

        public Rule InvokeHandler<T>(Actions actions) where T : IResponseHandler
        {
            responseHandlerType = typeof(T);
            return new Rule(condition, responseHandlerType, actions.ContextAction, actions.CreateState);
        }

        public IUpdateContext InvokeHandler<T>() where T : IResponseHandler
        {
            responseHandlerType = typeof (T);
            return this;
        }

        public IReturnState UpdateContext(Action<ApplicationContext> contextAction)
        {
            this.contextAction = contextAction;
            return this;
        }

        public Rule ReturnState(Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
        {
            return new Rule(condition, responseHandlerType, contextAction, createState);
        }
    }

    public interface IInvokeHandler
    {
        IUpdateContext InvokeHandler<T>() where T : IResponseHandler;
        Rule InvokeHandler<T>(Actions actions) where T : IResponseHandler;
    }

    public interface IUpdateContext
    {
        IReturnState UpdateContext(Action<ApplicationContext> contextAction);
    }

    public interface IReturnState
    {
        Rule ReturnState(Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState);
    }
}