using Restbucks.Client.Keys;

namespace Restbucks.Client
{
    public static class ApplicationContextKeys
    {
        public static readonly IKey EntryPointUri = new StringKey("entry-point-uri");
    }
}