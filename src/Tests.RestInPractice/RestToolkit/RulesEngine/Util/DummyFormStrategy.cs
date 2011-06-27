using System;
using System.Net.Http;
using Restbucks.RestToolkit.RulesEngine;
using Tests.RestInPractice.RestToolkit.Hacks;
using Tests.RestInPractice.RestToolkit.Utils;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    public class DummyFormStrategy : IFormStrategy
    {
        private readonly string formId;

        public DummyFormStrategy(string formId)
        {
            this.formId = formId;
        }

        public FormInfo GetFormInfo(HttpResponseMessage response)
        {
            var entityBody = response.Content.ReadAsObject<DummyEntityBody>(DummyMediaType.Instance);
            if (entityBody.FormId.Equals(formId))
            {
                return new FormInfo(new Uri("http://localhost/resource-uri"), HttpMethod.Post, DummyMediaType.ContentType);
            }
            return null;
        }

        public IFormDataStrategy GetFormDataStrategy(HttpResponseMessage response)
        {
            return null;
        }

        public bool FormExists(HttpResponseMessage response)
        {
            return GetFormInfo(response) != null;
        }
    }
}