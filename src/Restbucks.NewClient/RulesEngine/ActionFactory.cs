using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Net.Http;
using Restbucks.MediaType;

namespace Restbucks.NewClient.RulesEngine
{
    public class ActionFactory
    {
        private readonly HttpContentAdapter contentAdapter;
        private readonly HttpClient client;
        private readonly object items;
        
        public IAction SubmitForm(HttpResponseMessage response)
        {
            var entityBody = contentAdapter.CreateObject(response.Content);
            var shop = entityBody as Shop;

            var form = shop.Forms.First();

            var formInfo = new FormInfo(form.Resource, Enum.Parse(typeof(HttpMethod), form.Method) as HttpMethod, new MediaTypeHeaderValue(form.MediaType));
            
            return new SubmitFormAction(formInfo, items, contentAdapter, client);
        }
    }
}
