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

        public HttpResponseMessage Execute(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            var formInfo = formStrategy.GetFormInfo(response);
            var formDataStrategy = formStrategy.GetFormDataStrategy(response);

            var request = new HttpRequestMessage
                              {
                                  RequestUri = formInfo.ResourceUri,
                                  Method = formInfo.Method,
                                  Content = formDataStrategy.CreateFormData(response, stateVariables, clientCapabilities)
                              };

            return clientCapabilities.GetHttpClient().Send(request);
        }
    }
}