using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public delegate IState CreateState(HttpResponseMessage response, ApplicationContext context, Actions actions);

    public delegate IActionInvoker CreateActionInvoker(Actions actions);
}