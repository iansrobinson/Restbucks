using System;
using System.Net;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class On
    {
        public static On Status(HttpStatusCode statusCode)
        {
            return new On(statusCode);
        }

        private readonly HttpStatusCode statusCode;

        private On(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
        }

        public StateCreationRule Do(Func<HttpResponseMessage, ApplicationContext, IState> createState)
        {
            return new StateCreationRule(new StatusCodeBasedCondition(statusCode), new StateFactory(createState));
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
    }
}