using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using Restbucks.RestToolkit.Hypermedia;

namespace Restbucks.Quoting.Service.Configuration
{
    public class ResourceManager
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IWindsorContainer container;
        private readonly RouteCollection routes;
        private readonly IHttpHostConfigurationBuilder configuration;
        private readonly MethodInfo register;

        public ResourceManager(IHttpHostConfigurationBuilder configuration, IWindsorContainer container, RouteCollection routes)
        {
            this.configuration = configuration;
            this.container = container;
            this.routes = routes;

            register = GetType().GetMethod("Register", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void RegisterResourcesFor(Assembly assembly)
        {
            var types = from t in assembly.GetTypes()
                        where t.GetCustomAttributes(typeof (UriTemplateAttribute), false).Length > 0
                              && t.GetCustomAttributes(typeof (ServiceContractAttribute), false).Length > 0
                        select t;

            types.ToList().ForEach(t =>
                                       {
                                           var genericMethod = register.MakeGenericMethod(new[] {t});
                                           genericMethod.Invoke(this, null);
                                       }
                );
        }

        private void Register<T>() where T : class
        {
            var uriFactory = container.Resolve<UriFactory>();

            container.Register(Component.For<T>().LifeStyle.Transient);
            uriFactory.Register<T>();
            routes.MapServiceRoute<T>(uriFactory.GetRoutePrefix<T>(), configuration);

            Log.DebugFormat("Registered resource. Type: [{0}]. Prefix: [{1}]. UriTemplate: [{2}].", typeof (T).Name, uriFactory.GetRoutePrefix<T>(), uriFactory.GetUriTemplateValue<T>());
        }
    }
}