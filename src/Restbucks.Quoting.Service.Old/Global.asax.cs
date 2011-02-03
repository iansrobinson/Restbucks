using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Web;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using log4net.Config;
using Microsoft.ServiceModel.Http;
using Restbucks.Quoting.Implementation;
using Restbucks.Quoting.Service.Old.Processors;
using Restbucks.Quoting.Service.Old.Resources;

namespace Restbucks.Quoting.Service.Old
{
    public class Global : HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IWindsorContainer container;

        public Global()
        {
            container = new WindsorContainer();
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();

            Log.Debug("Starting Restbucks.Quoting.Service.Old ...");

            var uriFactory = new NewUriFactory();

            container.Register(Component.For(typeof (IQuotationEngine)).ImplementedBy(typeof (QuotationEngine)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IDateTimeProvider)).ImplementedBy(typeof (DateTimeProvider)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IGuidProvider)).ImplementedBy(typeof (GuidProvider)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (ISignForms)).Instance(new FormsIntegrityUtility(Signature.Instance, OrderForm.SignedFormPlaceholder)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (FormsIntegrityResponseProcessor)).LifeStyle.Singleton);
            container.Register(Component.For(typeof(NewUriFactory)).Instance(uriFactory).LifeStyle.Singleton);

            new ResourceManager(container, RouteTable.Routes).RegisterResourcesFor(Assembly.GetExecutingAssembly());
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Log.Debug("Shutting down Restbucks.Quoting.Service.Old ...");
            container.Dispose();
        }

        private class ResourceManager
        {
            private readonly IWindsorContainer container;
            private readonly RouteCollection routes;
            private readonly HttpConfigurationWithConventions configuration;
            private readonly MethodInfo register;

            public ResourceManager(IWindsorContainer container, RouteCollection routes)
            {
                this.container = container;
                this.routes = routes;

                configuration = new Config(container);
                register = GetType().GetMethod("Register", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            public void RegisterResourcesFor(Assembly assembly)
            {
                var types = from t in assembly.GetTypes()
                            where t.GetCustomAttributes(typeof(NewUriTemplateAttribute), false).Length > 0
                            select t;

                types.ToList().ForEach(t =>
                {
                    var genericMethod = register.MakeGenericMethod(new[] { t });
                    genericMethod.Invoke(this, null);
                }
                    );
            }

            private void Register<T>() where T : class
            {
                var uriFactory = container.Resolve<NewUriFactory>();

                container.Register(Component.For<T>().LifeStyle.Transient);
                uriFactory.Register<T>();
                routes.AddServiceRoute<T>(uriFactory.GetRoutePrefix<T>(), configuration);

                Log.DebugFormat("Registered resource. Type: [{0}]. Prefix: [{1}]. UriTemplate: [{2}].", typeof(T).Name, uriFactory.GetRoutePrefix<T>(), uriFactory.GetUriTemplateValue<T>());
            }
        }
    }
}