﻿using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.ApplicationServer.Http;

namespace Tests.RestInPractice.RestToolkit.Utils
{
    public class DummyMediaType : MediaTypeFormatter
    {
        private static readonly MediaTypeHeaderValue contentType;
        private static readonly MediaTypeFormatter instance;

        static DummyMediaType()
        {
            contentType = new MediaTypeHeaderValue("application/vnd.restinpractice+xml");
            instance = new DummyMediaType();
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

        public DummyMediaType()
        {
            serializer = new DataContractSerializer(typeof (DummyEntityBody));
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