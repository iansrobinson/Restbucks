using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Dispatcher;
using Microsoft.ServiceModel.Dispatcher;

namespace Restbucks.Quoting.Service.Processors
{
    public class FormsIntegrityResponseProcessor : Processor<HttpResponseMessage, object>
    {
        private readonly ISignForms formsSigner;

        public FormsIntegrityResponseProcessor(ISignForms formsSigner)
        {
            this.formsSigner = formsSigner;
            InArguments[0].Name = HttpPipelineFormatter.ArgumentHttpResponseMessage;
        }

        public override ProcessorResult<object> OnExecute(HttpResponseMessage response)
        {
            if (response.Content == null)
            {
                return new ProcessorResult<object>();
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

            return new ProcessorResult<object>();
        }
    }
}