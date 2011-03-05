using System;

namespace Restbucks.MediaType
{
    public abstract class LinkRelation
    {
        public static LinkRelation Parse(string value, Func<string, string> lookupNamespace)
        {
            if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
            {
                var parts = value.Split(new[] {':'}, 2);
                var prefix = parts[0];
                var reference = parts[1];

                if (IsCompactUri(prefix, lookupNamespace))
                {
                    var baseUri = new Uri(lookupNamespace(prefix));
                    return new CompactUriLinkRelation(prefix, baseUri, reference);
                }

                return new UriLinkRelation(new Uri(value));
            }

            return new StringLinkRelation(value);
        }

        private static bool IsCompactUri(string prefix, Func<string, string> lookupNamespace)
        {
            return !string.IsNullOrWhiteSpace(lookupNamespace(prefix));
        }

        public abstract string Value { get; }
        public abstract string DisplayValue { get; }
    }
}