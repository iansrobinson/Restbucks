using System;
using System.Net.Http.Headers;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class EntityBodyKey : IKey
    {
        private readonly string id;
        private readonly MediaTypeHeaderValue contentType;
        private readonly Uri schemaUri;

        public EntityBodyKey(string id, MediaTypeHeaderValue contentType, Uri schemaUri)
        {
            CheckString.Is(Not.NullOrEmptyOrWhitespace, id, "id");
            Check.IsNotNull(contentType, "contentType");
            Check.IsNotNull(schemaUri, "schemaUri");
            
            this.id = id;
            this.contentType = contentType;
            this.schemaUri = schemaUri;
        }

        public override string ToString()
        {
            return string.Format("Id: '{0}', Content-Type: '{1}', Schema-Uri: '{2}'", id, contentType.MediaType, schemaUri);
        }

        public bool Equals(EntityBodyKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.id, id) && Equals(other.contentType, contentType) && Equals(other.schemaUri, schemaUri);
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
                int result = id.GetHashCode();
                result = (result*397) ^ contentType.GetHashCode();
                result = (result*397) ^ schemaUri.GetHashCode();
                return result;
            }
        }
    }
}