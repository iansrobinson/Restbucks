using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class SubmitForm : IRequestAction
    {
        private readonly IFormStrategy formStrategy;

        public SubmitForm(IFormStrategy formStrategy)
        {
            this.formStrategy = formStrategy;
        }

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            var formInfo = formStrategy.GetFormInfo(previousResponse);
            var formDataStrategy = formStrategy.GetFormDataStrategy(previousResponse);

            var request = new HttpRequestMessage
                              {
                                  RequestUri = formInfo.ResourceUri,
                                  Method = formInfo.Method,
                                  Content = formDataStrategy.CreateFormData(previousResponse, stateVariables, clientCapabilities)
                              };

            return clientCapabilities.GetHttpClient().Send(request);
        }
    }
}