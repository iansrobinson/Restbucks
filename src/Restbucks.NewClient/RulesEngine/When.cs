using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public static IExecuteAction IsTrue(Func<HttpResponseMessage, bool> condition)
        {
            return new When(new ConditionWrapper(condition));
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

        public IRule ReturnState(Func<HttpResponseMessage, IState> createState)
        {
            return new Rule(condition, actionInvoker, new StateFactory(createState));
        }

        public IRule Return(IEnumerable<KeyValuePair<HttpStatusCode, IStateFactory>> createStateByStatusCode, Func<HttpResponseMessage, IState> defaultCreateState = null)
        {
            Check.IsNotNull(createStateByStatusCode, "createStateByStatusCode");

            var stateFactories = new Dictionary<HttpStatusCode, IStateFactory>();
            createStateByStatusCode.ToList().ForEach(kv => stateFactories.Add(kv.Key, kv.Value));

            if (defaultCreateState == null)
            {
                return new Rule(condition, actionInvoker, new StateFactoryCollection(stateFactories));
            }

            return new Rule(condition, actionInvoker, new StateFactoryCollection(stateFactories, new StateFactory(defaultCreateState)));
        }

        private class ConditionWrapper : ICondition
        {
            private readonly Func<HttpResponseMessage, bool> condition;

            public ConditionWrapper(Func<HttpResponseMessage, bool> condition)
            {
                this.condition = condition;
            }

            public bool IsApplicable(HttpResponseMessage response)
            {
                return condition(response);
            }
        }
    }

    public interface IExecuteAction
    {
        IReturnState ExecuteAction(IActionInvoker actionInvoker);
    }

    public interface IReturnState
    {
        IRule ReturnState(Func<HttpResponseMessage, IState> createState);
        IRule Return(IEnumerable<KeyValuePair<HttpStatusCode, IStateFactory>> createStateByStatusCode, Func<HttpResponseMessage, IState> defaultCreateState = null);
    }
}