using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Restbucks.NewClient
{
    public interface IFormInfo
    {
        Uri ResourceUri { get; }
        HttpMethod Method { get; }
        MediaTypeHeaderValue ContentType { get; }
    }
}