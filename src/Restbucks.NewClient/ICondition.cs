using System.Net.Http;

namespace Restbucks.NewClient
{
    public interface ICondition
    {
        bool IsApplicable(HttpResponseMessage response);
    }
}