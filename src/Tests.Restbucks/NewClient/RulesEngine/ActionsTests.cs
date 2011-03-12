using System.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class ActionsTests
    {
        [Test]
        public void ShouldReturnInvokerThatInvokesSuppliedAction()
        {
            var response = new HttpResponseMessage();
            var client = new HttpClient();

            var action = MockRepository.GenerateMock<IAction>();
            action.Expect(a => a.Execute(response, client));

            var actions = new Actions(client);
            var invoker = actions.Do(action);

            invoker.Invoke(response);

            action.VerifyAllExpectations();
        }
    }
}