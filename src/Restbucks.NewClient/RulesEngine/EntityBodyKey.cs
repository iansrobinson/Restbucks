using System;
using System.Net.Http.Headers;

namespace Restbucks.NewClient.RulesEngine
{
    public class EntityBodyKey : IKey
    {
        private readonly MediaTypeHeaderValue contentType;
        private readonly Uri schemaUri;

        public EntityBodyKey(MediaTypeHeaderValue contentType, Uri schemaUri)
        {
            this.contentType = contentType;
            this.schemaUri = schemaUri;
        }

        public override string ToString()
        {
            return string.Format("Content-Type: {0}, Schema-Uri: '{1}'", contentType.MediaType, schemaUri);
        }

        public bool Equals(EntityBodyKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.contentType, contentType) && Equals(other.schemaUri, schemaUri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (EntityBodyKey)) return false;
            return Equals((EntityBodyKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (contentType.GetHashCode()*397) ^ schemaUri.GetHashCode();
            }
        }
    }
}