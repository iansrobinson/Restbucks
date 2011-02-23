using System.Net.Http;
using System.Reflection;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public interface IRule
    {
        HandlerResult Evaluate(MethodInfo getResponseHandler, IResponseHandlerProvider responseHandlers, HttpResponseMessage response, ApplicationContext context);
        IState CreateNewState(IResponseHandlerProvider responseHandlers, ApplicationContext context, HttpResponseMessage response);
    }
}