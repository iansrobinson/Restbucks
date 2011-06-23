using System;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using Castle.Windsor;
using log4net;
using Microsoft.ApplicationServer.Http.Description;

namespace Restbucks.Quoting.Service.Configuration
{
    public class ResourceFactory : IResourceFactory
    {
        private readonly IWindsorContainer container;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ResourceFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        public object GetInstance(Type serviceType, InstanceContext instanceContext, HttpRequestMessage request)
        {
            Log.DebugFormat("Getting instance of type [{0}].", serviceType.FullName);
            return container.GetService(serviceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object service)
        {
            if (service is IDisposable)
            {
                Log.DebugFormat("Calling Dispose() on instance of type [{0}].", service.GetType().FullName);
                ((IDisposable)service).Dispose();
            }
        }
    }
}