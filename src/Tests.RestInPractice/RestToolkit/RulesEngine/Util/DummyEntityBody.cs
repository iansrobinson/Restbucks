using System;
using System.Runtime.Serialization;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    [DataContract]
    public class DummyEntityBody
    {
        public DummyEntityBody(Uri baseUri, string linkRel, string formId)
        {
            BaseUri = baseUri;
            LinkRel = linkRel;
            FormId = formId;
        }

        [DataMember] 
        public Uri BaseUri { get; set; }
        [DataMember]
        public string LinkRel { get; set; }
        [DataMember]
        public string FormId { get; set; }
    }
}