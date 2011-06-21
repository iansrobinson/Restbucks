using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface IFormStrategy
    {
        FormInfo GetFormInfo(HttpResponseMessage response);
        IFormDataStrategy GetFormDataStrategy(HttpResponseMessage response);
        bool FormExists(HttpResponseMessage response);
    }
}