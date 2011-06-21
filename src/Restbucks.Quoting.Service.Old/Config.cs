using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Castle.Windsor;
using log4net;
using Microsoft.ServiceModel.Description;
using Microsoft.ServiceModel.Http;
using Restbucks.Quoting.Service.Old.Processors;
using Restbucks.Quoting.Service.Old.Resources;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Old
{
    public class Config : HttpConfigurationWithConventions
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IWindsorContainer container;

        public Config(IWindsorContainer container)
        {
            this.container = container;
        }

        public override void RegisterRequestProcessorsForOperation(HttpOperationDescription operation, IList<Processor> processors, MediaTypeProcessorMode mode)
        {
            processors.Add(new RestbucksMediaTypeProcessor(operation, mode));
        }

        public override void RegisterResponseProcessorsForOperation(HttpOperationDescription operation, IList<Processor> processors, MediaTypeProcessorMode mode)
        {
            processors.Add(new RestbucksMediaTypeProcessor(operation, mode));
//            if (operation.DeclaringContract.ContractType.Equals(typeof (OrderForm)))
//            {
//                processors.Add(container.Resolve<FormsIntegrityResponseProcessor>());
//            }
        }

        protected override string GetUriTemplate(Type resource, string resourceName, string generatedUriTemplate)
        {
            return container.Resolve<UriFactory>().GetUriTemplateValueFor(resource);
        }

        public override object GetInstance(Type serviceType, InstanceContext instanceContext, Message message)
        {
            Log.DebugFormat("Getting instance of type [{0}].", serviceType.FullName);
            return container.GetService(serviceType);
        }

        public override void ReleaseInstance(InstanceContext instanceContext, object service)
        {
            if (service is IDisposable)
            {
                Log.DebugFormat("Calling Dispose() on instance of type [{0}].", service.GetType().FullName);
                ((IDisposable) service).Dispose();
            }
        }

        public override void CreateDescription(Type serviceType, IDictionary<string, ContractDescription> implementedContracts, ServiceDescription description, ServiceHost host)
        {
            ContractDescription contract = GetContract(host, description, serviceType, implementedContracts);
            description.Behaviors.Remove<AspNetCompatibilityRequirementsAttribute>();
            AspNetCompatibilityRequirementsAttribute item = new AspNetCompatibilityRequirementsAttribute();
            item.RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed;
            description.Behaviors.Add(item);
            foreach (Uri uri in host.BaseAddresses)
            {
                if (!uri.Scheme.StartsWith("https"))
                {
                    ServiceEndpoint endpoint = new ServiceEndpoint(contract, new HttpMessageBinding(HttpMessageBindingSecurityMode.None), new EndpointAddress(uri, new AddressHeader[0]));
                    endpoint.Behaviors.Add(new HttpEndpointBehavior(this));
                    description.Endpoints.Add(endpoint);
                }
            }
            CreateEndpoint(host, description, contract);
        }
    }
}