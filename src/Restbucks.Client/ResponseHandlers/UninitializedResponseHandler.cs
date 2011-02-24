﻿using System;
using System.Net.Http;
using System.Reflection;
using log4net;
using Restbucks.Client.RulesEngine;

namespace Restbucks.Client.ResponseHandlers
{
    public class UninitializedResponseHandler : IResponseHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public Result<HttpResponseMessage> Handle(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            Log.Debug("  Getting entry point...");
            
            var entryPointUri = context.Get<Uri>(ApplicationContextKeys.EntryPointUri);

            using (var client = clientProvider.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, entryPointUri);
                var newResponse = client.Send(request);

                return new Result<HttpResponseMessage>(true, newResponse);
            }
        }
    }
}