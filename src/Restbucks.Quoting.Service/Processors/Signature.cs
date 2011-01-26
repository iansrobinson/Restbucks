namespace Restbucks.Quoting.Service.Processors
{
    public class Signature : IGenerateSignature
    {
        public static readonly IGenerateSignature Instance = new Signature();

        private Signature()
        {
        }

        public string GenerateSignature(string value)
        {
            return value.Length.ToString();
        }
    }
}