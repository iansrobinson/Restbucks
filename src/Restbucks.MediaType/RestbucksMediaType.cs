using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Microsoft.ApplicationServer.Http;
using Restbucks.MediaType.Assemblers;
using Restbucks.MediaType.Formatters;

namespace Restbucks.MediaType
{
    public static class RestbucksMediaType
    {
        public const string Value = "application/vnd.restbucks+xml";
        public static MediaTypeFormatter Formatter = new RestbucksMediaTypeFormatter();

        private class RestbucksMediaTypeFormatter : MediaTypeFormatter
        {
            private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings {Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates};

            public RestbucksMediaTypeFormatter()
            {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(Value));
                SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
                SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
            }

            public override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
            {
                Log.Debug("Reading from stream...");

                if (stream == null)
                {
                    return null;
                }

                if (stream.Length.Equals(0))
                {
                    return null;
                }

                try
                {
                    if (stream.Position != 0)
                    {
                        if (!stream.CanSeek)
                        {
                            throw new InvalidOperationException("The stream was already consumed. It cannot be read again.");
                        }
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                    return new ShopAssembler(XElement.Load(stream)).AssembleShop();
                }
                catch (XmlException ex)
                {
                    throw new InvalidFormatException(string.Format("Incorrectly formatted entity body. Request must be formatted according to {0}.", Value), ex);
                }
                catch (Exception ex)
                {
                    Log.Warn(string.Format("Unexpected error reading {0} from request stream.", Value), ex);
                    throw;
                }
            }

            public override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
            {
                try
                {
                    var root = new ShopFormatter((Shop) value).CreateXml();

                    using (var writer = XmlWriter.Create(stream, WriterSettings))
                    {
                        root.WriteTo(writer);
                        writer.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn(string.Format("Unexpected error writing {0} to response stream.", Value), ex);
                    throw;
                }
            }
        }
    }
}