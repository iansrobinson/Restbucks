using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IActionInvoker
    {
        HttpResponseMessage Invoke(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables);
    }
}