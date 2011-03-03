using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IStateFactory
    {
        IState Create(HttpResponseMessage response);
    }
}