using System;

namespace Restbucks.Quoting.Service.Resources
{
    public class UriFactory
    {
        private readonly string routePrefix;
        private readonly UriTemplate uriTemplate;
        private readonly Uri dummyBaseAddress;

        private static readonly Uri Localhost = new Uri("http://localhost");

        public UriFactory(string routePrefix) : this(routePrefix, string.Empty)
        {
        }

        public UriFactory(string routePrefix, string uriTemplateValue)
        {
            this.routePrefix = routePrefix;
            uriTemplate = new UriTemplate(uriTemplateValue, true);

            dummyBaseAddress = new Uri(Localhost, routePrefix);
        }

        public string RoutePrefix
        {
            get { return routePrefix; }
        }

        public string UriTemplateValue
        {
            get { return uriTemplate.ToString(); }
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