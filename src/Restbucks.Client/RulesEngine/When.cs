using System;
using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public class When : IInvokeHandler, IUpdateContext, IReturnState
    {
        public static IInvokeHandler IsTrue(Func<bool> condition)
        {
            return new When(condition);
        }

        private readonly Func<bool> condition;
        private Func<IResponseHandler> createResponseHandler;
        private Action<ApplicationContext> contextAction;

        private When(Func<bool> condition)
        {
            this.condition = condition;
        }

        public IUpdateContext InvokeHandler(IResponseHandler responseHandler)
        {
            createResponseHandler = () => responseHandler;
            return this;
        }

        public IUpdateContext InvokeHandler(Func<IResponseHandler> createResponseHandler)
        {
            this.createResponseHandler = createResponseHandler;
            return this;
        }

        public IReturnState UpdateContext(Action<ApplicationContext> contextAction)
        {
            this.contextAction = contextAction;
            return this;
        }

        public Rule ReturnState(Func<HttpResponseMessage, ApplicationContext, IState> createState)
        {
            return new Rule(condition, createResponseHandler, contextAction, createState);
        }
    }

    public interface IInvokeHandler
    {
        IUpdateContext InvokeHandler(Func<IResponseHandler> createResponseHandler);
        IUpdateContext InvokeHandler(IResponseHandler responseHandler);
    }

    public interface IUpdateContext
    {
        IReturnState UpdateContext(Action<ApplicationContext> contextAction);
    }

    public interface IReturnState
    {
        Rule ReturnState(Func<HttpResponseMessage, ApplicationContext, IState> createState);
    }
}