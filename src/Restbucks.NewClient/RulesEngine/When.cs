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
        private IAction action;

        public static IExecuteAction IsTrue<T>() where T : ICondition, new()
        {
            return new When(new T());
        }

        public static IExecuteAction IsTrue(params Func<HttpResponseMessage, bool>[] conditions)
        {
            return new When(new ConditionWrapper(conditions));
        }

        private When(ICondition condition)
        {
            this.condition = condition;
        }

        public IReturnState ExecuteAction(IAction action)
        {
            this.action = action;
            return this;
        }

        public IRule ReturnState(Func<HttpResponseMessage, IState> createState)
        {
            return new Rule(condition, action, new StateFactory(createState));
        }

        public IRule Return(IEnumerable<KeyValuePair<HttpStatusCode, IStateFactory>> createStateByStatusCode, Func<HttpResponseMessage, IState> defaultCreateState = null)
        {
            Check.IsNotNull(createStateByStatusCode, "createStateByStatusCode");

            var stateFactories = new Dictionary<HttpStatusCode, IStateFactory>();
            createStateByStatusCode.ToList().ForEach(kv => stateFactories.Add(kv.Key, kv.Value));

            if (defaultCreateState == null)
            {
                return new Rule(condition, action, new StateFactoryCollection(stateFactories));
            }

            return new Rule(condition, action, new StateFactoryCollection(stateFactories, new StateFactory(defaultCreateState)));
        }

        private class ConditionWrapper : ICondition
        {
            private readonly IEnumerable<Func<HttpResponseMessage, bool>> conditions;

            public ConditionWrapper(params Func<HttpResponseMessage, bool>[] conditions)
            {
                this.conditions = conditions;
            }

            public bool IsApplicable(HttpResponseMessage response)
            {
                return conditions.Aggregate(true, (result, condition) => result && condition(response), result => result);
            }
        }
    }

    public interface IExecuteAction
    {
        IReturnState ExecuteAction(IAction action);
    }

    public interface IReturnState
    {
        IRule ReturnState(Func<HttpResponseMessage, IState> createState);
        IRule Return(IEnumerable<KeyValuePair<HttpStatusCode, IStateFactory>> createStateByStatusCode, Func<HttpResponseMessage, IState> defaultCreateState = null);
    }
}