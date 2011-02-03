using System;
using System.Collections.Generic;

namespace Restbucks.Quoting.Service.Old.Resources
{
    public class NewUriFactory
    {
        private readonly IDictionary<Type, UriFactoryWorker> uriFactories;

        public NewUriFactory()
        {
            uriFactories = new Dictionary<Type, UriFactoryWorker>();
        }

        public void Register<T>() where T : class
        {
            var attributes = typeof (T).GetCustomAttributes(typeof (NewUriTemplateAttribute), false);
            if (attributes.Length == 0)
            {
                throw new UriTemplateMissingException();
            }
            var uriFactory = ((NewUriTemplateAttribute) attributes[0]).UriFactoryWorker;

            uriFactories.Add(typeof (T), uriFactory);
        }

        public string GetRoutePrefix<T>() where T : class
        {
            return For<T>().RoutePrefix;
        }

        public string GetUriTemplateValue<T>() where T : class
        {
            return For<T>().UriTemplateValue;
        }

        public Uri CreateBaseUri<T>(Uri uri) where T : class
        {
            return For<T>().CreateBaseUri(uri);
        }

        public Uri CreateAbsoluteUri<T>(Uri baseUri, params string[] values) where T : class
        {
            return For<T>().CreateAbsoluteUri(baseUri, values);
        }

        public Uri CreateRelativeUri<T>(params string[] values) where T : class
        {
            return For<T>().CreateRelativeUri(values);
        }

        private UriFactoryWorker For<T>() where T : class
        {
            return uriFactories[typeof (T)];
        }
    }
}