using System.Net.Http;

namespace Restbucks.NewClient
{
    public interface IRule
    {
        Result Evaluate(HttpResponseMessage previousResponse);
    }
}