using System;
using System.IO;
using System.Xml;

namespace Restbucks.Quoting.Service.Old.Processors
{
    public class XmlTransform : IDisposable
    {
        private static readonly XmlReaderSettings DefaultReaderSettings = new XmlReaderSettings {IgnoreWhitespace = true};
        private static readonly XmlWriterSettings DefaultWriterSettings = new XmlWriterSettings { Indent = true};

        private readonly Stream streamIn;
        private readonly XmlReaderSettings readerSettings;

        public XmlTransform(Stream streamIn) : this(streamIn, DefaultReaderSettings)
        {
        }

        public XmlTransform(Stream streamIn, XmlReaderSettings readerSettings)
        {
            this.streamIn = streamIn;
            this.readerSettings = readerSettings;
        }

        public void Transform(Stream streamOut, Func<XmlReader, XmlWriter, bool> transformFunction)
        {
            using (XmlReader reader = XmlReader.Create(new StreamReader(streamIn), readerSettings))
            {
                using (XmlWriter writer = XmlWriter.Create(new StreamWriter(streamOut), DefaultWriterSettings))
                {
                    while (reader.Read())
                    {
                        while (transformFunction(reader, writer))
                        {
                        }
                        WriteShallowNode(reader, writer);
                    }
                }
            }
        }

        private static void WriteShallowNode(XmlReader reader, XmlWriter writer)
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                    writer.WriteAttributes(reader, true);
                    if (reader.IsEmptyElement)
                    {
                        writer.WriteEndElement();
                    }
                    return;

                case XmlNodeType.Text:
                    writer.WriteString(reader.Value);
                    return;

                case XmlNodeType.CDATA:
                    writer.WriteCData(reader.Value);
                    return;

                case XmlNodeType.ProcessingInstruction:
                    writer.WriteProcessingInstruction(reader.Name, reader.Value);
                    return;

                case XmlNodeType.Comment:
                    writer.WriteComment(reader.Value);
                    return;

                case XmlNodeType.DocumentType:
                    writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                    return;

                case XmlNodeType.Whitespace:
                    writer.WriteWhitespace(reader.Value);
                    return;

                case XmlNodeType.SignificantWhitespace:
                    writer.WriteWhitespace(reader.Value);
                    return;

                case XmlNodeType.EndElement:
                    writer.WriteFullEndElement();
                    return;

                case XmlNodeType.XmlDeclaration:
                    writer.WriteProcessingInstruction(reader.Name, reader.Value);
                    return;
            }
            throw new InvalidOperationException("Invalid node");
        }

        public void Dispose()
        {
            if (streamIn != null)
            {
                streamIn.Dispose();
            }
        }
    }
}