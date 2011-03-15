namespace Restbucks.NewClient.RulesEngine
{
    public interface IApplicationContextBuilder
    {
        IApplicationContextBuilder Add(IKey key, object value);
        IApplicationContextBuilder Remove(IKey key);
        IApplicationContextBuilder Update(IKey key, object value);
        ApplicationContext Build();
    }
}