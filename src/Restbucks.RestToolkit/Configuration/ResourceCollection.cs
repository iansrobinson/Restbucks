using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Configuration
{
    public class ResourceCollection
    {
        private readonly Assembly assembly;

        public ResourceCollection(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public void ForEach(Action<ResourceInfo> handleResourceType)
        {
            var results = from t in assembly.GetTypes()
                        where t.GetCustomAttributes(typeof (UriTemplateAttribute), false).Length > 0
                              && t.GetCustomAttributes(typeof (ServiceContractAttribute), false).Length > 0
                        select new ResourceInfo(t, ((UriTemplateAttribute) t.GetCustomAttributes(typeof (ServiceContractAttribute), false).First()).UriFactoryWorker);

            results.ToList().ForEach(handleResourceType);
        }
    }
}