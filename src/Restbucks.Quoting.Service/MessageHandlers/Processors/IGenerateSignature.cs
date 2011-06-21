namespace Restbucks.Quoting.Service.MessageHandlers.Processors
{
    public interface IGenerateSignature
    {
        string GenerateSignature(string value);
    }
}