using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Microsoft.Net.Http;
using Restbucks.MediaType;
using Restbucks.MediaType.Assemblers;
using Restbucks.MediaType.Formatters;

namespace Restbucks.NewClient
{
    public class RestbucksFormatter : IContentFormatter
    {
        public static readonly IContentFormatter Instance = new RestbucksFormatter();

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings {Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates};

        private RestbucksFormatter()
        {
        }

        public void WriteToStream(object instance, Stream stream)
        {
            try
            {
                var root = new ShopFormatter((Shop) instance).CreateXml();

                using (var writer = XmlWriter.Create(stream, WriterSettings))
                {
                    root.WriteTo(writer);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Unexpected error writing application/restbucks+xml to response stream.", ex);
                throw;
            }
        }

        public object ReadFromStream(Stream stream)
        {
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
                throw new InvalidFormatException("Incorrectly formatted entity body. Request must be formatted according to application/restbucks+xml.", ex);
            }
            catch (Exception ex)
            {
                Log.Warn("Unexpected error reading application/restbucks+xml from request stream.", ex);
                throw;
            }
        }

        public IEnumerable<MediaTypeHeaderValue> SupportedMediaTypes
        {
            get { return new[] {new MediaTypeHeaderValue(RestbucksMediaType.Value),}; }
        }
    }
}