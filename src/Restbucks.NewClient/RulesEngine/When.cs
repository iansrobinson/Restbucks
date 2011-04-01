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

        public static IExecuteAction IsTrue(ResponseConditionDelegate responseConditionDelegate)
        {
            return new When(new ResponseBasedCondition(responseConditionDelegate));
        }

        private When(ICondition condition)
        {
            this.condition = condition;
        }

        public IReturnState Invoke(CreateActionDelegate createActionDelegate)
        {
            actionInvoker = new ActionInvoker(createActionDelegate);
            return this;
        }

        public IRule ReturnState(StateDelegate stateDelegate)
        {
            return new Rule(condition, actionInvoker, new StateFactory(stateDelegate));
        }

        public IRule Return(IEnumerable<StateCreationRule> stateCreationRules, StateDelegate defaultStateDelegate = null)
        {
            Check.IsNotNull(stateCreationRules, "stateCreationRules");

            if (defaultStateDelegate == null)
            {
                return new Rule(condition, actionInvoker, new StateFactoryCollection(stateCreationRules));
            }

            return new Rule(condition, actionInvoker, new StateFactoryCollection(stateCreationRules, new StateFactory(defaultStateDelegate)));
        }

        private class ResponseBasedCondition : ICondition
        {
            private readonly ResponseConditionDelegate responseConditionDelegate;

            public ResponseBasedCondition(ResponseConditionDelegate responseConditionDelegate)
            {
                this.responseConditionDelegate = responseConditionDelegate;
            }

            public bool IsApplicable(HttpResponseMessage response, ApplicationStateVariables stateVariables)
            {
                return responseConditionDelegate(response);
            }
        }

        private class ActionInvoker : IActionInvoker
        {
            private readonly CreateActionDelegate createActionDelegate;

            public ActionInvoker(CreateActionDelegate createActionDelegate)
            {
                this.createActionDelegate = createActionDelegate;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
            {
                var action = createActionDelegate(new Actions(clientCapabilities));
                return action.Execute(previousResponse, stateVariables, clientCapabilities);
            }
        }
    }

    public interface IExecuteAction
    {
        IReturnState Invoke(CreateActionDelegate createActionDelegate);
    }

    public interface IReturnState
    {
        IRule ReturnState(StateDelegate stateDelegate);
        IRule Return(IEnumerable<StateCreationRule> stateCreationRules, StateDelegate defaultStateDelegate = null);
    }
}