using System;

namespace Restbucks.Quoting.Service.Old.Resources
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NewUriTemplateAttribute : Attribute
    {
        private readonly UriFactoryWorker uriFactoryWorker;

        public NewUriTemplateAttribute(string routePrefix)
            : this(routePrefix, string.Empty)
        {
        }

        public NewUriTemplateAttribute(string routePrefix, string uriTemplate)
        {
            uriFactoryWorker = new UriFactoryWorker(routePrefix, uriTemplate);
        }

        public UriFactoryWorker UriFactoryWorker
        {
            get { return uriFactoryWorker; }
        }
    }
}