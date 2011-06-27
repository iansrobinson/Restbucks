using System.Net.Http;
using Microsoft.ApplicationServer.Http;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    public static class DummyResponse
    {
        public const int EntityId = 5;
        public static string LinkRel = "rel-value";
        public static string FormId = "form-id";

        public static HttpResponseMessage CreateResponse()
        {
            var entityBody = new DummyEntityBody {Id = EntityId, Form = new DummyForm {Id = FormId, ContentType = "application/xml", Method = "post", Uri = "http://localhost/form"}, Link = new DummyLink {ContentType = "application/xml", Rel = LinkRel, Uri = "http://localhost/1"}};

            var content = new ObjectContent<DummyEntityBody>(entityBody, new[] {DummyMediaType.Instance});
            content.Headers.ContentType = DummyMediaType.ContentType;

            return new HttpResponseMessage {Content = content};
        }
    }
}