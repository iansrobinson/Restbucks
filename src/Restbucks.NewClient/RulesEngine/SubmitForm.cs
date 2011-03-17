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

            var request = new HttpRequestMessage
                              {
                                  RequestUri = formInfo.ResourceUri,
                                  Method = formInfo.Method,
                                  Content = formDataStrategy.CreateFormData(previousResponse, context)
                              };

            return clientCapabilities.HttpClient.Send(request);
        }
    }
}