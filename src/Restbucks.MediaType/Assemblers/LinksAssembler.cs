using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Restbucks.MediaType.Assemblers
{
    public class LinksAssembler
    {
        public static Func<string, string> LookupNamespace(XElement element)
        {
            return prefix =>
                       {
                           var ns = element.GetNamespaceOfPrefix(prefix);
                           if (ns == null)
                           {
                               return null;
                           }
                           return ns.NamespaceName;
                       };
        }

        private static LinkRelation[] CreateLinkRelationsFromRelAttribute(string value, XElement link)
        {
            return value.Split(new[] { ' ' })
                .Select(rel => LinkRelation.Parse(rel, LookupNamespace(link))).ToArray();
        }

        private readonly XElement root;

        public LinksAssembler(XElement root)
        {
            this.root = root;
        }

        public IEnumerable<Link> AssembleLinks()
        {
            var links = root.Descendants(Namespaces.ShopSchema + "link");
            return links.Select(lnk => new Link(
                                           lnk.Attribute("type")
                                               .ApplyNonEmptyValueOrDefault(),
                                           lnk.Attribute("href")
                                               .ApplyNonEmptyValueOrDefault(value => new Uri(value, UriKind.RelativeOrAbsolute)),
                                           lnk.Attribute("rel")
                                               .ApplyNonEmptyValueOrDefault(value => CreateLinkRelationsFromRelAttribute(value, lnk))));
        }  
    }
}