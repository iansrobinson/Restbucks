using System.IO;

namespace Restbucks.Quoting.Service.Processors
{
    public interface ISignForms
    {
        void SignForms(Stream streamIn, Stream streamOut);
    }
}