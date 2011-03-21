using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public delegate IState CreateState(HttpResponseMessage currentResponse, ApplicationContext context);

    public delegate HttpResponseMessage ExecuteAction(HttpResponseMessage previousResponse, ApplicationContext context, IClientCapabilities clientCapabilities);

    public delegate bool Condition(HttpResponseMessage response);

    public delegate bool ExtendedCondition(HttpResponseMessage response, ApplicationContext context);
}