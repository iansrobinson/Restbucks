namespace Restbucks.NewClient.RulesEngine
{
    public interface IActions
    {
        IAction SubmitForm(IFormStrategy formStrategy);
        IAction ClickLink(ILinkStrategy linkStrategy);
    }
}