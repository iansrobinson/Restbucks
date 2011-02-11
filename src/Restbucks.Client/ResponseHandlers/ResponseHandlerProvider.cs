using System;
using System.Collections.Generic;
using System.Linq;

namespace Restbucks.Client.ResponseHandlers
{
    public class ResponseHandlerProvider : IResponseHandlerProvider
    {
        private readonly IDictionary<Type, IResponseHandler> handlers;
        
        public ResponseHandlerProvider(params IResponseHandler[] responseHandlers)
        {
            handlers = new Dictionary<Type, IResponseHandler>(responseHandlers.Length);
            responseHandlers.ToList().ForEach(rh => handlers.Add(rh.GetType(), rh));
        }

        public IResponseHandler GetFor<T>() where T : IResponseHandler
        {
            return handlers[typeof(T)];
        }
    }
}