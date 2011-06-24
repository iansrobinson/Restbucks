using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public interface ICreateNextState
    {
        IState Execute(HttpResponseMessage currentResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);
    }
}