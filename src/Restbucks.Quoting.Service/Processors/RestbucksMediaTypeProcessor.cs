using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel.Description;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Microsoft.ServiceModel.Http;
using Restbucks.MediaType;
using Restbucks.MediaType.Assemblers;
using Restbucks.MediaType.Formatters;

namespace Restbucks.Quoting.Service.Processors
{
    public class RestbucksMediaTypeProcessor : MediaTypeProcessor
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings {Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates};

        public RestbucksMediaTypeProcessor(HttpOperationDescription operation, MediaTypeProcessorMode mode) : base(operation, mode)
        {
        }

        public override void WriteToStream(object instance, Stream stream, HttpRequestMessage request)
        {
            try
            {
                var root = new ShopFormatter((Shop)instance).CreateXml();

                using (XmlWriter writer = XmlWriter.Create(stream, WriterSettings))
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

        public override object ReadFromStream(Stream stream, HttpRequestMessage request)
        {
            try
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
                    return new ShopAssembler(XElement.Load(stream)).AssembleShop();
                }
                catch (XmlException)
                {
                    throw new InvalidFormatException("Incorrectly formatted entity body. Request must be formatted according to application/restbucks+xml.");
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Unexpected error reading application/restbucks+xml from request stream.", ex);
                throw;
            } 
        }

        public override IEnumerable<string> SupportedMediaTypes
        {
            get { return new[] {"application/restbucks+xml", "application/xml", "text/xml"}; }
        }
    }
}