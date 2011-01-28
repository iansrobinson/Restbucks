using System;

namespace Restbucks.Quoting.Service.Old.Resources
{
    public class UriFactory
    {
        private readonly string routePrefix;
        private readonly UriTemplate uriTemplate;
        private readonly Uri dummyBaseAddress;

        public UriFactory(string routePrefix) : this(routePrefix, "/")
        {
        }

        public UriFactory(string routePrefix, string uriTemplate)
        {
            this.routePrefix = routePrefix;
            this.uriTemplate = new UriTemplate(uriTemplate);

            dummyBaseAddress = new Uri(new Uri("http://localhost"), routePrefix);
        }

        public string RoutePrefix
        {
            get { return routePrefix; }
        }

        public Uri CreateRelativeUri(params string[] values)
        {
            return new Uri(uriTemplate.BindByPosition(dummyBaseAddress, values).PathAndQuery, UriKind.RelativeOrAbsolute);
        }

        public Uri CreateAbsoluteUri(Uri baseAddress, params string[] values)
        {
            var baseUri = new UriBuilder(baseAddress.Scheme, baseAddress.Host, baseAddress.Port).Uri;
            return uriTemplate.BindByPosition(new Uri(baseUri, routePrefix), values);
        }
    }
}