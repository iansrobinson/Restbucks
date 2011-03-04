using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class Actions
    {
        private readonly HttpContentAdapter contentAdapter;
        private readonly ApplicationContext applicationContext;
        private readonly HttpClient client;


        public IAction SubmitForm(IFormStrategy formStrategy)
        {
            return new DoSubmitForm(formStrategy, contentAdapter, applicationContext, client);
        }

        private class DoSubmitForm : IAction
        {

            private readonly IFormStrategy formStrategy;
            private readonly HttpContentAdapter contentAdapter;
            private readonly HttpClient client;
            private readonly ApplicationContext applicationContext;

            public DoSubmitForm(IFormStrategy formStrategy, HttpContentAdapter contentAdapter, ApplicationContext applicationContext, HttpClient client)
            {
                this.formStrategy = formStrategy;
                this.contentAdapter = contentAdapter;
                this.client = client;
                this.applicationContext = applicationContext;
            }

            public HttpResponseMessage Execute(HttpResponseMessage previousResponse)
            {
                var entityBody = contentAdapter.CreateObject(previousResponse.Content);
                var formInfo = formStrategy.GetFormInfo(entityBody, applicationContext);

                var request = new HttpRequestMessage
                                  {
                                      RequestUri = formInfo.ResourceUri,
                                      Method = formInfo.Method,
                                      Content = contentAdapter.CreateContent(formInfo.FormData, formInfo.ContentType)
                                  };

                if (formInfo.Etag != null)
                {
                    request.Headers.IfMatch.Add(formInfo.Etag);
                }

                return client.Send(request);
            }
        }
    }
}