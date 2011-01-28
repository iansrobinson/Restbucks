using System.ServiceModel.Dispatcher;
using Microsoft.Http;
using Microsoft.ServiceModel.Dispatcher;

namespace Restbucks.Quoting.Service.Old.Processors
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
            
            var entityBody = response.Content.ReadAsStream();

            response.Content = HttpContent.Create(output => formsSigner.SignForms(entityBody, output));
            return new ProcessorResult<object>();
        }
    }
}