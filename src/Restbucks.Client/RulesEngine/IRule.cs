using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public interface IRule
    {
        Result<IState> Evaluate(HttpResponseMessage response, ApplicationContext context);
    }
}