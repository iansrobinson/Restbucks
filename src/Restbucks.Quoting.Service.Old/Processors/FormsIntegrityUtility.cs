using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.Quoting.Service.Old.Processors
{
    public class FormsIntegrityUtility : ISignForms
    {
        private readonly IGenerateSignature signature;
        private readonly string signaturePlaceholder;

        public FormsIntegrityUtility(IGenerateSignature signature, string signaturePlaceholder)
        {
            this.signature = signature;
            this.signaturePlaceholder = signaturePlaceholder;
        }

        public void SignForms(Stream streamIn, Stream streamOut)
        {
            streamIn.Seek(0, SeekOrigin.Begin);
            using (var xmlTransform = new XmlTransform(streamIn))
            {
                xmlTransform.Transform(streamOut, SignForm);
            }
        }

        private bool SignForm(XmlReader reader, XmlWriter writer)
        {
            if (!reader.IsStartElement("model"))
            {
                return false;
            }

            var doc = XDocument.Load(reader.ReadSubtree());
            var formContents = GetFormContents(doc);
            var submission = GetSubmission(doc);

            if (submission != null)
            {
                submission.Element.SetAttributeValue("resource", submission.TargetUri.Replace(signaturePlaceholder, signature.GenerateSignature(formContents)));
            }

            reader.ReadEndElement();
            writer.WriteNode(doc.CreateReader(), false);
            writer.Flush();

            return true;
        }

        private static dynamic GetSubmission(XDocument doc)
        {
            return (from element in doc.Descendants(Namespaces.XForms + "submission")
                    let resource = element.Attribute("resource")
                    where resource != null
                    select new {Element = element, TargetUri = resource.Value}).FirstOrDefault();
        }

        private static string GetFormContents(XDocument doc)
        {
            var formContents = (from instance in doc.Descendants(Namespaces.XForms + "instance")
                                let contents = instance.Elements().FirstOrDefault()
                                select contents != null ? contents.ToString(SaveOptions.DisableFormatting) : string.Empty).FirstOrDefault();
            return formContents ?? string.Empty;
        }
    }
}