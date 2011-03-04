using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class ApplicationFormInfoFactory : IFormInfoFactory
    {
        private readonly IFormStrategy formStrategy;
        private readonly ApplicationContext context;
        private readonly HttpContentAdapter contentAdapter;

        public ApplicationFormInfoFactory(IFormStrategy formStrategy, ApplicationContext context, HttpContentAdapter contentAdapter)
        {
            this.formStrategy = formStrategy;
            this.context = context;
            this.contentAdapter = contentAdapter;
        }

        public FormInfo CreateFormInfo(HttpResponseMessage response)
        {
            return formStrategy.GetFormInfo(response, context, contentAdapter);
        }
    }
}