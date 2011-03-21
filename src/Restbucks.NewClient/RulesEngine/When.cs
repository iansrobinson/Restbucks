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

        public static IExecuteAction IsTrue<T>() where T : ICondition, new()
        {
            return new When(new T());
        }

        public static IExecuteAction IsTrue(Condition condition)
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

        public IRule ReturnState(CreateState createState)
        {
            return new Rule(condition, actionInvoker, new StateFactory(createState));
        }

        public IRule Return(IEnumerable<StateCreationRule> stateCreationRules, CreateState defaultCreateState = null)
        {
            Check.IsNotNull(stateCreationRules, "stateCreationRules");

            if (defaultCreateState == null)
            {
                return new Rule(condition, actionInvoker, new StateFactoryCollection(stateCreationRules));
            }

            return new Rule(condition, actionInvoker, new StateFactoryCollection(stateCreationRules, new StateFactory(defaultCreateState)));
        }

        private class ResponseBasedCondition : ICondition
        {
            private readonly Condition condition;

            public ResponseBasedCondition(Condition condition)
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
        IReturnState ExecuteAction(IActionInvoker actionInvoker);    }

    public interface IReturnState
    {
        IRule ReturnState(CreateState createState);
        IRule Return(IEnumerable<StateCreationRule> stateCreationRules, CreateState defaultCreateState = null);
    }
}