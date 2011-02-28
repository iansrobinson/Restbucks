using System;
using System.Net.Http;

namespace Restbucks.NewClient
{
    public class When : IExecuteAction, IReturnState
    {
        private readonly ICondition condition;
        private IAction action;

        public static IExecuteAction IsTrue<T>() where T : ICondition, new()
        {
            return new When(new T());
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

        private class StateFactory : IStateFactory
        {
            private readonly Func<HttpResponseMessage, IState> createState;

            public StateFactory(Func<HttpResponseMessage, IState> createState)
            {
                this.createState = createState;
            }

            public IState Create(HttpResponseMessage response)
            {
                return createState(response);
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
    }
}