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

        public FormInfo GetFormInfo(HttpResponseMessage response, ApplicationContext context, HttpContentAdapter contentAdapter)
        {
            var entityBody = contentAdapter.CreateObject(response.Content);
            var form = (from f in ((Shop) entityBody).Forms
                        where f.Id.Equals(id)
                        select f).FirstOrDefault();

            if (form == null)
            {
                throw new FormNotFoundException(string.Format("Could not find form with id '{0}'.", id));
            }

            var formData = form.Instance ?? context.Input;
            var contentType = new MediaTypeHeaderValue(form.MediaType);
            var content = contentAdapter.CreateContent(formData, contentType);

            return new FormInfo(form.Resource, new HttpMethod(form.Method), new MediaTypeHeaderValue(form.MediaType), null, content);
        }

        private Form GetForm(object entityBody)
        {
            var form = (from f in ((Shop) entityBody).Forms
                        where f.Id.Equals(id)
                        select f).FirstOrDefault();

            if (form == null)
            {
                throw new FormNotFoundException(string.Format("Could not find form with id '{0}'.", id));
            }

            return form;
        }
    }
}