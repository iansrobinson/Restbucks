using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;
using Restbucks.RestToolkit.Utils;

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

        public FormInfo GetFormInfo(object entityBody)
        {
            var form = GetForm(entityBody);
            return new FormInfo(form.Resource, new HttpMethod(form.Method), new MediaTypeHeaderValue(form.MediaType));
        }

        public object GetFormData(object entityBody, object input)
        {
            Check.IsNotNull(input, "input");
            
            var form = GetForm(entityBody);
            return form.Instance ?? input;
        }

        private Form GetForm(object entityBody)
        {
            Check.IsNotNull(entityBody, "entityBody");

            var form = (from f in ((Shop)entityBody).Forms
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