using System.Collections.Generic;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.NewClient.RulesEngine
{
    public class When : IExecuteAction, IReturnState
    {
        private readonly IsApplicableToStateInfoDelegate isApplicableDelegate;
        private IActionInvoker actionInvoker;

        public static IExecuteAction IsTrue<T>() where T : ICondition, new()
        {
            return new When(new T().IsApplicable);
        }

        public static IExecuteAction IsTrue(IsApplicableToResponseDelegate isApplicableToResponseDelegate)
        {
            return new When((response, variables) => isApplicableToResponseDelegate(response));
        }

        private When(IsApplicableToStateInfoDelegate isApplicableDelegate)
        {
            this.isApplicableDelegate = isApplicableDelegate;
        }

        public IReturnState ExecuteAction(IActionInvoker actionInvoker)
        {
            this.actionInvoker = actionInvoker;
            return this;
        }

        public IRule ReturnState(CreateStateDelegate createStateDelegate)
        {
            return new Rule(isApplicableDelegate, actionInvoker, new StateFactory(createStateDelegate));
        }

        public IRule Return(IEnumerable<StateCreationRule> stateCreationRules, CreateStateDelegate defaultCreateStateDelegate = null)
        {
            Check.IsNotNull(stateCreationRules, "stateCreationRules");

            if (defaultCreateStateDelegate == null)
            {
                return new Rule(isApplicableDelegate, actionInvoker, new StateFactoryCollection(stateCreationRules));
            }

            return new Rule(isApplicableDelegate, actionInvoker, new StateFactoryCollection(stateCreationRules, new StateFactory(defaultCreateStateDelegate)));
        }
    }

    public interface IExecuteAction
    {
        IReturnState ExecuteAction(IActionInvoker actionInvoker);
    }

    public interface IReturnState
    {
        IRule ReturnState(CreateStateDelegate createStateDelegate);
        IRule Return(IEnumerable<StateCreationRule> stateCreationRules, CreateStateDelegate defaultCreateStateDelegate = null);
    }
}