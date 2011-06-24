using System.Net.Http;

namespace Restbucks.RestToolkit.RulesEngine
{
    public delegate IState CreateNextStateDelegate(HttpResponseMessage currentResponse, ApplicationStateVariables stateVariables);

    public delegate IState CreateStateDelegate(HttpResponseMessage currentResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);

    public delegate HttpResponseMessage IssueRequestDelegate(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities);

    public delegate bool ResponseConditionDelegate(HttpResponseMessage response);

    public delegate bool StateConditionDelegate(HttpResponseMessage response, ApplicationStateVariables stateVariables);

    public delegate IGenerateNextRequest CreateActionDelegate(Actions actions);
}