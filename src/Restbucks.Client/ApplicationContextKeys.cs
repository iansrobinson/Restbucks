using Restbucks.Client.Keys;

namespace Restbucks.Client
{
    public static class ApplicationContextKeys
    {
        public static readonly IKey ContextName = new StringKey("context-name");
        public static readonly IKey EntryPointUri = new StringKey("entry-point-uri");
        public static readonly IKey CurrentEntity = new StringKey("current-entity");
    }
}