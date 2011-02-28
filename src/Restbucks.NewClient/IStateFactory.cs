using System.Net.Http;

namespace Restbucks.NewClient
{
    public interface IStateFactory
    {
        IState Create(HttpResponseMessage response);
    }
}