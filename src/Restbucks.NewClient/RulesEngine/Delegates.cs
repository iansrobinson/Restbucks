using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public delegate IState StateDelegate(HttpResponseMessage currentResponse, ApplicationContext context);

    public delegate HttpResponseMessage ActionDelegate(HttpResponseMessage previousResponse, ApplicationContext context, IClientCapabilities clientCapabilities);

    public delegate bool ResponseConditionDelegate(HttpResponseMessage response);

    public delegate bool StateConditionDelegate(HttpResponseMessage response, ApplicationContext context);
}