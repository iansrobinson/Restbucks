using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Restbucks.MediaType.Formatters
{
    public class FormsFormatter
    {
        private readonly Shop shop;

        public FormsFormatter(Shop shop)
        {
            this.shop = shop;
        }

        public IEnumerable<XElement> CreateXml()
        {
            return shop.Forms.Select(form => new XElement(Namespaces.XForms + "model",
                                                          form.Id != null ? new XAttribute("id", form.Id) : null,
                                                          form.Schema != null ? new XAttribute("schema", form.Schema) : null,
                                                          new XElement(Namespaces.XForms + "instance",
                                                                       form.Instance != null ? new ShopFormatter(form.Instance).CreateXml() : null),
                                                          new XElement(Namespaces.XForms + "submission",
                                                                       new XAttribute("resource", form.Resource),
                                                                       new XAttribute("method", form.Method),
                                                                       new XAttribute("mediatype", form.MediaType))));
        }
    }
}