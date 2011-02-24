using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using log4net;
using Microsoft.Net.Http;
using Restbucks.Client.Formatters;
using Restbucks.Client.Keys;
using Restbucks.MediaType;

namespace Restbucks.Client.ResponseHandlers
{
    public class RequestForQuoteFormResponseHandler : IResponseHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
        {
            Log.Debug("  Submitting a request for quote...");

            var entityBody = response.Content.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance);
            var form = entityBody.Forms.First();

            var key = new EntityBodyKey(form.MediaType, form.Schema.ToString(), context.Get<string>(ApplicationContextKeys.SemanticContext));
            var formData = context.Get<Shop>(key);

            using (var client = clientProvider.CreateClient(entityBody.BaseUri))
            {
                var content = formData.ToContent(RestbucksMediaTypeFormatter.Instance);
                content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

                var request = new HttpRequestMessage(new HttpMethod(form.Method), form.Resource) {Content = content};
                var newResponse = client.Send(request);

                return new HandlerResult(true, newResponse);
            }
        }
    }
}