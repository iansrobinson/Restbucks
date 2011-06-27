using System;
using RestInPractice.RestToolkit.Hypermedia;

namespace RestInPractice.RestToolkit.Configuration
{
    public class ResourceInfo
    {
        private readonly Type type;
        private readonly IUriTemplate uriTemplate;

        public ResourceInfo(Type type, IUriTemplate uriTemplate)
        {
            this.type = type;
            this.uriTemplate = uriTemplate;
        }

        public Type Type
        {
            get { return type; }
        }

        public IUriTemplate UriTemplate
        {
            get { return uriTemplate; }
        }
    }
}