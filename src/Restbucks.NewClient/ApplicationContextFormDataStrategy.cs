using System;
using System.Net.Http;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient
{
    public class ApplicationContextFormDataStrategy : IFormDataStrategy
    {
        private readonly EntityBodyKey key;

        public ApplicationContextFormDataStrategy(EntityBodyKey key)
        {
            this.key = key;
        }

        public HttpContent CreateFormData(HttpResponseMessage previousResponse)
        {
            throw new NotImplementedException();
        }
    }
}