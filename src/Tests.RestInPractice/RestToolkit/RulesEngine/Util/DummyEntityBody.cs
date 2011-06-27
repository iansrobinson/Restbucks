using System;
using System.Runtime.Serialization;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    [DataContract]
    public class DummyEntityBody
    {
        [DataMember] 
        public int Id { get; set; }
        [DataMember]
        public DummyLink Link { get; set; }
        [DataMember]
        public DummyForm Form { get; set; }
    }

    [DataContract]
    public class DummyLink
    {
        [DataMember]
        public string Rel { get; set; }
        [DataMember]
        public string Uri { get; set; }
        [DataMember]
        public string ContentType { get; set; }
    }

    [DataContract]
    public class DummyForm
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