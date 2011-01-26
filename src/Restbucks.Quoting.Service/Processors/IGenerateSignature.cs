namespace Restbucks.Quoting.Service.Processors
{
    public interface IGenerateSignature
    {
        string GenerateSignature(string value);
    }
}