﻿using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public interface ILinkStrategy
    {
        LinkInfo GetLinkInfo(HttpResponseMessage response, HttpContentAdapter contentAdapter);
    }
}