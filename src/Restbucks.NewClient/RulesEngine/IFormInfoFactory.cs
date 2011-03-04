using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IFormInfoFactory
    {
        FormInfo CreateFormInfo(HttpResponseMessage response);
    }
}