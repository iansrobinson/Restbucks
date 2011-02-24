using System.Net.Http;
using System.Reflection;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public interface IRule
    {
        HandlerResult Evaluate(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider);
        IState CreateNewState(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider);
    }
}