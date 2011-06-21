﻿using System;
using System.Net;
using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public class On
    {
        private readonly ICondition condition;

        public static On Status(HttpStatusCode statusCode)
        {
            return new On(new Condition((response, context) => response.StatusCode.Equals(statusCode)));
        }

        public static On Response(ResponseConditionDelegate responseConditionDelegate)
        {
            return new On(new Condition((response, context) => responseConditionDelegate(response)));
        }

        public static On Response(StateConditionDelegate stateConditionDelegate)
        {
            return new On(new Condition((response, context) => stateConditionDelegate(response, context)));
        }

        public On(ICondition condition)
        {
            this.condition = condition;
        }

        public StateCreationRule Do(CreateNextStateDelegate createNextStateDelegate)
        {
            return new StateCreationRule(condition, (r, v, c) => createNextStateDelegate(r, v));
        }

        private class Condition : ICondition
        {
            private readonly Func<HttpResponseMessage, ApplicationStateVariables, bool> condition;

            public Condition(Func<HttpResponseMessage, ApplicationStateVariables, bool> condition)
            {
                this.condition = condition;
            }

            public bool IsApplicable(HttpResponseMessage response, ApplicationStateVariables stateVariables)
            {
                return condition(response, stateVariables);
            }
        }
    }
}