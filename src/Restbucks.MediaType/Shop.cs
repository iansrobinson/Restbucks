using System;
using System.Collections.Generic;
using System.Linq;

namespace Restbucks.MediaType
{
    public class Shop
    {
        private readonly Uri uri;
        private readonly IList<Item> items;
        private readonly IList<Link> links;
        private readonly IList<Form> forms;

        public Shop(Uri uri1, IEnumerable<Item> items, IEnumerable<Link> links, IEnumerable<Form> forms)
        {
            this.items = new List<Item>(items);
            this.links = new List<Link>(links);
            this.forms = new List<Form>(forms);
        }

        public Shop(Uri uri) : this(uri, new Item[] {}, new Link[] {}, new Form[] {})
        {
        }

        public Shop(Uri uri, IEnumerable<Item> items) : this(uri, items, new Link[] {}, new Form[] {})
        {
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
                throw new NamespacePrefixConflictException(string.Format("One or more prefixes are each associated with more than one namespace: '{0}'.", string.Join(", ", conflictingPrefixes.Distinct())));
            }
        }

        public Shop AddForm(Form form)
        {
            forms.Add(form);
            return this;
        }

        public Uri BaseUri
        {
            get { return uri; }
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