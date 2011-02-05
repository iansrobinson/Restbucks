using System;
using System.Collections.Generic;
using System.Linq;

namespace Restbucks.MediaType
{
    public class Link
    {
        private readonly IEnumerable<LinkRelation> rels;
        private readonly string mediaType;
        private readonly Href href;
        private Shop instance;

        private Link(Href href, string mediaType, params LinkRelation[] rels)
        {
            this.href = href;
            this.mediaType = mediaType;
            this.rels = rels;  
        }

        public Link(Uri href, string mediaType, params LinkRelation[] rels) : this(new Href(href), mediaType, rels)
        {
        }

        public IEnumerable<LinkRelation> Rels
        {
            get { return rels; }
        }

        public string MediaType
        {
            get { return mediaType; }
        }

        public Uri Href
        {
            get { return href.DisplayUri; }
        }

        public bool IsClickable
        {
            get { return href.IsDereferenceable; }
        }

        public void Prefetch(Func<Uri, Shop> client)
        {
            instance = client(href.FullUri);
        }

        public Shop Click(Func<Uri, Shop> client)
        {
            if (!href.IsDereferenceable)
            {
                throw new InvalidOperationException("Unable to determine absolute URI.");
            }

            return instance ?? client(href.FullUri);
        }

        public Link NewLinkWithBaseUri(Uri baseUri)
        {
            var fullUri = new UriTemplate(href.DisplayUri.ToString()).BindByPosition(baseUri);

            return new Link(href.WithFullUri(fullUri), mediaType, rels.ToArray());
        }
    }
}