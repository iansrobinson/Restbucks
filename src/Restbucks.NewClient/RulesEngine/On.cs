using System;
using System.Collections.Generic;
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

        public KeyValuePair<HttpStatusCode, IStateFactory> Do(Func<HttpResponseMessage, ApplicationContext, IState> createState)
        {
            return new KeyValuePair<HttpStatusCode, IStateFactory>(statusCode, new StateFactory(createState));
        }
    }
}