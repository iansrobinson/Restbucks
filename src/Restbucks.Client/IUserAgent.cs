using System;
using Restbucks.RestToolkit.Http;

namespace Restbucks.Client
{
    public interface IUserAgent
    {
        Response<T> Invoke<T>(Uri uri, Response<T> previousResponse) where T : class;
    }
}