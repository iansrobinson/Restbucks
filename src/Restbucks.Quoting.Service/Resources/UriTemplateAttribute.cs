using System;

namespace Restbucks.Quoting.Service.Resources
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UriTemplateAttribute : Attribute
    {
        private readonly UriFactory uriFactory;

        public UriTemplateAttribute(string routePrefix) : this(routePrefix, string.Empty)
        {
        }

        public UriTemplateAttribute(string routePrefix, string uriTemplate)
        {
            uriFactory = new UriFactory(routePrefix, uriTemplate);
        }

        public UriFactory UriFactory
        {
            get { return uriFactory; }
        }
    }
}