using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IFormStrategy
    {
        FormInfo GetFormInfo(HttpResponseMessage response, HttpContentAdapter contentAdapter, ApplicationContext context);
    }
}