using System;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;
using Tests.RestInPractice.RestToolkit.Utils;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    public static class DummyResponse
    {
        public static Uri BaseUri = new Uri("http://localhost/base-uri");
        public static string LinkRel = "rel-value";
        public static string FormId = "form-id";
        
        public static HttpResponseMessage CreateResponse()
        {
            var entityBody = new DummyEntityBody(BaseUri, LinkRel, FormId);

            var content = new ObjectContent<DummyEntityBody>(entityBody, new[] {DummyMediaType.Instance});
            content.Headers.ContentType = DummyMediaType.ContentType;

            return new HttpResponseMessage {Content = content};
        }
    }
}