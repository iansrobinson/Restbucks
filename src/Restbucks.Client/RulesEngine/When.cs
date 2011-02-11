using System;
using System.Net.Http;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public class When :IInvokeHandler, ISetContext, IReturnState
    {
        public static IInvokeHandler IsTrue(Func<bool> condition)
        {
            return new When(condition);
        }
        
        private readonly Func<bool> condition;
        private Type responseHandlerType;
        private string contextName;

        private When(Func<bool> condition)
        {
            this.condition = condition;
        }

        public ISetContext InvokeHandler<T>() where T : IResponseHandler
        {
            responseHandlerType = typeof (T);
            return this;
        }

        public IReturnState SetContext(string contextName)
        {
            this.contextName = contextName;
            return this;
        } 

        public Rule ReturnState(Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
        {
            return new Rule(condition, responseHandlerType, contextName, createState);
        }
    }

    public interface IInvokeHandler
    {
        ISetContext InvokeHandler<T>() where T : IResponseHandler;
    }

    public interface ISetContext
    {
        IReturnState SetContext(string contextName);
    }

    public interface IReturnState
    {
        Rule ReturnState(Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState);
    }
}