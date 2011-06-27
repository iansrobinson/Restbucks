using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public class SubmitForm : IRequestAction
    {
        private readonly IForm form;

        public SubmitForm(IForm form)
        {
            this.form = form;
        }

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            var formInfo = form.GetFormInfo(previousResponse);
            var formDataStrategy = form.GetFormDataStrategy(previousResponse);

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