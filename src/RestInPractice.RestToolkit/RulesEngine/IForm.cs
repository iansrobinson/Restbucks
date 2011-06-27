using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public interface IForm
    {
        FormInfo GetFormInfo(HttpResponseMessage response);
        IFormDataStrategy GetFormDataStrategy(HttpResponseMessage response);
        bool FormExists(HttpResponseMessage response);
    }
}