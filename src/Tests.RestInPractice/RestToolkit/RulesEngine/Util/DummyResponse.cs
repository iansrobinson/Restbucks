using System.Net.Http;
using Microsoft.ApplicationServer.Http;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    public static class DummyResponse
    {
        private static readonly ExampleEntityBody DummyEntityBody;
        private static readonly ExampleLink DummyLink;
        private static readonly ExampleForm DummyForm;

        static DummyResponse()
        {
            DummyLink = new ExampleLink {ContentType = "application/xml", Rel = "re-value", Uri = "http://localhost/1"};
            DummyForm = new ExampleForm {Id = "form-id", ContentType = "application/xml", Method = "post", Uri = "http://localhost/form"};
            DummyEntityBody = new ExampleEntityBody {Id = 5, Form = DummyForm, Link = DummyLink};
        }

        public static ExampleEntityBody EntityBody
        {
            get { return DummyEntityBody; }
        }

        public static ExampleLink Link
        {
            get { return DummyLink; }
        }

        public static ExampleForm Form
        {
            get { return DummyForm; }
        }

        public static HttpResponseMessage CreateResponse()
        {
            var content = new ObjectContent<ExampleEntityBody>(EntityBody, new[] {ExampleMediaType.Instance});
            content.Headers.ContentType = ExampleMediaType.ContentType;

            return new HttpResponseMessage {Content = content};
        }
    }
}