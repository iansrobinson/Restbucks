namespace Restbucks.Quoting.Service.MessageHandlers.FormsIntegrity
{
    public interface IGenerateSignature
    {
        string GenerateSignature(string value);
    }
}