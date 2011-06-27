using System.Runtime.Serialization;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    [DataContract]
    public class ExampleEntityBody
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public ExampleLink Link { get; set; }

        [DataMember]
        public ExampleForm Form { get; set; }

        public bool Equals(ExampleEntityBody other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id == Id && Equals(other.Link, Link) && Equals(other.Form, Form);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ExampleEntityBody)) return false;
            return Equals((ExampleEntityBody) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Id;
                result = (result*397) ^ (Link != null ? Link.GetHashCode() : 0);
                result = (result*397) ^ (Form != null ? Form.GetHashCode() : 0);
                return result;
            }
        }
    }

    [DataContract]
    public class ExampleLink
    {
        [DataMember]
        public string Rel { get; set; }

        [DataMember]
        public string Uri { get; set; }

        [DataMember]
        public string ContentType { get; set; }

        public bool Equals(ExampleLink other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Rel, Rel) && Equals(other.Uri, Uri) && Equals(other.ContentType, ContentType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ExampleLink)) return false;
            return Equals((ExampleLink) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Rel != null ? Rel.GetHashCode() : 0);
                result = (result*397) ^ (Uri != null ? Uri.GetHashCode() : 0);
                result = (result*397) ^ (ContentType != null ? ContentType.GetHashCode() : 0);
                return result;
            }
        }
    }

    [DataContract]
    public class ExampleForm
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Uri { get; set; }

        [DataMember]
        public string Method { get; set; }

        [DataMember]
        public string ContentType { get; set; }

        public bool Equals(ExampleForm other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Id, Id) && Equals(other.Uri, Uri) && Equals(other.Method, Method) && Equals(other.ContentType, ContentType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ExampleForm)) return false;
            return Equals((ExampleForm) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Id != null ? Id.GetHashCode() : 0);
                result = (result*397) ^ (Uri != null ? Uri.GetHashCode() : 0);
                result = (result*397) ^ (Method != null ? Method.GetHashCode() : 0);
                result = (result*397) ^ (ContentType != null ? ContentType.GetHashCode() : 0);
                return result;
            }
        }
    }
}