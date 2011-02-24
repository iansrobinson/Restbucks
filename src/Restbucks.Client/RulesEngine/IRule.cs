using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public interface IRule
    {
        Result<HttpResponseMessage> Evaluate(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider);
        Result<IState> CreateNewState(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider);
    }
}