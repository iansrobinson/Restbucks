using System;
using System.Net.Http;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public interface IRule
    {
        bool IsApplicable { get; }
        Type ResponseHandlerType { get; }
        Action<ApplicationContext> ContextAction { get; }
        Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> CreateState { get; }
    }
}