using System.IO;

namespace Restbucks.Quoting.Service.Old.Processors
{
    public interface ISignForms
    {
        void SignForms(Stream streamIn, Stream streamOut);
    }
}