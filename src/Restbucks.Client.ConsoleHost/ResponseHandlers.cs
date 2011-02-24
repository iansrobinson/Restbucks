using System;
using Restbucks.Client.RulesEngine;
using Restbucks.Client.States;

namespace Restbucks.Client.ConsoleHost
{
    public class ResponseHandlers : IResponseHandlers
    {
        public static readonly IResponseHandlers Instance = new ResponseHandlers();

        private ResponseHandlers()
        {
        }

        public IResponseHandler Get<T>() where T : IResponseHandler
        {
            return (IResponseHandler) typeof (T).GetConstructor(new Type[] {}).Invoke(new object[] {});
        }
    }
}