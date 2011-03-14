using System;
using System.Net.Http;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient
{
    public class ApplicationContextFormDataStrategy : IFormDataStrategy
    {
        public HttpContent CreateFormData(HttpResponseMessage previousResponse)
        {
            throw new NotImplementedException();
        }
    }
}