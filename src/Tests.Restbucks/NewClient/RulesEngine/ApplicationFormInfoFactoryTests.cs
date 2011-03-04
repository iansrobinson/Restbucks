using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client.Formatters;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class ApplicationFormInfoFactoryTests
    {
        [Test]
        public void ShouldCallStrategyWithSuppliedContextAndContentAdapter()
        {
            var response = new HttpResponseMessage();
            var context = new ApplicationContext(new object());
            var adapter = new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance);

            var strategy = MockRepository.GenerateMock<IFormStrategy>();
            strategy.Expect(s => s.GetFormInfo(response, context, adapter)).Return(null);

            var factory = new ApplicationFormInfoFactory(strategy, context, adapter);
            factory.CreateFormInfo(response);

            strategy.VerifyAllExpectations();
        }
    }
}