namespace Restbucks.NewClient.RulesEngine
{
    public interface IApplicationStateVariablesBuilder
    {
        IApplicationStateVariablesBuilder Add(IKey key, object value);
        IApplicationStateVariablesBuilder Remove(IKey key);
        IApplicationStateVariablesBuilder Update(IKey key, object value);
        ApplicationStateVariables Build();
    }
}