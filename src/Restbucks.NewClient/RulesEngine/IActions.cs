namespace Restbucks.NewClient.RulesEngine
{
    public interface IActions
    {
        IActionInvoker SubmitForm(IFormStrategy formStrategy);
        IActionInvoker ClickLink(ILinkStrategy linkStrategy);
    }
}