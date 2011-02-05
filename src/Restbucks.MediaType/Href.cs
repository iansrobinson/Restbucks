using System;
using Restbucks.RestToolkit.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.MediaType
{
    public class Href
    {
        private readonly Uri uri;
        private readonly ResponseLifecycleController<Shop> responseAccessor;

        public Href(Uri uri) : this(uri, uri)
        {
        }

        public Href(Uri uri, Uri accessUri)
        {
            Check.IsNotNull(uri, "uri");
            Check.IsNotNull(accessUri, "accessUri");
            
            this.uri = uri;
            if (accessUri.IsAbsoluteUri)
            {
                responseAccessor = new ResponseLifecycleController<Shop>(accessUri);
            }
        }

        public Uri Uri
        {
            get { return uri; }
        }

        public ResponseLifecycleController<Shop> ResponseLifecycleController
        {
            get
            {
                if (!IsDereferenceable)
                {
                    throw new InvalidOperationException("Unable to determine full URI.");
                }

                return responseAccessor;
            }
        }

        public bool IsDereferenceable
        {
            get { return responseAccessor != null; }
        }

        public Href WithNewAccessUri(Uri accessUri)
        {
            if (IsDereferenceable)
            {
                throw new InvalidOperationException("Href is already backed by an absolute URI.");
            }
            
            return new Href(uri, accessUri);
        }
    }
}