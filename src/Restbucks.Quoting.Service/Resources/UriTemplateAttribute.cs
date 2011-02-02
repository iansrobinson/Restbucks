using System;

namespace Restbucks.Quoting.Service.Resources
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UriTemplateAttribute : Attribute
    {
        private readonly UriFactoryWorker uriFactoryWorker;

        public UriTemplateAttribute(string routePrefix) : this(routePrefix, string.Empty)
        {
        }

        public UriTemplateAttribute(string routePrefix, string uriTemplate)
        {
            uriFactoryWorker = new UriFactoryWorker(routePrefix, uriTemplate);
        }

        public UriFactoryWorker UriFactoryWorker
        {
            get { return uriFactoryWorker; }
        }
    }
}