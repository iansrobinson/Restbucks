using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient.RulesEngine
{
    public interface IFormStrategy
    {
        FormInfo GetFormInfo(object entityBody);
        object GetFormData(object entityBody, object input);
    }
}