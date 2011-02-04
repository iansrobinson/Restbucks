using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Castle.Windsor;
using log4net;
using Microsoft.ServiceModel.Description;
using Microsoft.ServiceModel.Http;
using Restbucks.Quoting.Service.Old.Processors;
using Restbucks.Quoting.Service.Old.Resources;

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
            if (operation.DeclaringContract.ContractType.Equals(typeof(OrderForm)))
            {
                processors.Add(container.Resolve<FormsIntegrityResponseProcessor>());
            }
        }

//        protected override ContractDescription GetContract(ServiceHost host, ServiceDescription description, Type serviceType, IDictionary<string, ContractDescription> implementedContracts)
//        {
//            var contract = base.GetContract(host, description, serviceType, implementedContracts);
//            
//            foreach (var method in serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
//            {
//                var templates = method.GetCustomAttributes(typeof (UriTemplateAttribute), true);
//                var uriTemplate = templates.Length.Equals(0) ? string.Empty : ((UriTemplateAttribute) templates.First()).Value;
//
//                var behavior = GetWebAttribute(contract.Operations.Find(method.Name));
//                if (behavior != null)
//                {
//                    behavior.UriTemplate = uriTemplate;
//                }
//            }
//
//            return contract;
//        }

        private static dynamic GetWebAttribute(OperationDescription operation)
        {
            return (from b in operation.Behaviors
                    where b.GetType().Equals(typeof (WebGetAttribute)) || b.GetType().Equals(typeof (WebInvokeAttribute))
                    select b).FirstOrDefault();
        }

        protected override string GetUriTemplate(Type resource, string resourceName, string generatedUriTemplate)
        {
            return string.Empty;
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
            ContractDescription contract = this.GetContract(host, description, serviceType, implementedContracts);
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
            this.CreateEndpoint(host, description, contract);
        }


    }
}