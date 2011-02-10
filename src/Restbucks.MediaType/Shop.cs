using System;
using System.Collections.Generic;
using System.Linq;

namespace Restbucks.MediaType
{
    public class Shop
    {
        private readonly Uri baseUri;
        private readonly IList<Item> items;
        private readonly IList<Link> links;
        private readonly IList<Form> forms;

        public Shop(Uri baseUri) : this(baseUri, new Item[] {})
        {
        }

        public Shop(Uri baseUri, IEnumerable<Item> items)
        {
            this.baseUri = baseUri;
            this.items = new List<Item>(items);

            links = new List<Link>();
            forms = new List<Form>();
        }

        public Shop AddItem(Item item)
        {
            items.Add(item);
            return this;
        }

        public Shop AddLink(Link link)
        {
            ThrowIfNamespacePrefixesConflict(link);
            
            links.Add(link);
            return this;
        }

        private void ThrowIfNamespacePrefixesConflict(Link link)
        {
            var rels = (from rel in
                            ((from Link l in links select l.Rels)
                            .SelectMany(rel => rel))
                            .Union(
                                from rel in link.Rels select rel)
                        where rel.GetType().Equals(typeof (CompactUriLinkRelation))
                        select (CompactUriLinkRelation) rel);

            var conflictingPrefixes = from rel1 in rels
                                      from rel2 in rels
                                      where rel1.Prefix.Equals(rel2.Prefix)
                                            && (!rel1.Uri.Equals(rel2.Uri))
                                      select rel1.Prefix;

            if (conflictingPrefixes.Count() > 0)
            {
                throw new NamespacePrefixConflictException(string.Format("One or more namespace prefixes are each associated with more than one namespace: '{0}'.", string.Join(", ", conflictingPrefixes.Distinct())));
            }
        }

        public Shop AddForm(Form form)
        {
            forms.Add(form);
            return this;
        }

        public Uri BaseUri
        {
            get { return baseUri; }
        }

        public IEnumerable<Link> Links
        {
            get { return links; }
        }

        public IEnumerable<Item> Items
        {
            get { return items; }
        }

        public IEnumerable<Form> Forms
        {
            get { return forms; }
        }

        public bool HasItems
        {
            get { return items.Count > 0; }
        }

        public bool HasLinks
        {
            get { return links.Count > 0; }
        }

        public bool HasForms
        {
            get { return forms.Count > 0; }
        }
    }
}