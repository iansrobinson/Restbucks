using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class Actions
    {
        private readonly HttpContentAdapter contentAdapter;
        private readonly ApplicationContext context;
        private readonly HttpClient client;

        public Actions(HttpContentAdapter contentAdapter, ApplicationContext context, HttpClient client)
        {
            this.contentAdapter = contentAdapter;
            this.context = context;
            this.client = client;
        }

        public IAction SubmitForm(IFormStrategy formStrategy)
        {
            var formInfoFactory = new ApplicationFormInfoFactory(formStrategy, context, contentAdapter);
            return new SubmitForm(formInfoFactory, client);
        }
    }
}