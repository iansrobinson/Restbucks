﻿using System;
using System.Net.Http;
using Restbucks.Client.ResponseHandlers;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.Client.RulesEngine
{
    public class Rule : IRule
    {
        private readonly Func<bool> condition;
        private readonly Type responseHandlerType;
        private readonly Action<ApplicationContext> contextAction;
        private readonly Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState;

        public Rule(Func<bool> condition, Type responseHandlerType, Action<ApplicationContext> contextAction, Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> createState)
        {
            Check.IsNotNull(condition, "condition");
            Check.IsNotNull(contextAction, "contextAction");
            Check.IsNotNull(createState, "createState");
            
            this.condition = condition;
            this.responseHandlerType = responseHandlerType;
            this.contextAction = contextAction;
            this.createState = createState;
        }

        bool IRule.IsApplicable
        {
            get { return condition(); }
        }

        Type IRule.ResponseHandlerType
        {
            get { return responseHandlerType; }
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