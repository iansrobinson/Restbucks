using System.Net.Http;
using Restbucks.RestToolkit.Http;

namespace Restbucks.Client.Adapters
{
    public interface IHttpResponseMessageAdapter<T> where T : class
    {
        Response<T> Adapt(HttpResponseMessage response);
    }
}