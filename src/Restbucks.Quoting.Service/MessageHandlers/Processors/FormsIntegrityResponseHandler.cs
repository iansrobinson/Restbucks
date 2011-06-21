using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace Restbucks.Quoting.Service.MessageHandlers.Processors
{
    public class FormsIntegrityResponseHandler : HttpOperationHandler<HttpResponseMessage, HttpResponseMessage>
    {
        private readonly ISignForms formsSigner;

        public FormsIntegrityResponseHandler(ISignForms formsSigner) : base("ResponseMessage")
        {
            this.formsSigner = formsSigner;
        }

        public override HttpResponseMessage OnHandle(HttpResponseMessage response)
        {
            if (response.Content == null)
            {
                return response;
            }

            var headers = from header in response.Content.Headers
                          where (!header.Key.Equals("Content-Length"))
                          select header;

            var entityBody = response.Content.ContentReadStream;
            var output = new MemoryStream();

            formsSigner.SignForms(entityBody, output);
            response.Content = new ByteArrayContent(output.ToArray());

            foreach (var header in headers)
            {
                response.Content.Headers.Add(header.Key, header.Value);
            }

            return response;
        }
    }
}