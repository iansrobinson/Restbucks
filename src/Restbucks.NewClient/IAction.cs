using System.Net.Http;

namespace Restbucks.NewClient
{
    public interface IAction
    {
        HttpResponseMessage Execute();
    }
}