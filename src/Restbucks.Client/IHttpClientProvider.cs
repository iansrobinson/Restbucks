﻿using System;
using System.Net.Http;

namespace Restbucks.Client
{
    public interface IHttpClientProvider
    {
        HttpClient CreateClient();
        HttpClient CreateClient(Uri baseUri);
    }
}