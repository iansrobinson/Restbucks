using System.Collections.Generic;
using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class When : IExecuteAction, IReturnState
    {
        private readonly ICondition condition;
        private IRequestAction requestAction;

        public static IExecuteAction IsTrue(ResponseConditionDelegate responseConditionDelegate)
        {
            return new When(new ResponseBasedCondition(responseConditionDelegate));
        }

        private When(ICondition condition)
        {
            this.condition = condition;
        }

        public IReturnState Execute(CreateRequestActionDelegate createRequestActionDelegate)
        {
            requestAction = new RequestAction(createRequestActionDelegate);
            return this;
        }

        public IRule ReturnState(CreateNextStateDelegate createNextStateDelegate)
        {
            return new Rule(condition, requestAction, new CreateNextState(createNextStateDelegate));
        }

        public IRule Return(IEnumerable<StateCreationRule> stateCreationRules, CreateNextStateDelegate defaultCreateNextStateDelegate = null)
        {
            Check.IsNotNull(stateCreationRules, "stateCreationRules");

            if (defaultCreateNextStateDelegate == null)
            {
                return new Rule(condition, requestAction, new StateFactoryCollection(stateCreationRules));
            }

            var stateFactoryCollection = new StateFactoryCollection(stateCreationRules, (r, v, c) => defaultCreateNextStateDelegate(r, v));
            return new Rule(condition, requestAction, stateFactoryCollection);
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

        private class RequestAction : IRequestAction
        {
            private readonly CreateRequestActionDelegate createRequestActionDelegate;

            public RequestAction(CreateRequestActionDelegate createRequestActionDelegate)
            {
                this.createRequestActionDelegate = createRequestActionDelegate;
            }

            public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
            {
                var generateNextRequest = createRequestActionDelegate(new Actions(clientCapabilities));
                return generateNextRequest.Execute(previousResponse, stateVariables, clientCapabilities);
            }
        }

        private class CreateNextState : ICreateNextState
        {
            private readonly CreateNextStateDelegate createNextStateDelegate;

            public CreateNextState(CreateNextStateDelegate createNextStateDelegate)
            {
                this.createNextStateDelegate = createNextStateDelegate;
            }

            public IState Execute(HttpResponseMessage currentResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
            {
                return createNextStateDelegate(currentResponse, stateVariables);
            }
        }
    }

    public interface IExecuteAction
    {
        IReturnState Execute(CreateRequestActionDelegate createRequestActionDelegate);
    }

    public interface IReturnState
    {
        IRule ReturnState(CreateNextStateDelegate createNextStateDelegate);
        IRule Return(IEnumerable<StateCreationRule> stateCreationRules, CreateNextStateDelegate defaultCreateNextStateDelegate = null);
    }
}