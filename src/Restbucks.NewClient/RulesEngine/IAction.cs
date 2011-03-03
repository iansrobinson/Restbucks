using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IAction
    {
        HttpResponseMessage Execute();
    }
}