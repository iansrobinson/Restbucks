using System;
using System.Collections.Generic;
using System.Linq;

namespace Restbucks.MediaType
{
    public class ShopBuilder
    {
        private readonly Uri baseUri;
        private readonly IList<Item> items;
        private readonly IList<Link> links;
        private readonly IList<Form> forms;

        public ShopBuilder(Uri baseUri)
        {
            this.baseUri = baseUri;

            items = new List<Item>();
            links = new List<Link>();
            forms = new List<Form>();
        }

        public ShopBuilder AddItem(Item item)
        {
            items.Add(item);
            return this;
        }

        public ShopBuilder AddItems(IEnumerable<Item> items)
        {
            items.ToList().ForEach(i => AddItem(i));
            return this;
        }

        public ShopBuilder AddLink(Link link)
        {
            links.Add(link);
            return this;
        }

        public ShopBuilder AddLinks(IEnumerable<Link> links)
        {
            links.ToList().ForEach(l => AddLink(l));
            return this;
        }

        public ShopBuilder AddForm(Form form)
        {
            forms.Add(form);
            return this;
        }

        public ShopBuilder AddForms(IEnumerable<Form> forms)
        {
            forms.ToList().ForEach(f => AddForm(f));
            return this;
        }

        public Shop Build()
        {
            return new Shop(baseUri, items, links, forms);
        }
    }
}