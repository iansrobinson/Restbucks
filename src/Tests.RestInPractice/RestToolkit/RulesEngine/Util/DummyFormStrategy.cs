using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Restbucks.RestToolkit.RulesEngine;
using Tests.RestInPractice.RestToolkit.Hacks;

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

            if (entityBody.Form.Id.Equals(formId))
            {
                return new FormInfo(new Uri(entityBody.Form.Uri), new HttpMethod(entityBody.Form.Method), new MediaTypeHeaderValue(entityBody.Form.ContentType));
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