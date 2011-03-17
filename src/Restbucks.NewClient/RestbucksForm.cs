using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient
{
    public class RestbucksForm : IFormStrategy
    {
        public static IFormStrategy WithId(string id)
        {
            return new RestbucksForm(id);
        }

        private readonly string id;

        private RestbucksForm(string id)
        {
            this.id = id;
        }

        public FormInfo GetFormInfo(HttpResponseMessage response)
        {
            FormInfo formInfo;
            var success = TryGetFormInfo(response, out formInfo);
            
            if (!success)
            {
                throw new ControlNotFoundException(string.Format("Could not find form with id '{0}'.", id));
            }

            return formInfo;
        }

        public bool FormExists(HttpResponseMessage response)
        {
            FormInfo formInfo;
            return TryGetFormInfo(response, out formInfo);
        }

        private bool TryGetFormInfo(HttpResponseMessage response, out FormInfo formInfo)
        {
            var entityBody = GetEntityBody(response);
            var form = GetForm(id, entityBody);
            var dataStrategy = CreateDataStrategy(form);

            formInfo = GetFormInfo(entityBody.BaseUri, form, dataStrategy);

            return formInfo != null;
        }

        private static FormInfo GetFormInfo(Uri baseUri, Form form, IFormDataStrategy dataStrategy)
        {
            if (form == null)
            {
                return null;
            }
            
            var resourceUri = form.Resource.IsAbsoluteUri ? form.Resource : new Uri(baseUri, form.Resource);
            
            return new FormInfo(resourceUri, new HttpMethod(form.Method), new MediaTypeHeaderValue(form.MediaType), dataStrategy);
        }

        private static Form GetForm(string id, Shop entityBody)
        {
            return (from f in entityBody.Forms
                    where f.Id.Equals(id)
                    select f).FirstOrDefault();
        }

        private static Shop GetEntityBody(HttpResponseMessage response)
        {
            return response.Content.ReadAsObject<Shop>(RestbucksFormatter.Instance);
        }

        private static IFormDataStrategy CreateDataStrategy(Form form)
        {
            if (form == null)
            {
                return null;
            }

            var contentType = new MediaTypeHeaderValue(form.MediaType);

            if (form.Instance == null)
            {
                if (form.Schema == null)
                {
                    throw new InvalidOperationException(string.Format("Unable to create a data strategy for empty form with null schema attribute. Id: '{0}'.", form.Id));
                }
                return new ApplicationContextFormDataStrategy(new EntityBodyKey(form.Id, contentType, form.Schema), contentType);
            }

            return new PrepopulatedFormDataStrategy(form, contentType);
        }
    }
}