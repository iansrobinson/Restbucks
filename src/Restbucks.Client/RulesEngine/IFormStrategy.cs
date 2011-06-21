using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public interface IFormStrategy
    {
        FormInfo GetFormInfo(HttpResponseMessage response);
        IFormDataStrategy GetFormDataStrategy(HttpResponseMessage response);
        bool FormExists(HttpResponseMessage response);
    }
}