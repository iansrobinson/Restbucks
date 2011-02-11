using System;
using System.Net.Http;
using System.Reflection;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public class Rule
    {
        private readonly Func<bool> condition;
        private readonly Type responseHandlerType;
        private readonly string contextName;
        private readonly Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState;

        public Rule(Func<bool> condition, Type responseHandlerType, string contextName, Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
        {
            this.condition = condition;
            this.responseHandlerType = responseHandlerType;
            this.contextName = contextName;
            this.createState = createState;
        }

        public bool IsApplicable
        {
            get { return condition(); }
        }

        public Type ResponseHandlerType
        {
            get { return responseHandlerType; }
        }

        public string ContextName
        {
            get { return contextName; }
        }

        public Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> CreateState
        {
            get { return createState; }
        }
    }
}