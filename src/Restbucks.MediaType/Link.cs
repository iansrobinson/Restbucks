using System;
using System.Collections.Generic;
using System.Linq;

namespace Restbucks.MediaType
{
    public class Link
    {
        private readonly IEnumerable<LinkRelation> rels;
        private readonly string mediaType;
        private readonly Uri href;
        private readonly Uri clickUri;
        private Shop instance;

        public Link(Uri href, Uri clickUri, string mediaType, params LinkRelation[] rels)
        {
            this.href = href;
            this.clickUri = clickUri;
            this.mediaType = mediaType;
            this.rels = rels;

            if (clickUri == null && href.IsAbsoluteUri)
            {
                this.clickUri = href;
            }
        }

        public Link(Uri href, string mediaType, params LinkRelation[] rels) : this(href, null, mediaType, rels)
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
            get { return href; }
        }

        public bool IsClickable
        {
            get { return clickUri != null; }
        }

        public void Prefetch(Func<Uri, Shop> client)
        {
            instance = client(clickUri);
        }

        public Shop Click(Func<Uri, Shop> client)
        {
            if (clickUri == null)
            {
                throw new InvalidOperationException("Unable to determine absolute URI.");
            }

            return instance ?? client(clickUri);
        }

        public Link NewLinkWithBaseUri(Uri baseUri)
        {
            if (clickUri != null)
            {
                throw new InvalidOperationException("Link is already backed by an absolute URI.");
            }
            var newClickUri = new UriTemplate(href.ToString()).BindByPosition(baseUri);

            return new Link(href, newClickUri, mediaType, rels.ToArray());
        }
    }
}