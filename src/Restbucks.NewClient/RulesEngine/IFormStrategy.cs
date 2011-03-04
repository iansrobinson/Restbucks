namespace Restbucks.NewClient.RulesEngine
{
    public interface IFormStrategy
    {
        FormInfo GetFormInfo(object entityBody, object input);
    }
}