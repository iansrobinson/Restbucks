using System;

namespace Restbucks.RestToolkit.Http
{
    public interface IResponseAccessor<T> where T : class
    {
        Response<T> GetResponse(Func<Uri, Response<T>, Response<T>> client);
        void PrefetchResponse(Func<Uri, Response<T>, Response<T>> client);
        bool IsDereferenceable { get; }
    }
}