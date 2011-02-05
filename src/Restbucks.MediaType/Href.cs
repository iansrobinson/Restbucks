using System;

namespace Restbucks.MediaType
{
    internal class Href
    {
        private readonly Uri displayUri;
        private readonly Uri fullUri;

        public Href(Uri displayUri)
        {
            this.displayUri = displayUri;

            if (displayUri.IsAbsoluteUri)
            {
                fullUri = displayUri;
            }
        }

        public Href(Uri displayUri, Uri fullUri)
        {
            this.displayUri = displayUri;
            this.fullUri = fullUri;
        }

        public Uri DisplayUri
        {
            get { return displayUri; }
        }

        public Uri FullUri
        {
            get { return fullUri; }
        }

        public bool IsDereferenceable
        {
            get { return fullUri != null; }
        }

        public Href WithFullUri(Uri uri)
        {
            if (IsDereferenceable)
            {
                throw new InvalidOperationException("Href is already backed by an absolute URI.");
            }
            
            return new Href(displayUri, uri);
        }
    }
}