using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public RestbucksForm(string id)
        {
            this.id = id;
        }

        public FormInfo GetFormInfo(HttpResponseMessage response, HttpContentAdapter contentAdapter, ApplicationContext context)
        {
            FormInfo formInfo;
            var success = TryGetFormInfo(response, contentAdapter, context, out formInfo);
            
            if (!success)
            {
                throw new ControlNotFoundException(string.Format("Could not find form with id '{0}'.", id));
            }

            return formInfo;
        }

        public bool TryGetFormInfo(HttpResponseMessage response, HttpContentAdapter contentAdapter, ApplicationContext context, out FormInfo formInfo)
        {
            var entityBody = (Shop)contentAdapter.CreateObject(response.Content);
            var form = (from f in entityBody.Forms
                        where f.Id.Equals(id)
                        select f).FirstOrDefault();

            if (form == null)
            {
                formInfo = null;
                return false;
            }

            var formData = form.Instance ?? context.Input;
            var contentType = new MediaTypeHeaderValue(form.MediaType);
            var content = contentAdapter.CreateContent(formData, contentType);
            var resourceUri = form.Resource.IsAbsoluteUri ? form.Resource : new Uri(entityBody.BaseUri, form.Resource);

            formInfo = new FormInfo(resourceUri, new HttpMethod(form.Method), new MediaTypeHeaderValue(form.MediaType), content);
            return true;
        }
    }
}