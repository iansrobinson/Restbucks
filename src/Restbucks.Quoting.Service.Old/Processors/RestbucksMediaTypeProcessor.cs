using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel.Description;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Microsoft.Http;
using Microsoft.ServiceModel.Http;
using Restbucks.MediaType;
using Restbucks.MediaType.Assemblers;
using Restbucks.MediaType.Formatters;

namespace Restbucks.Quoting.Service.Old.Processors
{
    public class RestbucksMediaTypeProcessor : MediaTypeProcessor
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings
                                                                       {
                                                                           Indent = true,
                                                                           NamespaceHandling =
                                                                               NamespaceHandling.OmitDuplicates
                                                                       };

        public RestbucksMediaTypeProcessor(HttpOperationDescription operation, MediaTypeProcessorMode mode)
            : base(operation, mode)
        {
        }

        public override void WriteToStream(object instance, Stream stream, HttpRequestMessage request)
        {
            try
            {
                var root = new ShopFormatter((Shop) instance).CreateXml();

                using (XmlWriter writer = XmlWriter.Create(stream, WriterSettings))
                {
                    root.WriteTo(writer);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                Log.Warn(string.Format("Unexpected error writing {0} to response stream.", RestbucksMediaType.Value), ex);
                throw;
            }
        }

        public override object ReadFromStream(Stream stream, HttpRequestMessage request)
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
                Log.Warn(string.Format("Unexpected error reading {0} from request stream.", RestbucksMediaType.Value), ex);
                throw;
            }
        }

        public override IEnumerable<string> SupportedMediaTypes
        {
            get { return new[] {RestbucksMediaType.Value, "application/xml", "text/xml"}; }
        }
    }
}