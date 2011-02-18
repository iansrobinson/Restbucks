using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Castle.Windsor;
using log4net;
using Microsoft.ServiceModel.Description;
using Microsoft.ServiceModel.Http;
using Restbucks.Quoting.Service.Processors;
using Restbucks.Quoting.Service.Resources;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service
{
    public class Config : HttpHostConfiguration, IProcessorProvider, IInstanceFactory, IServiceConfiguration
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IWindsorContainer container;

        public Config(IWindsorContainer container)
        {
            this.container = container;
        }

        public void RegisterRequestProcessorsForOperation(HttpOperationDescription operation, IList<Processor> processors, MediaTypeProcessorMode mode)
        {
            processors.Add(new RestbucksMediaTypeProcessor(operation, mode));
        }

        public void RegisterResponseProcessorsForOperation(HttpOperationDescription operation, IList<Processor> processors, MediaTypeProcessorMode mode)
        {
            processors.Add(new RestbucksMediaTypeProcessor(operation, mode));
            if (operation.DeclaringContract.ContractType.Equals(typeof (OrderForm)))
            {
                processors.Add(container.Resolve<FormsIntegrityResponseProcessor>());
            }
        }

        public object GetInstance(Type serviceType, InstanceContext instanceContext, Message message)
        {
            Log.DebugFormat("Getting instance of type [{0}].", serviceType.FullName);
            return container.GetService(serviceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object service)
        {
            if (service is IDisposable)
            {
                Log.DebugFormat("Calling Dispose() on instance of type [{0}].", service.GetType().FullName);
                ((IDisposable) service).Dispose();
            }
        }

        public void Configure(ServiceDescription service)
        {
            Log.DebugFormat("Configuring service description for [{0}].", service.ServiceType.FullName);
            
            var serviceType = service.ServiceType;
            
            foreach (var endpoint in service.Endpoints)
            {
                var contract = endpoint.Contract;
                
                foreach (var method in serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    var behavior = GetWebAttribute(contract.Operations.Find(method.Name));
                    if (behavior != null)
                    {
                        behavior.UriTemplate = container.Resolve<UriFactory>().GetUriTemplateValueFor(serviceType);

                        Log.DebugFormat("Setting URI template for method. Method: [{0}], URI template: [{1}].", method.Name, behavior.UriTemplate);
                    }
                }
            }
        }

        private static dynamic GetWebAttribute(OperationDescription operation)
        {
            return (from b in operation.Behaviors
                    where b.GetType().Equals(typeof(WebGetAttribute)) || b.GetType().Equals(typeof(WebInvokeAttribute))
                    select b).FirstOrDefault();
        }
    }
}