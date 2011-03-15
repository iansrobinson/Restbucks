using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient
{
    public class PrepopulatedFormDataStrategy : IFormDataStrategy
    {
        private readonly Form form;
        private readonly MediaTypeHeaderValue contentType;

        public PrepopulatedFormDataStrategy(Form form, MediaTypeHeaderValue contentType)
        {
            this.form = form;
            this.contentType = contentType;
        }

        public HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationContext context)
        {
            throw new NotImplementedException();
        }
    }
}