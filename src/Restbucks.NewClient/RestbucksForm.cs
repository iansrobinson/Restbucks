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
            var entityBody = response.Content.ReadAsObject<Shop>(RestbucksFormatter.Instance);
            var form = (from f in entityBody.Forms
                        where f.Id.Equals(id)
                        select f).FirstOrDefault();

            if (form == null)
            {
                formInfo = null;
                return false;
            }

            var resourceUri = form.Resource.IsAbsoluteUri ? form.Resource : new Uri(entityBody.BaseUri, form.Resource);

            formInfo = new FormInfo(resourceUri, new HttpMethod(form.Method), new MediaTypeHeaderValue(form.MediaType));
            return true;
        }
    }
}