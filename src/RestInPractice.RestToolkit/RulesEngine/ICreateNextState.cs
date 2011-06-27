using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public interface ICreateNextState
    {
        IState Execute(HttpResponseMessage currentResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}