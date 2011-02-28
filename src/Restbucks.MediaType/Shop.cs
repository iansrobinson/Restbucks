using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Restbucks.MediaType
{
    public class Shop
    {        
        private readonly Uri baseUri;
        private readonly ReadOnlyCollection<Item> items;
        private readonly ReadOnlyCollection<Link> links;
        private readonly ReadOnlyCollection<Form> forms;

        public Shop(Uri baseUri, IEnumerable<Item> items, IEnumerable<Link> links, IEnumerable<Form> forms)
        {
            this.baseUri = baseUri;
            this.items = new List<Item>(items).AsReadOnly();
            this.links = new List<Link>(links).AsReadOnly();
            this.forms = new List<Form>(forms).AsReadOnly();

            links.ToList().ForEach(ThrowIfNamespacePrefixesConflict);
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
            get { return items.Count() > 0; }
        }

        public bool HasLinks
        {
            get { return links.Count() > 0; }
        }

        public bool HasForms
        {
            get { return forms.Count() > 0; }
        }
    }
}