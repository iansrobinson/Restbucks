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
    }
}