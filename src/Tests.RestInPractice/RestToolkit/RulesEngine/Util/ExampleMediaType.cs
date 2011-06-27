using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.ApplicationServer.Http;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    public class ExampleMediaType : MediaTypeFormatter
    {
        private static readonly MediaTypeHeaderValue contentType;
        private static readonly MediaTypeFormatter instance;

        static ExampleMediaType()
        {
            contentType = new MediaTypeHeaderValue("application/vnd.restinpractice+xml");
            instance = new ExampleMediaType();
        }

        public static MediaTypeHeaderValue ContentType
        {
            get { return contentType; }
        }

        public static MediaTypeFormatter Instance
        {
            get { return instance; }
        }

        private readonly DataContractSerializer serializer;

        public ExampleMediaType()
        {
            serializer = new DataContractSerializer(typeof (ExampleEntityBody));
            SupportedMediaTypes.Add(contentType);
        }

        public override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return serializer.ReadObject(stream);
        }

        public override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            serializer.WriteObject(stream, value);
        }
    }
}