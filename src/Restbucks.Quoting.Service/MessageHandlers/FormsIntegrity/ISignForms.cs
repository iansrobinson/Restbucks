using System.IO;

namespace Restbucks.Quoting.Service.MessageHandlers.FormsIntegrity
{
    public interface ISignForms
    {
        void SignForms(Stream streamIn, Stream streamOut);
    }
}