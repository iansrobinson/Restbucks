using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IFormStrategy
    {
        FormInfo GetFormInfo(object entityBody, ApplicationContext applicationContext);
        FormInfo GetFormInfo(HttpResponseMessage response, ApplicationContext context, HttpContentAdapter contentAdapter);
    }
}