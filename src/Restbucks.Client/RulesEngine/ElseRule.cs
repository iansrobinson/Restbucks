using System;
using System.Net.Http;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public class ElseRule : IRule
    {
        private readonly Action<ApplicationContext> contextAction;
        private readonly Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState;

        public ElseRule(Action<ApplicationContext> contextAction, Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
        {
            this.contextAction = contextAction;
            this.createState = createState;
        }

        bool IRule.IsApplicable
        {
            get { return true; }
        }

        Type IRule.ResponseHandlerType
        {
            get { return typeof (NullResponseHandler); }
        }

        Action<ApplicationContext> IRule.ContextAction
        {
            get { return contextAction; }
        }

        Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> IRule.CreateState
        {
            get { return createState; }
        }
    }
}