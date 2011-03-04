using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class ApplicationFormInfoFactory : IFormInfoFactory
    {
        private readonly IFormStrategy formStrategy;       
        private readonly HttpContentAdapter contentAdapter;
        private readonly ApplicationContext context;

        public ApplicationFormInfoFactory(IFormStrategy formStrategy, HttpContentAdapter contentAdapter, ApplicationContext context)
        {
            this.formStrategy = formStrategy;         
            this.contentAdapter = contentAdapter;
            this.context = context;
        }

        public FormInfo CreateFormInfo(HttpResponseMessage response)
        {
            return formStrategy.GetFormInfo(response, contentAdapter, context);
        }
    }
}