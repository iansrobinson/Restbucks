using System;
using System.Collections.Generic;
using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.NewClient.RulesEngine
{
    public class When : IExecuteAction, IReturnState
    {
        private readonly ICondition condition;
        private IActionInvoker actionInvoker;
        private Func<Actions, IActionInvoker> createActionInvoker;

        public static IExecuteAction IsTrue<T>() where T : ICondition, new()
        {
            return new When(new T());
        }

        public static IExecuteAction IsTrue(Func<HttpResponseMessage, bool> condition)
        {
            return new When(new ResponseBasedCondition(condition));
        }

        private When(ICondition condition)
        {
            this.condition = condition;
        }

        public IReturnState ExecuteAction(IActionInvoker actionInvoker)
        {
            this.actionInvoker = actionInvoker;
            return this;
        }

        public IReturnState ExecuteAction(Func<Actions, IActionInvoker> createActionInvoker)
        {
            this.createActionInvoker = createActionInvoker;
            return this;
        }

        public IRule ReturnState(Func<HttpResponseMessage, ApplicationContext, IState> createState)
        {
            if (actionInvoker != null)
            {
                return new Rule(condition, actionInvoker, new StateFactory(createState));
            }

            return new Rule(condition, createActionInvoker, new StateFactory(createState));
        }

        public IRule Return(IEnumerable<StateCreationRule> stateCreationRules, Func<HttpResponseMessage, ApplicationContext, IState> defaultCreateState = null)
        {
            Check.IsNotNull(stateCreationRules, "stateCreationRules");

            if (actionInvoker != null)
            {
                if (defaultCreateState == null)
                {
                    return new Rule(condition, actionInvoker, new StateFactoryCollection(stateCreationRules));
                }

                return new Rule(condition, actionInvoker, new StateFactoryCollection(stateCreationRules, new StateFactory(defaultCreateState)));
            }

            if (defaultCreateState == null)
            {
                return new Rule(condition, createActionInvoker, new StateFactoryCollection(stateCreationRules));
            }

            return new Rule(condition, createActionInvoker, new StateFactoryCollection(stateCreationRules, new StateFactory(defaultCreateState)));
        }

        private class ResponseBasedCondition : ICondition
        {
            private readonly Func<HttpResponseMessage, bool> condition;

            public ResponseBasedCondition(Func<HttpResponseMessage, bool> condition)
            {
                this.condition = condition;
            }

            public bool IsApplicable(HttpResponseMessage response, ApplicationContext context)
            {
                return condition(response);
            }
        }
    }

    public interface IExecuteAction
    {
        IReturnState ExecuteAction(IActionInvoker actionInvoker);
        IReturnState ExecuteAction(Func<Actions, IActionInvoker> createActionInvoker);
    }

    public interface IReturnState
    {
        IRule ReturnState(Func<HttpResponseMessage, ApplicationContext, IState> createState);
        IRule Return(IEnumerable<StateCreationRule> stateCreationRules, Func<HttpResponseMessage, ApplicationContext, IState> defaultCreateState = null);
    }
}