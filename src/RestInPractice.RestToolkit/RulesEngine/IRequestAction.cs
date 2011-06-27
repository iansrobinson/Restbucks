using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public interface IRequestAction
    {
        HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}