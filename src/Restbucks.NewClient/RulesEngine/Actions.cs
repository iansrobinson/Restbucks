using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Restbucks.NewClient.RulesEngine
{
    public class Actions
    {
//        public object SubmitForm(IFormStrategy formStrategy)
//        {
//            Func<HttpResponseMessage> action = () => 
//        }

        private class DeferredAction : IAction
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> action;
            
            public HttpResponseMessage Execute(HttpResponseMessage previousResponse)
            {
                throw new NotImplementedException();
            }
        }
    }
}
