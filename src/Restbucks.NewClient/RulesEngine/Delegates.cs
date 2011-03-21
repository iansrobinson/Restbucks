using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public delegate IState StateDelegate(HttpResponseMessage currentResponse, ApplicationStateVariables stateVariables);

    public delegate HttpResponseMessage ActionDelegate(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);

    public delegate bool ResponseConditionDelegate(HttpResponseMessage response);

    public delegate bool StateConditionDelegate(HttpResponseMessage response, ApplicationStateVariables stateVariables);
}