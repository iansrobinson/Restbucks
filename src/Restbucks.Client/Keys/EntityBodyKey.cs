using System.Text;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.Client.Keys
{
    public class EntityBodyKey : IKey
    {
        private readonly string value;

        private const string KeyValueTemplate = "http://context-keys.restbucks.com/{0}:{1}";

        public EntityBodyKey(string mediaType, string schema, string contextName)
        {
            CheckString.Is(Not.NullOrEmptyOrWhitespace, mediaType, "mediaType");
            CheckString.Is(Not.Whitespace, schema, "schema");
            CheckString.Is(Not.Whitespace, contextName, "contextName");
            
            var sb = new StringBuilder();
            sb.Append(string.Format(KeyValueTemplate, "media-type", mediaType));
            sb.Append("&");
            sb.Append(string.Format(KeyValueTemplate, "schema", schema));
            sb.Append("&");
            sb.Append(string.Format(KeyValueTemplate, "context-name", contextName));
            value = sb.ToString();
        }

        public string Value
        {
            get { return value; }
        }
    }
}