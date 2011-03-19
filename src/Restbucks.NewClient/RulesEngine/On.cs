using System;
using System.Net;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class On
    {
        public static On Status(HttpStatusCode statusCode)
        {
            return new On(new StatusCodeBasedCondition(statusCode));
        }

        public static On Response(Func<HttpResponseMessage, bool> responseCondition)
        {
            return new On(new ResponseBasedCondition(responseCondition));
        }

        public static On Response(Func<HttpResponseMessage, ApplicationContext, bool> responseCondition)
        {
            return new On(new ResponseAndContextBasedCondition(responseCondition));
        }

        private readonly ICondition condition;

        public On(ICondition condition)
        {
            this.condition = condition;
        }

        public StateCreationRule Do(CreateState createState)
        {
            return new StateCreationRule(condition, new StateFactory(createState));
        }

        private class StatusCodeBasedCondition : ICondition
        {
            private readonly HttpStatusCode statusCode;

            public StatusCodeBasedCondition(HttpStatusCode statusCode)
            {
                this.statusCode = statusCode;
            }

            public bool IsApplicable(HttpResponseMessage response, ApplicationContext context)
            {
                return response.StatusCode.Equals(statusCode);
            }
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

        private class ResponseAndContextBasedCondition : ICondition
        {
            private readonly Func<HttpResponseMessage, ApplicationContext, bool> condition;

            public ResponseAndContextBasedCondition(Func<HttpResponseMessage, ApplicationContext, bool> condition)
            {
                this.condition = condition;
            }

            public bool IsApplicable(HttpResponseMessage response, ApplicationContext context)
            {
                return condition(response, context);
            }
        }
    }
}