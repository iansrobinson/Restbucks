using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class SubmitForm : IAction
    {
        private readonly IFormStrategy formStrategy;

        public SubmitForm(IFormStrategy formStrategy)
        {
            this.formStrategy = formStrategy;
        }

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationContext context, IClientCapabilities clientCapabilities)
        {
            var formInfo = formStrategy.GetFormInfo(previousResponse);
            var formDataStrategy = formStrategy.GetFormDataStrategy(previousResponse);

            var content = formDataStrategy.CreateFormData(previousResponse, context);
            //TODO get rid of setting content-type here
            content.Headers.ContentType = formInfo.ContentType;

            var request = new HttpRequestMessage
                              {
                                  RequestUri = formInfo.ResourceUri,
                                  Method = formInfo.Method,
                                  Content = content
                              };

            return clientCapabilities.HttpClient.Send(request);
        }
    }
}