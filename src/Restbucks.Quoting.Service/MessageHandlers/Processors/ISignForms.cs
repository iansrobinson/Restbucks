using System.IO;

namespace Restbucks.Quoting.Service.MessageHandlers.Processors
{
    public interface ISignForms
    {
        void SignForms(Stream streamIn, Stream streamOut);
    }
}