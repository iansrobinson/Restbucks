using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IFormStrategy
    {
        FormInfo GetFormInfo(HttpResponseMessage response, ApplicationContext context, HttpContentAdapter contentAdapter);
    }
}