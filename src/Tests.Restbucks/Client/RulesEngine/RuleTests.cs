using System;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.RulesEngine;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class RuleTests
    {
        [Test]
        public void ShouldEvaluateCondition()
        {
            var rule1 = new Rule(() => true, typeof (DummyResponseHandler), c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"), (h, c, r) => null);
            Assert.IsTrue(rule1.IsApplicable);

            var rule2 = new Rule(() => false, typeof (DummyResponseHandler), c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"), (h, c, r) => null);
            Assert.IsFalse(rule2.IsApplicable);
        }

        private class DummyResponseHandler : IResponseHandler
        {
            public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}