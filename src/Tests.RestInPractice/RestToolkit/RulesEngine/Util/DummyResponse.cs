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
            var entityBody = new ExampleEntityBody {Id = EntityId, Form = new ExampleForm {Id = FormId, ContentType = "application/xml", Method = "post", Uri = "http://localhost/form"}, Link = new ExampleLink {ContentType = "application/xml", Rel = LinkRel, Uri = "http://localhost/1"}};

            var content = new ObjectContent<ExampleEntityBody>(entityBody, new[] {ExampleMediaType.Instance});
            content.Headers.ContentType = ExampleMediaType.ContentType;

            return new HttpResponseMessage {Content = content};
        }
    }
}