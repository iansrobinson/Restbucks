using System;
using System.Net.Http;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient
{
    public class PrepopulatedFormDataStrategy : IFormDataStrategy
    {
        private readonly Form form;

        public PrepopulatedFormDataStrategy(Form form)
        {
            this.form = form;
        }

        public HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationContext context)
        {
            throw new NotImplementedException();
        }
    }
}