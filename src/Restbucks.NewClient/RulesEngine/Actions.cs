//using System;
//using System.Net.Http;
//
//namespace Restbucks.NewClient.RulesEngine
//{
//    public class Actions
//    {
//        private readonly HttpContentAdapter contentAdapter;
//        private readonly object input;
//        private readonly HttpClient client;
//
//        public IAction SubmitForm(IFormStrategy formStrategy)
//        {
//            var response = new HttpResponseMessage();
//            return CreateSubmitFormAction(response, formStrategy);
//        }
//
//        private IAction CreateSubmitFormAction(HttpResponseMessage response, IFormStrategy formStrategy)
//        {
//            var entityBody = contentAdapter.CreateObject(response.Content);
//            var formInfo = formStrategy.GetFormInfo(entityBody, input);
//            return new SubmitFormAction(formInfo, contentAdapter, client);
//        }
//
//        private class DeferredAction : IAction
//        {
//            private readonly Func<HttpRequestMessage, HttpResponseMessage> action;
//
//            public HttpResponseMessage Execute()
//            {
//                throw new NotImplementedException();
//            }
//        }
//    }
//}