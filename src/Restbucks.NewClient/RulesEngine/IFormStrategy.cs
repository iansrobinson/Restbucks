using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IFormStrategy
    {
        FormInfo GetFormInfo(HttpResponseMessage response);
        bool TryGetFormInfo(HttpResponseMessage response, out FormInfo formInfo);
        bool FormExists(HttpResponseMessage response);
    }
}