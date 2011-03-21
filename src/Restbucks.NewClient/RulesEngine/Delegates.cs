using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public delegate IState CreateStateDelegate(HttpResponseMessage currentResponse, ApplicationStateVariables stateVariables);

    public delegate HttpResponseMessage ExecuteActionDelegate(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);

    public delegate bool IsApplicableToResponseDelegate(HttpResponseMessage response);

    public delegate bool IsApplicableToStateInfoDelegate(HttpResponseMessage response, ApplicationStateVariables stateVariables);
}